package com.multigame.multiserver.room;

import com.multigame.multiserver.security.AESUtil;
import com.multigame.multiserver.security.JwtUtil;
import com.multigame.multiserver.member.MemberEntity;
import com.multigame.multiserver.member.MemberRepository;
import io.grpc.Context;
import io.grpc.Status;
import io.grpc.stub.StreamObserver;
import net.devh.boot.grpc.server.service.GrpcService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import room.Room;
import room.RoomServiceGrpc;

import java.util.ArrayList;
import java.util.List;
import java.util.Objects;
import java.util.Optional;

import static com.multigame.multiserver.ContextKeys.getUserIdContextKey;

@GrpcService
public class RoomServiceImpl extends RoomServiceGrpc.RoomServiceImplBase {
    @Autowired
    private RoomRepository roomRepository;

    @Autowired
    private MemberRepository memberRepository;

    @Autowired
    private BCryptPasswordEncoder encoder;

    @Autowired
    private JwtUtil jwtUtil;

    @Autowired
    private AESUtil aesUtil;

    @Override
    public void getRoomInfo(Room.GetRoomInfoRequest request, StreamObserver<Room.GetRoomInfoResponse> responseStreamObserver) {
        try {
            Optional<RoomEntity> room = roomRepository.findByRoomId(request.getRoomId());
            if (room.isPresent()) {
                Room.RoomInfo roomInfo = Room.RoomInfo.newBuilder()
                        .setRoomId(room.get().getRoomId())
                        .setRoomTitle(room.get().getRoomTitle())
                        .setMaxPlayers(room.get().getMaxPlayers())
                        .setCurPlayers(room.get().getCurrentPlayers())
                        .setIsExistPassword(room.get().isChecked())
                        .setRoomStatus(room.get().getCurrentStatus())
                        .setRoomManager(room.get().getRoomManager())
                        .build();

                Room.GetRoomInfoResponse response = Room.GetRoomInfoResponse.newBuilder()
                        .setRoomInfo(roomInfo)
                        .build();

                responseStreamObserver.onNext(response);
                responseStreamObserver.onCompleted();
            }
            else {
                responseStreamObserver.onError(
                        Status.NOT_FOUND.withDescription("User not found").asRuntimeException()
                );
            }
        } catch (Exception e) {
            responseStreamObserver.onError(Status.INTERNAL.withDescription("Internal server error: " + e.getMessage()).asRuntimeException());
        }
    }

    @Override
    public void getRoomInfoList(Room.GetRoomInfoListRequest request, StreamObserver<Room.GetRoomInfoListResponse> responseStreamObserver) {
        try {
            List<RoomEntity> rooms = roomRepository.findAll();

            List<Room.RoomInfo> roomInfoList = new ArrayList<>();

            for (RoomEntity room : rooms) {
                Room.RoomInfo roomInfo = Room.RoomInfo.newBuilder()
                        .setRoomId(room.getRoomId())
                        .setRoomTitle(room.getRoomTitle())
                        .setMaxPlayers(room.getMaxPlayers())
                        .setCurPlayers(room.getCurrentPlayers())
                        .setIsExistPassword(room.isChecked())
                        .setRoomStatus(room.getCurrentStatus())
                        .setRoomManager(room.getRoomManager())
                        .build();
                roomInfoList.add(roomInfo);
            }

            Room.GetRoomInfoListResponse response = Room.GetRoomInfoListResponse.newBuilder()
                    .addAllRooms(roomInfoList)
                    .build();

            responseStreamObserver.onNext(response);
            responseStreamObserver.onCompleted();
        } catch (Exception e) {
            responseStreamObserver.onError(Status.INTERNAL.withDescription("Internal server error: " + e.getMessage()).asRuntimeException());
        }
    }

    @Override
    public void createRoom(Room.CreateRoomRequest request, StreamObserver<Room.CreateRoomResponse> responseStreamObserver) {
        try {
            Optional<MemberEntity> user = memberRepository.findByUserNickname(request.getRoomManager());

            if (user.isPresent()) {
                RoomEntity roomEntity = new RoomEntity();
                roomEntity.setRoomId(request.getRoomId());
                roomEntity.setRoomTitle(request.getRoomTitle());
                roomEntity.setCurrentPlayers(1);
                roomEntity.setMaxPlayers(request.getMaxPlayer());
                roomEntity.setChecked(request.getIsExistPassword());
                roomEntity.setRoomPassword(encoder.encode(request.getRoomPassword()));
                roomEntity.setCurrentStatus(1);
                roomEntity.setRoomManager(request.getRoomManager());
                roomEntity.addMember(user.get());

                roomRepository.save(roomEntity);

                memberRepository.updateMemberStatus(2, user.get().getUserId());

                Room.CreateRoomResponse response = Room.CreateRoomResponse.newBuilder()
                        .setMessage("Create Room is successful!")
                        .build();

                responseStreamObserver.onNext(response);
                responseStreamObserver.onCompleted();
            }
            else {
                responseStreamObserver.onError(
                        Status.NOT_FOUND.withDescription("User not found").asRuntimeException()
                );
            }
        } catch (Exception e) {
            responseStreamObserver.onError(Status.INTERNAL.withDescription("Internal server error: " + e.getMessage()).asRuntimeException());
        }
    }

    @Override
    public void joinRoom(Room.JoinRoomRequest request, StreamObserver<Room.JoinRoomResponse> responseStreamObserver) {
        try {
            Context context = Context.current();
            String userId = getUserIdContextKey().get(context);

            Optional<RoomEntity> room = roomRepository.findByRoomId(request.getRoomId());
            Optional<MemberEntity> user = memberRepository.findByUserId(userId);

            if (room.isPresent() && user.isPresent()) {
                room.get().addMember(user.get());
                roomRepository.save(room.get());
                memberRepository.updateMemberStatus(2, userId);

                Room.JoinRoomResponse response = Room.JoinRoomResponse.newBuilder()
                        .setMessage("Join Room successful!")
                        .build();

                responseStreamObserver.onNext(response);
                responseStreamObserver.onCompleted();
            }
            else {
                responseStreamObserver.onError(
                        Status.NOT_FOUND.withDescription("Room not found").asRuntimeException()
                );
            }
        } catch (Exception e) {
            responseStreamObserver.onError(Status.INTERNAL.withDescription("Internal server error: " + e.getMessage()).asRuntimeException());
        }
    }

    @Override
    public void joinRoomWithPassword(Room.JoinRoomWithPasswordRequest request, StreamObserver<Room.JoinRoomWithPasswordResponse> responseStreamObserver) {
        try {
            Context context = Context.current();
            String userId = getUserIdContextKey().get(context);

            Optional<RoomEntity> room = roomRepository.findByRoomId(request.getRoomId());
            Optional<MemberEntity> user = memberRepository.findByUserId(userId);

            if (room.isPresent() && user.isPresent()) {
                if (!encoder.matches(request.getRoomPassword(), room.get().getRoomPassword())) {
                    responseStreamObserver.onError(
                            Status.INTERNAL.withDescription("Password is incorrect!").asRuntimeException()
                    );
                }
                room.get().addMember(user.get());
                roomRepository.save(room.get());
                memberRepository.updateMemberStatus(2, userId);

                Room.JoinRoomWithPasswordResponse response = Room.JoinRoomWithPasswordResponse.newBuilder()
                        .setMessage("Join Room successful!")
                        .build();

                responseStreamObserver.onNext(response);
                responseStreamObserver.onCompleted();
            }
            else {
                responseStreamObserver.onError(
                        Status.NOT_FOUND.withDescription("Room not found").asRuntimeException()
                );
            }
        } catch (Exception e) {
            responseStreamObserver.onError(Status.INTERNAL.withDescription("Internal server error: " + e.getMessage()).asRuntimeException());
        }
    }

    @Override
    public void exitRoom(Room.ExitRoomRequest request, StreamObserver<Room.ExitRoomResponse> responseStreamObserver) {
        try {
            Context context = Context.current();
            String userId = getUserIdContextKey().get(context);

            Optional<MemberEntity> user = memberRepository.findByUserId(userId);
            Optional<RoomEntity> room = roomRepository.findByRoomId(request.getRoomId());

            if (room.isPresent() && user.isPresent()) {
                room.get().removeMember(user.get());
                roomRepository.save(room.get());

                memberRepository.updateMemberStatus(1, userId);

                Room.ExitRoomResponse response = Room.ExitRoomResponse.newBuilder()
                        .setMessage("Exit room successful!")
                        .build();

                responseStreamObserver.onNext(response);
                responseStreamObserver.onCompleted();
            }
            else {
                responseStreamObserver.onError(
                        Status.NOT_FOUND.withDescription("Room and User not found").asRuntimeException()
                );
            }
        } catch (Exception e) {
            responseStreamObserver.onError(Status.INTERNAL.withDescription("Internal server error: " + e.getMessage()).asRuntimeException());
        }
    }

    @Override
    public void changeRoomInfo(Room.ChangeRoomInfoRequest request, StreamObserver<Room.ChangeRoomInfoResponse> responseStreamObserver) {
        try {
            Optional<RoomEntity> room = roomRepository.findByRoomId(request.getRoomId());

            if (room.isPresent()) {
                room.get().setRoomTitle(request.getRoomTitle());
                room.get().setMaxPlayers(request.getMaxPlayer());
                room.get().setChecked(request.getIsExistPassword());
                room.get().setRoomPassword(encoder.encode(request.getRoomPassword()));

                roomRepository.save(room.get());

                Room.ChangeRoomInfoResponse response = Room.ChangeRoomInfoResponse.newBuilder()
                        .setMessage("Change room information successful!")
                        .build();

                responseStreamObserver.onNext(response);
                responseStreamObserver.onCompleted();
            }
            else {
                responseStreamObserver.onError(
                        Status.NOT_FOUND.withDescription("Room not found").asRuntimeException()
                );
            }
        } catch (Exception e) {
            responseStreamObserver.onError(Status.INTERNAL.withDescription("Internal server error: " + e.getMessage()).asRuntimeException());
        }
    }

    @Override
    public void exileUser(Room.ExileUserRequest request, StreamObserver<Room.ExileUserResponse> responseStreamObserver) {
        try {
            String exileUserId = jwtUtil.getUserIdFromToken(aesUtil.decrypt(request.getUserAccessToken()));
            Optional<RoomEntity> room = roomRepository.findByRoomId(request.getRoomId());
            Optional<MemberEntity> user = memberRepository.findByUserId(exileUserId);

            if (room.isPresent() && user.isPresent()) {
                if (room.get().getRoomMembers().contains(user.get())) {
                    room.get().removeMember(user.get());
                    roomRepository.save(room.get());

                    memberRepository.updateMemberStatus(1, exileUserId);

                    Room.ExileUserResponse response = Room.ExileUserResponse.newBuilder()
                            .setMessage("Exile user successful!")
                            .build();

                    responseStreamObserver.onNext(response);
                    responseStreamObserver.onCompleted();
                }
                else {
                    responseStreamObserver.onError(
                            Status.NOT_FOUND.withDescription("User not found").asRuntimeException()
                    );
                }
            }
            else {
                responseStreamObserver.onError(
                        Status.NOT_FOUND.withDescription("Room and User not found").asRuntimeException()
                );
            }


        } catch (Exception e) {
            responseStreamObserver.onError(Status.INTERNAL.withDescription("Internal server error: " + e.getMessage()).asRuntimeException());
        }
    }

    @Override
    public void changeRoomStatus(Room.ChangeRoomStatusRequest request, StreamObserver<Room.ChangeRoomStatusResponse> responseStreamObserver) {
        try {
            Optional<RoomEntity> room = roomRepository.findByRoomId(request.getRoomId());

            if (room.isPresent()) {
                roomRepository.updateRoomStatus(request.getChangeRoomStatus(), room.get().getRoomId());

                Room.ChangeRoomStatusResponse response = Room.ChangeRoomStatusResponse.newBuilder()
                        .setMessage("Change room status successful!")
                        .build();

                responseStreamObserver.onNext(response);
                responseStreamObserver.onCompleted();
            }
            else {
                responseStreamObserver.onError(
                        Status.NOT_FOUND.withDescription("Room not found").asRuntimeException()
                );
            }
        } catch (Exception e) {
            responseStreamObserver.onError(Status.INTERNAL.withDescription("Internal server error: " + e.getMessage()).asRuntimeException());
        }
    }

    @Override
    public void deleteRoom(Room.DeleteRoomRequest request, StreamObserver<Room.DeleteRoomResponse> responseStreamObserver) {
        try {
            Optional<RoomEntity> room = roomRepository.findByRoomId(request.getRoomId());

            if (room.isPresent()) {
                for (MemberEntity user : room.get().getRoomMembers()) {
                    room.get().removeMember(user);
                }
                roomRepository.deleteByRoomId(request.getRoomId());

                Room.DeleteRoomResponse response = Room.DeleteRoomResponse.newBuilder()
                        .setMessage("Delete room successful!")
                        .build();

                responseStreamObserver.onNext(response);
                responseStreamObserver.onCompleted();
            }
            else {
                responseStreamObserver.onError(
                        Status.NOT_FOUND.withDescription("Room not found").asRuntimeException()
                );
            }
        } catch (Exception e) {
            responseStreamObserver.onError(Status.INTERNAL.withDescription("Internal server error: " + e.getMessage()).asRuntimeException());
        }
    }

    @Override
    public void changeRoomManager(Room.ChangeRoomManagerRequest request, StreamObserver<Room.ChangeRoomManagerResponse> responseStreamObserver) {
        try {
            Optional<RoomEntity> room = roomRepository.findByRoomId(request.getRoomId());

            if (room.isPresent()) {
                if (Objects.equals(room.get().getRoomManager(), request.getCurrentRoomManager())) {
                    room.get().setRoomManager(request.getNewRoomManager());
                    roomRepository.save(room.get());

                    Room.ChangeRoomManagerResponse response = Room.ChangeRoomManagerResponse.newBuilder()
                            .setMessage("Change room manager successful!")
                            .build();

                    responseStreamObserver.onNext(response);
                    responseStreamObserver.onCompleted();
                }
                else {
                    responseStreamObserver.onError(
                            Status.INTERNAL.withDescription("Current manager is not equal").asRuntimeException()
                    );
                }
            }
            else {
                responseStreamObserver.onError(
                        Status.NOT_FOUND.withDescription("Room not found").asRuntimeException()
                );
            }
        } catch (Exception e) {
            responseStreamObserver.onError(Status.INTERNAL.withDescription("Internal server error: " + e.getMessage()).asRuntimeException());
        }
    }
}

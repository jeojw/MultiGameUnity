package com.multigame.multiserver.member;

import com.multigame.multiserver.auth.JwtUtil;
import io.grpc.stub.StreamObserver;
import member.Member;
import member.MemberServiceGrpc;
import net.devh.boot.grpc.server.service.GrpcService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.stereotype.Service;

import java.util.Optional;

@GrpcService
public class MemberServiceImpl extends MemberServiceGrpc.MemberServiceImplBase {
    @Autowired
    private MemberRepository memberRepository;

    @Autowired
    private BCryptPasswordEncoder encoder;

    @Autowired
    private JwtUtil jwtUtil;

    @Override
    public void signUp(Member.SignUpRequest request, StreamObserver<Member.SignUpResponse> responseStreamObserver) {
        try {
            MemberEntity member = new MemberEntity();
            member.setUserId(request.getUserId());
            member.setUserPassword(encoder.encode(request.getUserPassword()));
            member.setUserNickname(request.getUserNickname());
            member.setProfileName(request.getProfileName());
            member.setProfileType(request.getProfileType());
            member.setProfileData(request.getProfileData().toByteArray());
            memberRepository.save(member);

            Member.SignUpResponse response = Member.SignUpResponse.newBuilder()
                    .setMessage("Sign-up successful!")
                    .build();

            responseStreamObserver.onNext(response);
            responseStreamObserver.onCompleted();
        } catch (Exception e) {
            responseStreamObserver.onError(e);
        }
    }

    @Override
    public void checkDuplicateId(Member.CheckDuplicateIdRequest request, StreamObserver<Member.CheckDuplicateIdResponse> responseStreamObserver) {
        try {
            Member.CheckDuplicateIdResponse response = Member.CheckDuplicateIdResponse.newBuilder()
                    .setIsIdDuplicate(memberRepository.existsByUserId(request.getUserId()))
                    .build();

            responseStreamObserver.onNext(response);
            responseStreamObserver.onCompleted();
        } catch (Exception e) {
            responseStreamObserver.onError(e);
        }
    }

    @Override
    public void checkDuplicateNickname(Member.CheckDuplicateNicknameRequest request, StreamObserver<Member.CheckDuplicateNicknameResponse> responseStreamObserver) {
        try {
            Member.CheckDuplicateNicknameResponse response = Member.CheckDuplicateNicknameResponse.newBuilder()
                    .setIsNicknameDuplicate(memberRepository.existsByUserNickname(request.getUserNickname()))
                    .build();

            responseStreamObserver.onNext(response);
            responseStreamObserver.onCompleted();
        } catch (Exception e) {
            responseStreamObserver.onError(e);
        }
    }

    @Override
    public void checkDuplicateNicknameWithToken(Member.CheckDuplicateNicknameWithTokenRequest request, StreamObserver<Member.CheckDuplicateNicknameWithTokenResponse> responseStreamObserver) {
        try {
            if (!jwtUtil.validateToken(request.getToken())) {
                return;
            }

            Member.CheckDuplicateNicknameWithTokenResponse response = Member.CheckDuplicateNicknameWithTokenResponse.newBuilder()
                    .setIsDuplicate(memberRepository.existsByUserNickname(request.getNewNickname()))
                    .build();

            responseStreamObserver.onNext(response);
            responseStreamObserver.onCompleted();
        } catch (Exception e) {
            responseStreamObserver.onError(e);
        }
    }

    @Override
    public void updateNickname(Member.UpdateNicknameRequest request, StreamObserver<Member.UpdateNicknameResponse> responseStreamObserver) {
        try {
            String userId = jwtUtil.getUserIdFromToken(request.getToken());
            Optional<MemberEntity> member = memberRepository.findByUserId(userId);
            if (member.isPresent()) {
                member.get().setUserNickname(request.getUserNickname());
                memberRepository.save(member.get());

                Member.UpdateNicknameResponse response = Member.UpdateNicknameResponse.newBuilder()
                        .setMessage("Nickname updated sucessfully!")
                        .build();

                responseStreamObserver.onNext(response);
                responseStreamObserver.onCompleted();
            }
        } catch (Exception e) {
            responseStreamObserver.onError(e);
        }
    }

    @Override
    public void updatePassword(Member.UpdatePasswordRequest request, StreamObserver<Member.UpdatePasswordResponse> responseStreamObserver) {
        try {
            String userId = jwtUtil.getUserIdFromToken(request.getToken());
            Optional<MemberEntity> member = memberRepository.findByUserId(userId);
            if (member.isPresent()) {
                if (!encoder.matches(request.getOldPassword(), member.get().getUserPassword())) {
                    throw new IllegalArgumentException("Old password is incorrect");
                }

                member.get().setUserPassword(encoder.encode(request.getNewPassword()));
                memberRepository.save(member.get());

                Member.UpdatePasswordResponse response = Member.UpdatePasswordResponse.newBuilder()
                        .setMessage("Nickname updated sucessfully!")
                        .build();

                responseStreamObserver.onNext(response);
                responseStreamObserver.onCompleted();
            }
        } catch (Exception e) {
            responseStreamObserver.onError(e);
        }
    }
}

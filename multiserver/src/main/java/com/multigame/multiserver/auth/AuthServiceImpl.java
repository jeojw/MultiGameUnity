package com.multigame.multiserver.auth;

import auth.Auth;
import auth.AuthServiceGrpc;
import com.multigame.multiserver.member.MemberEntity;
import com.multigame.multiserver.member.MemberRepository;
import io.grpc.stub.StreamObserver;
import net.devh.boot.grpc.server.service.GrpcService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;

import java.util.Optional;

@GrpcService
public class AuthServiceImpl extends AuthServiceGrpc.AuthServiceImplBase {
    @Autowired
    private MemberRepository memberRepository;

    @Autowired
    private JwtUtil jwtUtil;

    @Autowired
    private RedisUtil redisUtil;

    @Autowired
    private BCryptPasswordEncoder encoder;

    @Value("${spring.jwt.token.access-expiration-time}")
    private long refreshExpirationTime;

    @Override
    public void signUp(Auth.SignUpRequest request, StreamObserver<Auth.SignUpResponse> responseStreamObserver) {
        try {
            MemberEntity member = new MemberEntity();
            member.setUserId(request.getUserId());
            member.setUserPassword(encoder.encode(request.getUserPassword()));
            member.setUserNickname(request.getUserNickname());
            member.setProfileName(request.getProfileName());
            member.setProfileData(request.getProfileData().toByteArray());
            memberRepository.save(member);

            Auth.SignUpResponse response = Auth.SignUpResponse.newBuilder()
                    .setMessage("Sign-up successful!")
                    .build();

            responseStreamObserver.onNext(response);
            responseStreamObserver.onCompleted();
        } catch (Exception e) {
            responseStreamObserver.onError(e);
        }
    }

    @Override
    public void signIn(Auth.SignInRequest request, StreamObserver<Auth.SignInResponse> responseStreamObserver) {
        try {
            Optional<MemberEntity> member = memberRepository.findByUserId(request.getUserId());

            if (member.isPresent()) {
                if (!encoder.matches(request.getUserPassword(), member.get().getUserPassword())) {
                    throw new IllegalArgumentException("Invalid password");
                }

                String accessToken = jwtUtil.generateAccessToken(member.get().getUserId());
                String refreshToken = jwtUtil.generateRefreshToken(member.get().getUserId());

                redisUtil.saveRefreshToken(request.getUserId(), refreshToken, refreshExpirationTime);

                Auth.SignInResponse response = Auth.SignInResponse.newBuilder()
                        .setAccessToken(accessToken)
                        .setRefreshToken(refreshToken)
                        .setUserNickname(member.get().getUserNickname())
                        .setProfileData(com.google.protobuf.ByteString.copyFrom(member.get().getProfileData()))
                        .build();

                responseStreamObserver.onNext(response);
                responseStreamObserver.onCompleted();
            }
        } catch (Exception e) {
            responseStreamObserver.onError(e);
        }
    }

    @Override
    public void validateToken(Auth.ValidateTokenRequest request, StreamObserver<Auth.ValidateTokenResponse> responseStreamObserver) {
        String token = request.getToken();
        boolean isValid = jwtUtil.validateToken(token);

        Auth.ValidateTokenResponse response = Auth.ValidateTokenResponse.newBuilder()
                .setIsValid(isValid)
                .build();
        responseStreamObserver.onNext(response);
        responseStreamObserver.onCompleted();
    }

    @Override
    public void refreshToken(Auth.RefreshTokenRequest request, StreamObserver<Auth.RefreshTokenResponse> responseStreamObserver) {
        String refreshToken = request.getRefreshToken();

        String userId = jwtUtil.getUserIdFromToken(refreshToken);

        String storedToken = redisUtil.getRefreshToken(userId);
        if (storedToken == null || !storedToken.equals(refreshToken)) {
            responseStreamObserver.onError(new RuntimeException("Invalid refresh token"));
            return;
        }

        String newAccessToken = jwtUtil.generateAccessToken(userId);

        Auth.RefreshTokenResponse response = Auth.RefreshTokenResponse.newBuilder()
                .setAccessToken(newAccessToken)
                .build();
        responseStreamObserver.onNext(response);
        responseStreamObserver.onCompleted();
    }

    @Override
    public void updateNickname(Auth.UpdateNicknameRequest request, StreamObserver<Auth.UpdateNicknameResponse> responseStreamObserver) {
        try {
            String userId = jwtUtil.getUserIdFromToken(request.getToken());
            Optional<MemberEntity> member = memberRepository.findByUserId(userId);
            if (member.isPresent()) {
                member.get().setUserNickname(request.getUserNickname());
                memberRepository.save(member.get());

                Auth.UpdateNicknameResponse response = Auth.UpdateNicknameResponse.newBuilder()
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
    public void updatePassword(Auth.UpdatePasswordRequest request, StreamObserver<Auth.UpdatePasswordResponse> responseStreamObserver) {
        try {
            String userId = jwtUtil.getUserIdFromToken(request.getToken());
            Optional<MemberEntity> member = memberRepository.findByUserId(userId);
            if (member.isPresent()) {
                if (!encoder.matches(request.getOldPassword(), member.get().getUserPassword())) {
                    throw new IllegalArgumentException("Old password is incorrect");
                }

                member.get().setUserPassword(encoder.encode(request.getNewPassword()));
                memberRepository.save(member.get());

                Auth.UpdatePasswordResponse response = Auth.UpdatePasswordResponse.newBuilder()
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

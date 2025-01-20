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
import org.springframework.stereotype.Service;

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


}

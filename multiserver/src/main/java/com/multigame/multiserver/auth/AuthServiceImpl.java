package com.multigame.multiserver.auth;

import auth.Auth;
import auth.AuthServiceGrpc;
import com.multigame.multiserver.member.MemberEntity;
import com.multigame.multiserver.member.MemberRepository;
import io.grpc.Context;
import io.grpc.Status;
import io.grpc.stub.StreamObserver;
import net.devh.boot.grpc.server.service.GrpcService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;

import java.util.Optional;

import static com.multigame.multiserver.ContextKeys.getUserIdContextKey;

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

    @Autowired
    private AESUtil aesUtil;

    @Value("${spring.jwt.token.access-expiration-time}")
    private long refreshExpirationTime;

    @Override
    public void signIn(Auth.SignInRequest request, StreamObserver<Auth.SignInResponse> responseStreamObserver) {
        try {
            Optional<MemberEntity> member = memberRepository.findByUserId(request.getUserId());

            if (member.isEmpty()) {
                throw new IllegalArgumentException("User not found");
            }

            if (!encoder.matches(request.getUserPassword(), member.get().getUserPassword())) {
                throw new IllegalArgumentException("Invalid password");
            }

            String accessToken = aesUtil.encrypt(jwtUtil.generateAccessToken(member.get().getUserId()));
            String refreshToken = jwtUtil.generateRefreshToken(member.get().getUserId());

            redisUtil.saveRefreshToken(request.getUserId(), refreshToken, refreshExpirationTime);

            memberRepository.updateMemberStatus(1, request.getUserId());

            Auth.SignInResponse response = Auth.SignInResponse.newBuilder()
                    .setAccessToken(accessToken)
                    .build();

            responseStreamObserver.onNext(response);
            responseStreamObserver.onCompleted();
        } catch (Exception e) {
            responseStreamObserver.onError(Status.INTERNAL.withDescription("Internal server error: " + e.getMessage()).asRuntimeException());
        }
    }

    @Override
    public void signOut(Auth.SignOutRequest request, StreamObserver<Auth.SignOutResponse> responseStreamObserver) {
        Context context = Context.current();
        String userId = getUserIdContextKey().get(context);

        redisUtil.deleteRefreshToken(userId);

        memberRepository.updateMemberStatus(-1, userId);

        Auth.SignOutResponse response = Auth.SignOutResponse.newBuilder()
                .setMessage("Logout Success!")
                .build();
        responseStreamObserver.onNext(response);
        responseStreamObserver.onCompleted();
    }

    @Override
    public void refreshToken(Auth.RefreshTokenRequest request, StreamObserver<Auth.RefreshTokenResponse> responseStreamObserver) {
        Context context = Context.current();
        String userId = getUserIdContextKey().get(context);

        String storedToken = redisUtil.getRefreshToken(userId);
        if (storedToken == null) {
            responseStreamObserver.onError(Status.UNAUTHENTICATED
                    .withDescription("Refresh token expired. Please log in again.")
                    .asRuntimeException());
            return;
        }

        try {
            String newAccessToken = aesUtil.encrypt(jwtUtil.generateAccessToken(userId));

            Auth.RefreshTokenResponse response = Auth.RefreshTokenResponse.newBuilder()
                    .setAccessToken(newAccessToken)
                    .build();
            responseStreamObserver.onNext(response);
            responseStreamObserver.onCompleted();
        } catch (Exception e) {
            responseStreamObserver.onError(e);
        }
    }
}

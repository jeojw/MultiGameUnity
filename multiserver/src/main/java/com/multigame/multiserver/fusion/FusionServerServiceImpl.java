package com.multigame.multiserver.fusion;

import com.multigame.multiserver.security.AESUtil;
import com.multigame.multiserver.security.JwtUtil;
import com.multigame.multiserver.security.RedisUtil;
import fusionServer.FusionServer;
import fusionServer.FusionServerServiceGrpc;
import io.grpc.Status;
import io.grpc.stub.StreamObserver;
import lombok.extern.slf4j.Slf4j;
import net.devh.boot.grpc.server.service.GrpcService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;

@Slf4j
@GrpcService
public class FusionServerServiceImpl extends FusionServerServiceGrpc.FusionServerServiceImplBase {
    @Autowired
    private JwtUtil jwtUtil;

    @Autowired
    private RedisUtil redisUtil;

    @Autowired
    private AESUtil aesUtil;

    @Value("${spring.jwt.token.access-expiration-time}")
    private long refreshExpirationTime;

    @Override
    public void generateToken(FusionServer.GenerateTokenRequest request, StreamObserver<FusionServer.GenerateTokenResponse> responseStreamObserver) {
        try {
            String accessToken = aesUtil.encrypt(jwtUtil.generateAccessToken("fusion"));
            String refreshToken = jwtUtil.generateRefreshToken("fusion");

            redisUtil.saveRefreshToken("fusion", refreshToken, refreshExpirationTime);

            FusionServer.GenerateTokenResponse response = FusionServer.GenerateTokenResponse.newBuilder()
                    .setAccessToken(accessToken)
                    .build();

            responseStreamObserver.onNext(response);
            responseStreamObserver.onCompleted();
        } catch (Exception e) {
            responseStreamObserver.onError(Status.INTERNAL.withDescription("Internal server error: " + e.getMessage()).asRuntimeException());
        }
    }
}

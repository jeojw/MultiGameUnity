package com.multigame.multiserver;

import com.multigame.multiserver.auth.AuthServiceImpl;
import com.multigame.multiserver.auth.JwtRedisAuthInterceptor;
import com.multigame.multiserver.member.MemberServiceImpl;
import io.grpc.Server;
import io.grpc.netty.shaded.io.grpc.netty.NettyServerBuilder;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Configuration;

import javax.annotation.PostConstruct;
import javax.annotation.PreDestroy;
import java.util.concurrent.TimeUnit;

@Configuration
public class GrpcServerConfig {

    private static final Logger log = LoggerFactory.getLogger(GrpcServerConfig.class);


    @Value("${grpc.server.port}")
    private int grpcServerPort;

    private Server grpcServer;

    @Autowired
    private MemberServiceImpl memberServiceImpl;

    @Autowired
    private AuthServiceImpl authServiceImpl;

    @Autowired
    private JwtRedisAuthInterceptor jwtRedisAuthInterceptor;

    @PostConstruct
    public void startGrpcServer() throws Exception {
        try {
            if (grpcServer == null || grpcServer.isShutdown()) {
                log.info("Starting gRPC server on port: {}", grpcServerPort);

                NettyServerBuilder nettyServerBuilder = NettyServerBuilder.forPort(grpcServerPort);

                // 서버 시작
                grpcServer = nettyServerBuilder.build().start();
                log.info("gRPC Server started successfully on port: {}", grpcServerPort);
            }
        } catch (Exception e) {
            log.error("Error starting gRPC server: {}", e.getMessage(), e);
        }
    }

    @PreDestroy
    public void stopGrpcServer() throws InterruptedException {
        if (grpcServer != null) {
            grpcServer.shutdown().awaitTermination(30, TimeUnit.SECONDS);
            log.info("gRPC Server stopped successfully.");
        }
    }
}

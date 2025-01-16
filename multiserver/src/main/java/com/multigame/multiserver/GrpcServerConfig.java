package com.multigame.multiserver;

import com.multigame.multiserver.auth.AuthServiceImpl;
import io.grpc.Server;
import io.grpc.ServerBuilder;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class GrpcServerConfig {

    @Bean
    public Server grpcServer(AuthServiceImpl authService) throws  Exception {
        return ServerBuilder.forPort(50051)
                .addService(authService)
                .build()
                .start();
    }
}

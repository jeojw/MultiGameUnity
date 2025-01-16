package com.multigame.multiserver;

import com.multigame.multiserver.auth.AuthServiceImpl;
import com.multigame.multiserver.member.MemberServiceImpl;
import io.grpc.Server;
import io.grpc.ServerBuilder;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;

@Configuration
public class GrpcServerConfig {

    @Value("${grpc.server.port}")
    private int port;

    @Bean
    public Server grpcServer(AuthServiceImpl authService, MemberServiceImpl memberService) throws  Exception {
        return ServerBuilder.forPort(port)
                .addService(authService)
                .addService(memberService)
                .build()
                .start();
    }

    @Bean
    public BCryptPasswordEncoder passwordEncoder() {
        return new BCryptPasswordEncoder();
    }
}

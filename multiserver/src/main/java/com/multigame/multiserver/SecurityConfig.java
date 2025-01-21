package com.multigame.multiserver;

import net.devh.boot.grpc.server.serverfactory.GrpcServerFactory;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.web.SecurityFilterChain;

@Configuration
public class SecurityConfig {

    @Bean
    public SecurityFilterChain filterChain(HttpSecurity http) throws Exception {
        http
                .authorizeHttpRequests(authz -> authz
                        .requestMatchers("/member.MemberService/SignUp",
                                "/member.MemberService/CheckDuplicateId",
                                "/member.MemberService/CheckDuplicateNickname")
                        .permitAll()  // gRPC-Web 요청에 대해 인증 없이 접근 허용
                        .anyRequest()
                        .authenticated()// 다른 모든 요청은 인증 필요
                );

        return http.build();
    }

    @Bean
    public BCryptPasswordEncoder passwordEncoder() {
        return new BCryptPasswordEncoder();
    }
}

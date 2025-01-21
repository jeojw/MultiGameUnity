package com.multigame.multiserver.auth;

import io.grpc.*;
import io.jsonwebtoken.JwtException;
import lombok.extern.slf4j.Slf4j;
import net.devh.boot.grpc.server.interceptor.GrpcGlobalServerInterceptor;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.HashSet;
import java.util.List;
import java.util.Set;

@GrpcGlobalServerInterceptor
@Component
@Slf4j
public class JwtRedisAuthInterceptor implements ServerInterceptor {

    @Autowired
    private JwtUtil jwtUtil;

    @Autowired
    private RedisUtil redisUtil;

    private static final Set<String> EXEMPT_METHODS = new HashSet<>(List.of(
            "/member.MemberService/CheckDuplicateId"
    ));

    @Override
    public <ReqT, RespT> ServerCall.Listener<ReqT> interceptCall(
            ServerCall<ReqT, RespT> call,
            Metadata headers,
            ServerCallHandler<ReqT, RespT> next) {

        log.debug("gRPC Request Headers: {}", headers);
        log.debug("gRPC Call Method: {}", call.getMethodDescriptor().getFullMethodName());

        String methodName = call.getMethodDescriptor().getFullMethodName();

        // 인증이 면제되는 메소드
//        if (EXEMPT_METHODS.contains(methodName)) {
//            return next.startCall(call, headers);
//        }
//
//        // Authorization 헤더에서 JWT 추출
//        Metadata.Key<String> authKey = Metadata.Key.of("Authorization", Metadata.ASCII_STRING_MARSHALLER);
//        String authHeader = headers.get(authKey);
//
//        if (authHeader == null || !authHeader.startsWith("Bearer ")) {
//            log.error("Missing or invalid Authorization header");
//            call.close(Status.UNAUTHENTICATED.withDescription("Missing or invalid Authorization header"), headers);
//            return new ServerCall.Listener<>() {};
//        }
//
//        String token = authHeader.substring("Bearer ".length());
//
//        // 토큰 유효성 검사
//        if (!validateToken(token, headers, call)) {
//            return new ServerCall.Listener<>() {};
//        }

        return next.startCall(call, headers);
    }

    private boolean validateToken(String token, Metadata headers, ServerCall<?, ?> call) {
        try {
            String userId = jwtUtil.getUserIdFromToken(token);
            String refreshToken = redisUtil.getRefreshToken(userId);
            if (refreshToken == null || !refreshToken.equals(token)) {
                log.error("Token is invalid or blacklisted");
                call.close(Status.UNAUTHENTICATED.withDescription("Token is invalid or blacklisted"), headers);
                return false;
            }
            log.info("Authenticated user ID: {}", userId);
            return true;
        } catch (JwtException e) {
            log.error("Invalid token: {}", e.getMessage());
            call.close(Status.UNAUTHENTICATED.withDescription("Invalid token"), headers);
            return false;
        }
    }
}

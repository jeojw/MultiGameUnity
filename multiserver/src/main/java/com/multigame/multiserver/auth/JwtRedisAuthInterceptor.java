package com.multigame.multiserver.auth;

import io.grpc.*;
import io.jsonwebtoken.ExpiredJwtException;
import lombok.extern.slf4j.Slf4j;
import net.devh.boot.grpc.server.interceptor.GrpcGlobalServerInterceptor;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.HashSet;
import java.util.List;
import java.util.Set;

import static com.multigame.multiserver.ContextKeys.getUserIdContextKey;


@GrpcGlobalServerInterceptor
@Component
@Slf4j
public class JwtRedisAuthInterceptor implements ServerInterceptor {

    @Autowired
    private JwtUtil jwtUtil;

    @Autowired
    private RedisUtil redisUtil;

    @Autowired
    private AESUtil aesUtil;

    private static final Set<String> EXEMPT_METHODS = new HashSet<>(List.of(
            "member.MemberService/CheckDuplicateId",
            "member.MemberService/CheckDuplicateNickname",
            "member.MemberService/SignUp",
            "auth.AuthService/SignIn"
    ));

    @Override
    public <ReqT, RespT> ServerCall.Listener<ReqT> interceptCall(
            ServerCall<ReqT, RespT> call,
            Metadata headers,
            ServerCallHandler<ReqT, RespT> next) {

        String methodName = call.getMethodDescriptor().getFullMethodName();

        if (EXEMPT_METHODS.contains(methodName)) {
            return next.startCall(call, headers);
        }

        String authHeader = headers.get(Metadata.Key.of("Authorization", Metadata.ASCII_STRING_MARSHALLER));

        if (authHeader == null || !authHeader.startsWith("Bearer ")) {
            log.error("Missing or invalid Authorization header");
            call.close(Status.UNAUTHENTICATED.withDescription("Missing or invalid Authorization header"), headers);
            return new ServerCall.Listener<>() {};
        }

        String token = authHeader.substring("Bearer ".length());
        try {
            token = aesUtil.decrypt(token);

            if (!validateToken(token, headers, call)) {
                call.close(Status.UNAUTHENTICATED.withDescription("Invalid token"), headers);
                return new ServerCall.Listener<>() {};
            }

            String userId = jwtUtil.getUserIdFromToken(token);

            Context context = Context.current().withValue(getUserIdContextKey(), userId);
            log.info("USER_ID_CONTEXT_KEY hashcode: {}", getUserIdContextKey().hashCode());

            return Contexts.interceptCall(context, new ForwardingServerCall.SimpleForwardingServerCall<ReqT, RespT>(call) {
                @Override
                public void close(Status status, Metadata trailers) {
                    context.detach(Context.current().attach());
                    super.close(status, trailers);
                }
            }, headers, next);

        } catch (Exception e) {
            call.close(Status.UNAUTHENTICATED.withDescription("Error while processing token").withCause(e), headers);
            return new ServerCall.Listener<>() {};
        }
    }

    private boolean validateToken(String token, Metadata headers, ServerCall<?, ?> call) {
        try {
            String userId = jwtUtil.getUserIdFromToken(token);
            String refreshToken = redisUtil.getRefreshToken(userId);
            if (refreshToken == null) {
                call.close(Status.UNAUTHENTICATED.withDescription("Token is invalid or blacklisted"), headers);
                return false;
            }
            log.info("Authenticated user ID: {}", userId);
            return true;
        } catch (ExpiredJwtException e) {
            log.error("Token expired: {}", e.getMessage());
            return false;
        } catch (Exception e) {
            log.error("Invalid token: {}", e.getMessage());
            return false;
        }
    }
}

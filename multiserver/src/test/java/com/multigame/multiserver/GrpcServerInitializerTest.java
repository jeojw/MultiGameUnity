package com.multigame.multiserver;

import com.multigame.multiserver.member.MemberServiceImpl;
import io.grpc.ManagedChannel;
import io.grpc.ManagedChannelBuilder;
import member.Member;
import member.MemberServiceGrpc;
import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.test.web.server.LocalServerPort;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;

@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT, classes = MultiserverApplication.class)
public class GrpcServerInitializerTest {
    @LocalServerPort
    private int port;

    private ManagedChannel channel;

    @BeforeEach
    public void setUp() {
        // gRPC 채널 생성 (서버의 동적 포트 사용)
        channel = ManagedChannelBuilder.forAddress("127.0.0.1", 50051)
                .usePlaintext() // 보안 연결이 아니라면 사용
                .build();
    }

    @Test
    public void testGrpcService() {
        // 예시: gRPC 서비스 호출
        // 예를 들어 MyGrpcService의 메서드를 테스트하고 그 응답을 확인하는 방식
        MemberServiceGrpc.MemberServiceBlockingStub stub = MemberServiceGrpc.newBlockingStub(channel);
        Member.CheckDuplicateIdRequest request = Member.CheckDuplicateIdRequest.newBuilder().setUserId("jh").build();
        Member.CheckDuplicateIdResponse response = stub.checkDuplicateId(request);

        // 응답값 검증
        assertNotNull(response);
        assertEquals("expectedValue", response.getIsIdDuplicate());
    }

    @AfterEach
    public void tearDown() {
        // 리소스 해제
        if (channel != null) {
            channel.shutdown();
        }
    }
}

package com.multigame.multiserver.member;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Modifying;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface MemberRepository extends JpaRepository<MemberEntity, Long> {
    Optional<MemberEntity> findByUserId(String userId);

    Optional<MemberEntity> findByUserNickname(String userNickname);

    boolean existsByUserId(String userId);

    boolean existsByUserNickname(String userNickname);

    @Modifying
    @Query(value = "UPDATE MultiServer.member_table SET member_status = :status WHERE user_id = :userId",
            nativeQuery = true)
    void updateMemberStatus(@Param("status") int status, @Param("userId") String memberId);
}

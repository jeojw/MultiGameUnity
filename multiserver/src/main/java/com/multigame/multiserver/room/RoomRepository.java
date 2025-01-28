package com.multigame.multiserver.room;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Modifying;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

@Repository
public interface RoomRepository extends JpaRepository<RoomEntity, Long> {

    Optional<RoomEntity> findByRoomId(String roomId);

    @Modifying
    @Query(value = "UPDATE MultiServer.room_table SET current_status = :status WHERE room_id = :roomId",
            nativeQuery = true)
    List<RoomEntity> updateRoomStatus(@Param("status") int status, @Param("roomId") String roomId);

    void deleteByRoomId(String roomId);
}

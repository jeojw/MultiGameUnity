package com.multigame.multiserver.room;

import com.multigame.multiserver.member.MemberEntity;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.util.ArrayList;
import java.util.List;

@Entity
@Getter
@Setter
@NoArgsConstructor
@Table(name = "room_table")
public class RoomEntity {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(name="room_id")
    private String roomId;

    @Column(name="room_title")
    private String roomTitle;

    @Column(name="current_players")
    private int currentPlayers;

    @Column(name="max_players")
    private int maxPlayers;

    @Column(name="is_checked")
    private boolean isChecked;

    @Column(name="room_password")
    private String roomPassword;

    @Column(name="current_status")
    private int currentStatus;

    @Column(name="room_manager")
    private String roomManager;

    @OneToMany(mappedBy = "room", cascade = CascadeType.ALL, orphanRemoval = true)
    private List<MemberEntity> roomMembers = new ArrayList<>();

    public void addMember(MemberEntity member) {
        roomMembers.add(member);
        member.setRoom(this);
    }

    public void removeMember(MemberEntity member) {
        roomMembers.remove(member);
        member.setRoom(null); // 연관 관계 해제
    }
}

package com.multigame.multiserver.member;

import com.multigame.multiserver.room.RoomEntity;
import jakarta.persistence.*;
import lombok.*;

@Entity
@Getter
@Setter
@NoArgsConstructor
@Table(name = "member_table")
public class MemberEntity {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(name = "user_id", unique = true)
    private String userId;

    @Column(name = "user_password")
    private String userPassword;

    @Column(name = "user_nickname", unique = true)
    private String userNickname;

    @Column(name = "profile_name")
    private String profileName;

    @Column(name = "profile_type")
    private String profileType;

    @Lob
    @Column(name = "profile_file", columnDefinition = "BLOB")
    private byte[] profileData;

    @Column(name = "member_status")
    private int memberStatus;

    @ManyToOne(cascade = CascadeType.PERSIST)
    @JoinColumn(name = "room_id")
    private RoomEntity room;
}

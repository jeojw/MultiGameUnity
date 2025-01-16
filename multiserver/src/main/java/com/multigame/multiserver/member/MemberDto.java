package com.multigame.multiserver.member;

import lombok.Data;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@Data
public class MemberDto {
    private String userId;
    private String userPassword;
    private String userNickname;
    private String profileName;
    private String profileType;
    private String profileData;
}

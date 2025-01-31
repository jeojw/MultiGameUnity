using Grpc.Core;
using Member;
using System.Threading.Tasks;
using UnityEngine;

public class MemberServiceManager : MonoBehaviour
{
    private MemberService.MemberServiceClient client;

    public MemberServiceManager(GrpcClientManager grpcClientManager)
    {
        client = grpcClientManager.GetMemberClient();
    }
    public async Task<CheckDuplicateNicknameResponse> CheckDuplicateNicknameAsync(string userNickname)
    {
        var request = new CheckDuplicateNicknameRequest
        {
            UserNickname = userNickname
        };
        return await client.CheckDuplicateNicknameAsync(request);
    }

    public async Task<CheckDuplicateIdResponse> CheckDuplicateIdAsync(string userId)
    {
        var request = new CheckDuplicateIdRequest
        {
            UserId = userId
        };
        return await client.CheckDuplicateIdAsync(request);
    }

    public async Task<CheckDuplicateNicknameWithTokenResponse> CheckDuplicateNicknameWithTokenAsync(string token, string newNickname)
    {
        var request = new CheckDuplicateNicknameWithTokenRequest
        {
            NewNickname = newNickname
        };
        var header = new Metadata
        {
            {"Authorization", $"Bearer {token}"}
        };
        return await client.CheckDuplicateNicknameWithTokenAsync(request, header);
    }

    public async Task<SignUpResponse> SignUpAsync(
            string userId,
            string userPassword,
            string userNickname,
            string profileName,
            string profileType,
            Google.Protobuf.ByteString profileData
        )
    {
        var request = new SignUpRequest
        {
            UserId = userId,
            UserPassword = userPassword,
            UserNickname = userNickname,
            ProfileType = profileType,
            ProfileData = profileData,
            ProfileName = profileName
        };
        return await client.SignUpAsync(request);
    }
    public async Task<UserInfoResponse> UserInfoAsync(string token)
    {
        var request = new UserInfoRequest();
        var header = new Metadata
        {
            {"Authorization", $"Bearer {token}"}
        };
        return await client.UserInfoAsync(request, header);
    }
}

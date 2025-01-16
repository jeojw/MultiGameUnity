using Grpc.Core;
using Member;
using System.Threading.Tasks;
using UnityEngine;

public class MemberServiceManager
{
    private MemberService.MemberServiceClient client;

    private static MemberServiceManager instance;
    public static MemberServiceManager Instance => instance ??= new MemberServiceManager();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public MemberServiceManager()
    {
        client = GrpcClientManager.Instance.GetMemberClient();
    }

    public async Task<CheckDuplicateNicknameResponse> CheckDuplicateNicknameAsync(string userNickname)
    {
        var request = new CheckDuplicateNicknameRequest
        {
            UserNickname = userNickname
        };
        try
        {
            return await client.CheckDuplicateNicknameAsync(request);
        }
        catch (RpcException e)
        {
            Debug.LogError($"gRPC Error: {e.Status.Detail}");
            throw;
        }
    }

    public async Task<CheckDuplicateIdResponse> CheckDuplicateIdAsync(string userId)
    {
        var request = new CheckDuplicateIdRequest
        {
            UserId = userId
        };
        try
        {
            return await client.CheckDuplicateIdAsync(request);
        }
        catch(RpcException e)
        {
            throw;
        }
    }

    public async Task<CheckDuplicateNicknameWithTokenResponse> CheckDuplicateNicknameWithTokenAsync(string token, string newNickname)
    {
        var request = new CheckDuplicateNicknameWithTokenRequest
        {
            Token = token,
            NewNickname = newNickname
        };
        try
        {
            return await client.CheckDuplicateNicknameWithTokenAsync(request);
        }
        catch (RpcException e)
        {
            Debug.LogError($"gRPC Error: {e.Status.Detail}");
            throw;
        }
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
        try
        {

            return await client.SignUpAsync(request);
        }
        catch (RpcException e)
        {
            Debug.LogError($"gRPC Error: {e.Status.Detail}");
            throw;
        }
    }
}

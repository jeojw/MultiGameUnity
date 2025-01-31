using Auth;
using Grpc.Core;
using System.Threading.Tasks;
using UnityEngine;

public class AuthServiceManager : MonoBehaviour
{
    private AuthService.AuthServiceClient client;

    public AuthServiceManager(GrpcClientManager grpcClientManager)
    {
        client = grpcClientManager.GetAuthClient();
    }

    public async Task<SignInResponse> SignInAsync(string userId, string userPassword)
    {
        var request = new SignInRequest
        {
            UserId = userId,
            UserPassword = userPassword
        };

        return await client.SignInAsync(request);
    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var request = new RefreshTokenRequest();

        return await client.RefreshTokenAsync(request);
    }

    public async Task<SignOutResponse> SignOutAsync(string accessToken)
    {
        var request = new SignOutRequest();

        var header = new Metadata
        {
            {"Authorization", $"Bearer {accessToken}"}
        };

        return await client.SignOutAsync(request, header);
    }    
}

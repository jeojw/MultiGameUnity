using Auth;
using Grpc.Core;
using System.Threading.Tasks;
using UnityEngine;

public class AuthServiceManager
{
    private readonly AuthService.AuthServiceClient client;

    private static AuthServiceManager instance;
    public static AuthServiceManager Instance => instance ??= new AuthServiceManager();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AuthServiceManager()
    {
        client = GrpcClientManager.Instance.GetAuthClient();
    }

    public async Task<SignInResponse> SignInAsync(string userId, string userPassword)
    {
        var request = new SignInRequest
        {
            UserId = userId,
            UserPassword = userPassword
        };

        try
        {
            return await client.SignInAsync(request);
        }
        catch (RpcException e)
        {
            Debug.LogError($"SignIn Error: {e.Status.Detail}");
            throw;
        }
    }
}

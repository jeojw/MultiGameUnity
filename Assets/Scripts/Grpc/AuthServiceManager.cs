using Auth;
using Grpc.Net.Client;
using System.Threading.Tasks;
using UnityEngine;

public class AuthServiceManager : MonoBehaviour
{
    private AuthService.AuthServiceClient client;

    private static AuthServiceManager instance;
    public static AuthServiceManager Instance => instance ??= new AuthServiceManager();

    // Start는 MonoBehaviour에서 초기화를 위한 메서드
    void Start()
    {
        // 클라이언트 초기화
        client = GrpcClientManager.Instance.GetAuthClient();
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
}

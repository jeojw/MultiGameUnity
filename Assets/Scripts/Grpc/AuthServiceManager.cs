using Auth;
using System.Threading.Tasks;
using UnityEngine;

public class AuthServiceManager : MonoBehaviour
{
    private static readonly object lockObj = new object();

    private AuthService.AuthServiceClient client;
    private static AuthServiceManager instance;
    public static AuthServiceManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject(nameof(AuthServiceManager));
                instance = obj.AddComponent<AuthServiceManager>();
                DontDestroyOnLoad(obj); // Ensure the instance persists across scenes
            }
            return instance;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateInstanceOnGameStart()
    {
        // Explicitly ensure the singleton instance is created at game start
        _ = Instance;
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Prevent duplicate instances
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
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

        return await client.SignInAsync(request);
    }
}

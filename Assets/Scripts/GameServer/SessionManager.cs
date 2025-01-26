using Fusion;
using System.Threading.Tasks;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    private static SessionManager instance;
    public static SessionManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject(nameof(SessionManager));
                instance = obj.AddComponent<SessionManager>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    private PlayerRef playerRef;
    private string accessToken;

    private async void Start()
    {
        var authManager = AuthManager.Instance;
        playerRef = authManager.CurrentPlayerRef;
        accessToken = await authManager.GetAccessToken(playerRef);
    }

    public async Task TransitionToLobby()
    {
        await LobbyManager.Instance.StartLobbyAsync(accessToken);
    }
}

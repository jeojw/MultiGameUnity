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

    public async Task TransitionToLobby()
    {
        await LobbyManager.Instance.StartLobby();
    }
}

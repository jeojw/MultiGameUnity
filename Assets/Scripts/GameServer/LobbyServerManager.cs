using Fusion;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LobbyServerManager : MonoBehaviour
{
    private static LobbyServerManager instance;
    public static LobbyServerManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("LobbyServerManager");
                instance = obj.AddComponent<LobbyServerManager>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    private NetworkRunner lobbyRunner;
    private LobbyCallbacks networkCallbacks;
    private RoomServiceManager roomServiceManager;
    private FusionServerServiceManager fusionServerServiceManager;

    private Texture2D profileImage;
    private string userNickname;
    private string serverAccessToken;

    private string defaultLobbyName = "Lobby";
    private int maxPlayersPerLobby = 200;

    private void Awake()
    {
        roomServiceManager = ServiceInitializer.Instance.GetRoomServiceManager();
        fusionServerServiceManager = ServiceInitializer.Instance.GetFusionServerServiceManager();

        lobbyRunner = gameObject.AddComponent<NetworkRunner>();
        lobbyRunner.ProvideInput = true;

        networkCallbacks = new LobbyCallbacks();
        lobbyRunner.AddCallbacks(networkCallbacks);
    }

    public async Task StartLobbyAsync(string userId, string accessToken)
    {
        StartGameArgs startGameArgs = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = defaultLobbyName,
            SessionProperties = new Dictionary<string, SessionProperty>
            {
                { "maxPlayers", maxPlayersPerLobby }
            },
            SceneManager = lobbyRunner.gameObject.AddComponent<NetworkSceneManagerDefault>()
        };

        var result = await lobbyRunner.StartGame(startGameArgs);

        if (result.Ok)
        {
            Debug.Log($"기본 로비 '{defaultLobbyName}'가 생성되었습니다.");
            var authManager = AuthManager.Instance;
            await authManager.SetAccessToken(userId, accessToken);
        }
        else
        {
            Debug.LogError($"기본 로비 생성 실패: {result.ErrorMessage}");
        }

        await Task.Yield();
    }
    public async Task ShutDownLobbyAsync()
    {
        await lobbyRunner.Shutdown();

        await Task.Yield();
    }

    public async Task UpdateRoomList()
    {
        //foreach (var session in activeSessions)
        //{
        //    var roomInfo = new RoomInfo
        //    {
        //        roomId = session.Name,
        //        roomName = session.Properties.,
        //        maxPlayers = session.Properties.GetInt("maxPlayers", 0),
        //        currentPlayers = session.PlayerCount,
        //        hasPassword = session.Properties,
        //        roomPassword = session.Properties,
        //        roomManager = session.Properties,
        //        roomStatus = session.Properties
        //    };

        //    if (roomCache.ContainsKey(session.Name))
        //        roomCache[session.Name] = roomInfo;
        //    else
        //        roomCache.Add(session.Name, roomInfo);
        //}

        //await Task.Yield();
    }
}

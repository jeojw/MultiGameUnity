using Fusion;
using Fusion.Sockets;
using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static Unity.Collections.Unicode;

public class LobbyServerManager : MonoBehaviour
{
    private static LobbyServerManager instance;
    public static LobbyServerManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject(nameof(LobbyServerManager));
                instance = obj.AddComponent<LobbyServerManager>();
                DontDestroyOnLoad(obj); // Ensure the instance persists across scenes
            }
            return instance;
        }
    }

    private NetworkRunner lobbyRunner;
    private LobbyCallbacks networkCallbacks;

    private Texture2D profileImage;
    private string userNickname;
    private string accessToken;

    private string defaultLobbyName = "Lobby"; // 기본 로비 이름
    private int maxPlayersPerLobby = 200;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateInstanceOnGameStart()
    {
        // Explicitly ensure the singleton instance is created at game start
        _ = Instance;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        lobbyRunner = gameObject.AddComponent<NetworkRunner>();
        lobbyRunner.ProvideInput = true;

        networkCallbacks = gameObject.AddComponent<LobbyCallbacks>();
        lobbyRunner.AddCallbacks(networkCallbacks);
    }
    
    public async Task StartLobbyAsync(string accessToken)
    {
        StartGameArgs startGameArgs = new StartGameArgs
        {
            GameMode = GameMode.Host,
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
            await authManager.SetAccessToken(lobbyRunner.LocalPlayer, accessToken);
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
}

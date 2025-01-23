using Fusion;
using Fusion.Sockets;
using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using UnityEngine;
using static Unity.Collections.Unicode;

public class LobbyManager : MonoBehaviour, INetworkRunnerCallbacks
{
    private static LobbyManager instance;
    public static LobbyManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject(nameof(LobbyManager));
                instance = obj.AddComponent<LobbyManager>();
                DontDestroyOnLoad(obj); // Ensure the instance persists across scenes
            }
            return instance;
        }
    }

    private NetworkRunner lobbyRunner;

    private NetworkEvents networkEvents;
    private NetworkObject networkObject;

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
    }

    public async Task StartLobby()
    {
        lobbyRunner = gameObject.AddComponent<NetworkRunner>();
        lobbyRunner.ProvideInput = false;
        networkEvents = gameObject.AddComponent<NetworkEvents>();

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
        }
        else
        {
            Debug.LogError($"기본 로비 생성 실패: {result.ErrorMessage}");
        }

        await Task.Yield();
    }

    public async Task AutoJoinDefaultLobbyAsync(string jwtToken, string nickname, ByteString profileImage, PlayerRef playerRef)
    {
        if (lobbyRunner == null)
        {
            Debug.Log("서버가 초기화되지 않았습니다.");
        }

        OnPlayerJoined(lobbyRunner, playerRef);

        await Task.Yield();
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        GameObject playerPrefeb = Resources.Load<GameObject>("Futuristic_soldier");
        runner.Spawn(playerPrefeb, Vector3.zero, Quaternion.identity, player);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        var playerObject = runner.GetPlayerObject(player);
        if (playerObject != null)
        {
            runner.Despawn(playerObject);
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }
}

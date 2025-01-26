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
    
    public async Task StartLobbyAsync(string accessToken)
    {
        lobbyRunner = gameObject.AddComponent<NetworkRunner>();
        lobbyRunner.ProvideInput = true;
        networkEvents = gameObject.AddComponent<NetworkEvents>();

        lobbyRunner.AddCallbacks(this);

        StartGameArgs startGameArgs = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = defaultLobbyName,
            SessionProperties = new Dictionary<string, SessionProperty>
            {
                { "maxPlayers", maxPlayersPerLobby }
            },
            ConnectionToken = System.Text.Encoding.UTF8.GetBytes(accessToken),
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

    public async Task AutoJoinDefaultLobbyAsync(PlayerRef playerRef)
    {
        if (lobbyRunner == null)
        {
            Debug.Log("서버가 초기화되지 않았습니다.");
        }

        OnPlayerJoined(lobbyRunner, playerRef);

        await Task.Yield();
    }

    public async Task LeftLobbyAsync(PlayerRef player)
    {
        if (lobbyRunner == null)
        {
            Debug.Log("서버가 존재하지 않습니다.");
        }

        OnPlayerLeft(lobbyRunner, player);

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
        GameObject playerPrefab = Resources.Load<GameObject>("LobbyPrefab");
        var playerObject = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);

        if (playerObject != null)
        {
            Debug.Log("Player spawned successfully.");

            if (playerObject.TryGetComponent<NetworkObject>(out var networkObject))
            {
                networkObject.AssignInputAuthority(player);
                Debug.Log("Input authority assigned.");
            }
            else
            {
                Debug.LogError("NetworkObject component is missing.");
            }
        }
        else
        {
            Debug.LogError("Failed to spawn player object.");
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        var playerObject = runner.GetPlayerObject(player);
        var networkObject = playerObject.GetComponent<NetworkObject>();

        if (networkObject != null)
        {
            networkObject.RemoveInputAuthority();
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

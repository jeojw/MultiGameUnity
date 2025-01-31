using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class GameServerManager : MonoBehaviour
{
    private static GameServerManager instance;
    public static GameServerManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject(nameof(GameServerManager));
                instance = obj.AddComponent<GameServerManager>();
                DontDestroyOnLoad(obj); // Ensure the instance persists across scenes
            }
            return instance;
        }
    }

    private NetworkRunner gameRunner;
    private GameCallbacks networkCallbacks;

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

        gameRunner = gameObject.AddComponent<NetworkRunner>();
        gameRunner.ProvideInput = true;

        networkCallbacks = gameObject.AddComponent<GameCallbacks>();
        gameRunner.AddCallbacks(networkCallbacks);
    }

    //private async void Start()
    //{
    //    StartGameArgs startGameArgs = new StartGameArgs
    //    {
    //        GameMode = GameMode.Host,
    //        SessionName = SystemInfo.deviceUniqueIdentifier,
    //        SessionProperties = new Dictionary<string, SessionProperty>
    //        {
    //            { "maxPlayers", maxPlayersPerLobby }
    //        },
    //        SceneManager = gameRunner.gameObject.AddComponent<NetworkSceneManagerDefault>()
    //    };

    //    var result = await gameRunner.StartGame(startGameArgs);
    //}
}

using Fusion;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RoomServerManager : MonoBehaviour
{
    private static RoomServerManager instance;
    public static RoomServerManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject(nameof(RoomServerManager));
                instance = obj.AddComponent<RoomServerManager>();
                DontDestroyOnLoad(obj); // Ensure the instance persists across scenes
            }
            return instance;
        }
    }

    private NetworkRunner  roomRunner;
    private LobbyCallbacks networkCallbacks;
    private RoomServiceManager roomServiceManager;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        networkCallbacks = gameObject.AddComponent<LobbyCallbacks>();
        roomServiceManager = ServiceInitializer.Instance.GetRoomServiceManager();
    }

    public async Task JoinRoomAsync(string accessToken, string roomId)
    {
        roomRunner = new GameObject("RoomRunner").AddComponent<NetworkRunner>();
        roomRunner.ProvideInput = true;
        roomRunner.AddCallbacks(networkCallbacks);

        var response = await roomServiceManager.JoinRoomAsync(accessToken, roomId);

        if (response != null)
        {
            StartGameArgs startGameArgs = new StartGameArgs
            {
                GameMode = GameMode.Shared,
                SessionName = roomId,
                SceneManager = roomRunner.gameObject.AddComponent<NetworkSceneManagerDefault>()
            };

            var result = await roomRunner.StartGame(startGameArgs);

            if (result.Ok)
            {
                Debug.Log($"방 입장에 성공했습니다.");
            }
            else
            {
                Debug.LogError($"방 입장 실패: {result.ErrorMessage}");
            }
        }
    }

    public async Task JoinRoomWithPasswordAsync(string accessToken, string roomId, string roomPassword)
    {
        roomRunner = new GameObject("RoomRunner").AddComponent<NetworkRunner>();
        roomRunner.ProvideInput = true;
        roomRunner.AddCallbacks(networkCallbacks);

        var response = await roomServiceManager.JoinRoomWithPasswordAsync(accessToken, roomId, roomPassword);

        if (response != null)
        {
            StartGameArgs startGameArgs = new StartGameArgs
            {
                GameMode = GameMode.Shared,
                SessionName = roomId,
                SceneManager = roomRunner.gameObject.AddComponent<NetworkSceneManagerDefault>()
            };

            var result = await roomRunner.StartGame(startGameArgs);

            if (result.Ok)
            {
                Debug.Log($"방 입장에 성공했습니다.");
            }
            else
            {
                Debug.LogError($"방 입장 실패: {result.ErrorMessage}");
            }
        }
    }

    public async Task ExitRoomAsync(string accessToken, string roomId)
    {
        var response = await roomServiceManager.ExitRoomAsync(accessToken, roomId);

        if (response != null)
        {
            if (roomRunner != null)
            {
                await roomRunner.Shutdown();
                Destroy(roomRunner.gameObject);
                roomRunner = null;
                Debug.Log("방에서 퇴장했습니다.");
            }
        }
    }
}

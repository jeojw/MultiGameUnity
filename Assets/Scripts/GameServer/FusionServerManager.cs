using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FusionServerManager : MonoBehaviour
{
    private static FusionServerManager instance;

    // Public static property for accessing the singleton instance
    public static FusionServerManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject(nameof(FusionServerManager));
                instance = obj.AddComponent<FusionServerManager>();
                DontDestroyOnLoad(obj); // Ensure the instance persists across scenes
            }
            return instance;
        }
    }

    private string serverAccessToken;

    private RoomServiceManager roomServiceManager;
    private FusionServerServiceManager fusionServerServiceManager;

    private async void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        roomServiceManager = ServiceInitializer.Instance.GetRoomServiceManager();
        fusionServerServiceManager = ServiceInitializer.Instance.GetFusionServerServiceManager();

        await InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        if (instance != null)
        {
            var response = await fusionServerServiceManager.GenerateTokenAsync();

            if (response.AccessToken != null)
            {
                serverAccessToken = response.AccessToken;
            }
        }
    }

    public async Task<List<RoomInfo>> GetRoomList()
    {
        var roomList = await roomServiceManager.GetRoomInfoListAsync(serverAccessToken);

        List<RoomInfo> roomCache = new List<RoomInfo>();

        if (roomList.Rooms != null)
        {
            foreach (var roomInfo in roomList.Rooms)
            {
                var _ = new RoomInfo
                {
                    roomId = roomInfo.RoomId,
                    roomName = roomInfo.RoomTitle,
                    maxPlayers = roomInfo.MaxPlayers,
                    currentPlayers = roomInfo.CurPlayers,
                    hasPassword = roomInfo.IsExistPassword,
                    roomPassword = roomInfo.RoomPassword,
                    roomManager = roomInfo.RoomManager,
                    roomStatus = roomInfo.RoomStatus
                };

                roomCache.Add(_);
            }
        }

        return roomCache;
    }

    public async Task<bool> ChangeRoomStatusAsync(string roomId, int status)
    {
        var response = await roomServiceManager.ChangeRoomStatusAsync(serverAccessToken, status, roomId);

        return response.Message != null;
    }

    public async Task<bool> DeleteRoomAsync(string roomId)
    {
        var response = await roomServiceManager.DeleteRoomAsync(serverAccessToken, roomId);

        return response.Message != null;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

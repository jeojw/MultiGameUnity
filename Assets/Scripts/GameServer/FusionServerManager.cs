using Fusion;
using Grpc.Core;
using Room;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class FusionServerManager : MonoBehaviour
{
    private static FusionServerManager instance;
    public static FusionServerManager Instance => instance ??= new FusionServerManager();

    private string serverAccessToken;

    private RoomServiceManager roomServiceManager;
    private FusionServerServiceManager fusionServerServiceManager;

    private void Awake()
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
    }

    private void Start()
    {
        _ = InitializeAsync();
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

    public async Task<Dictionary<string, RoomInfo>> GetRoomList()
    {
        var roomList = await roomServiceManager.GetRoomInfoListAsync(serverAccessToken);

        Dictionary<string, RoomInfo> roomCache = new Dictionary<string, RoomInfo>();

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

                roomCache.Add(roomInfo.RoomId, _);
            }
        }

        return roomCache;
    }

    public async Task<ChangeRoomStatusResponse> ChangeRoomStatusAsync(string roomId, int status)
    {
        return await roomServiceManager.ChangeRoomStatusAsync(serverAccessToken, status, roomId);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

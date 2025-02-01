using Grpc.Core;
using Room;
using System.Threading.Tasks;
using UnityEngine;

public class RoomServiceManager : MonoBehaviour
{
    private RoomService.RoomServiceClient client;
    public RoomServiceManager(GrpcClientManager grpcClientManager)
    {
        client = grpcClientManager.GetRoomClient();
    }
    public async Task<GetRoomInfoResponse> GetRoomInfoAsync(string token, string roomId)
    {
        var request = new GetRoomInfoRequest
        {
            RoomId = roomId
        };

        var header = new Metadata
        {
            {"Authorization", $"Bearer {token}"}
        };

        return await client.GetRoomInfoAsync(request, header);
    }

    public async Task<GetRoomInfoListResponse> GetRoomInfoListAsync(string token)
    {
        var request = new GetRoomInfoListRequest();

        var header = new Metadata
        {
            {"Authorization", $"Bearer {token}"}
        };

        return await client.GetRoomInfoListAsync(request, header);
    }

    public async Task<CreateRoomResponse> CreateRoomAsync(
        string token,
        string roomTitle,
        int maxPlayers,
        bool isExistPassword,
        string roomPassword,
        string roomManager
    )
    {
        var request = new CreateRoomRequest
        {
            RoomTitle = roomTitle,
            MaxPlayer = maxPlayers,
            IsExistPassword = isExistPassword,
            RoomPassword = roomPassword,
            RoomManager = roomManager
        };

        var header = new Metadata
        {
            {"Authorization", $"Bearer {token}"}
        };

        return await client.CreateRoomAsync(request, header);
    }

    public async Task<JoinRoomResponse> JoinRoomAsync(string token, string roomId)
    {
        var request = new JoinRoomRequest
        {
            RoomId = roomId
        };

        var header = new Metadata
        {
            {"Authorization", $"Bearer {token}"}
        };

        return await client.JoinRoomAsync(request, header);
    }

    public async Task<JoinRoomWithPasswordResponse> JoinRoomWithPasswordAsync(string token, string roomId, string password)
    {
        var request = new JoinRoomWithPasswordRequest
        {
            RoomId = roomId,
            RoomPassword = password
        };

        var header = new Metadata
        {
            {"Authorization", $"Bearer {token}"}
        };

        return await client.JoinRoomWithPasswordAsync(request, header);
    }

    public async Task<ExitRoomResponse> ExitRoomAsync(string token, string roomId)
    {
        var request = new ExitRoomRequest
        {
            RoomId = roomId
        };

        var header = new Metadata
        {
            {"Authorization", $"Bearer {token}"}
        };

        return await client.ExitRoomAsync(request, header);
    }

    public async Task<ChangeRoomInfoResponse> ChangRoomInfoAsync(
        string token,
        string roomId,
        string roomTitle,
        int maxPlayers,
        bool isExistPassword,
        string roomPassword
    )
    {
        var request = new ChangeRoomInfoRequest
        {
            RoomId = roomId,
            RoomTitle = roomTitle,
            MaxPlayer = maxPlayers,
            IsExistPassword = isExistPassword,
            RoomPassword = roomPassword,
        };

        var header = new Metadata
        {
            {"Authorization", $"Bearer {token}"}
        };

        return await client.ChangeRoomInfoAsync(request, header);
    }

    public async Task<ExileUserResponse> ExileUserAsync(string token, string roomId, string userAccessToken)
    {
        var request = new ExileUserRequest
        {
            RoomId = roomId,
            UserAccessToken = userAccessToken
        };

        var header = new Metadata
        {
            {"Authorization", $"Bearer {token}"}
        };

        return await client.ExileUserAsync(request, header);
    }

    public async Task<ChangeRoomStatusResponse> ChangeRoomStatusAsync(string token, int status, string roomId)
    {
        var request = new ChangeRoomStatusRequest
        {
            RoomId = roomId,
            ChangeRoomStatus = status
        };

        var header = new Metadata
        {
            {"Authorization", $"Bearer {token}"}
        };

        return await client.ChangeRoomStatusAsync(request, header);
    }

    public async Task<DeleteRoomResponse> DeleteRoomAsync(string token, string roomId)
    {
        var request = new DeleteRoomRequest
        {
            RoomId = roomId
        };

        var header = new Metadata
        {
            {"Authorization", $"Bearer {token}"}
        };

        return await client.DeleteRoomAsync(request, header);
    }

    public async Task<ChangeRoomManagerResponse> ChangeRoomManagerAsync(string token, string roomId, string currentRoomManager, string newRoomManager)
    {
        var request = new ChangeRoomManagerRequest
        {
            RoomId = roomId,
            CurrentRoomManager = currentRoomManager,
            NewRoomManager = newRoomManager
        };

        var header = new Metadata
        {
            {"Authorization", $"Bearer {token}"}
        };

        return await client.ChangeRoomManagerAsync(request, header);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

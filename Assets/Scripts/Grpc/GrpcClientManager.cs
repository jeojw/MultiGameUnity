using System;
using System.Threading.Tasks;
using UnityEngine;
using Auth;
using Member;
using FusionServer;
using Grpc.Net.Client;
using System.Net.Http;
using Grpc.Net.Client.Web;
using Room;

public class GrpcClientManager
{
    private GrpcChannel channel;
    private bool isInitialized = false;

    public GrpcClientManager() 
    {
        InitializeChannel();
    }

    private void InitializeChannel()
    {
        try
        {
            // gRPC 채널 생성
            channel = GrpcChannel.ForAddress("http://127.0.0.1:7070", new GrpcChannelOptions
            {
                HttpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler())
            });
            Debug.Log("gRPC Channel Initialized!");
            isInitialized = true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize gRPC channel: {ex.Message}");
        }
    }

    private T CreateGrpcClient<T>(Func<GrpcChannel, T> clientFactory) where T : class
    {
        if (!isInitialized || channel == null)
        {
            Debug.LogError("gRPC channel is not initialized. Cannot create client.");
            return null;
        }

        return clientFactory(channel);
    }
    public AuthService.AuthServiceClient GetAuthClient()
    {
        return CreateGrpcClient(channel => new AuthService.AuthServiceClient(channel));
    }

    public MemberService.MemberServiceClient GetMemberClient()
    {
        return CreateGrpcClient(channel => new MemberService.MemberServiceClient(channel));
    }

    public RoomService.RoomServiceClient GetRoomClient()
    {
        return CreateGrpcClient(channel => new RoomService.RoomServiceClient(channel));
    }

    public FusionServerService.FusionServerServiceClient GetFusionServiceClient()
    {
        return CreateGrpcClient(channel => new FusionServerService.FusionServerServiceClient(channel));
    }

    public async Task DisposeAsync()
    {
        if (channel != null)
        {
            try
            {
                await channel.ShutdownAsync();
                Debug.Log("gRPC channel shutdown.");
            }
            catch (Exception ex)
            {
                Debug.LogError("Error shutting down gRPC channel: " + ex.Message);
            }
            finally
            {
                channel = null;
            }
        }
    }
}

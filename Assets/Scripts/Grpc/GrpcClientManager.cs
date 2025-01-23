using System;
using System.Threading.Tasks;
using UnityEngine;
using Auth;
using Member;
using Grpc.Net.Client;
using System.Net.Http;
using Grpc.Net.Client.Web;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Net;
using Microsoft.AspNetCore;

public class GrpcClientManager : MonoBehaviour
{
    private static readonly object lockObj = new object();

    private GrpcChannel channel;
    private bool isInitialized = false;

    private static GrpcClientManager instance;
    public static GrpcClientManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject(nameof(GrpcClientManager));
                instance = obj.AddComponent<GrpcClientManager>();
                DontDestroyOnLoad(obj); // Ensure the instance persists across scenes
            }
            return instance;
        }
    }
    private GrpcClientManager() { }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateInstanceOnGameStart()
    {
        // Explicitly ensure the singleton instance is created at game start
        _ = Instance;
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Prevent duplicate instances
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
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

    private void OnDestroy()
    {
        Task.Run(async () => await DisposeAsync());
    }

    private async Task DisposeAsync()
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

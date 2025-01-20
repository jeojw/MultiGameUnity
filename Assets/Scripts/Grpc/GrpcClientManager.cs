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

public class GrpcClientManager : MonoBehaviour
{
    private GrpcChannel channel;
    private bool isInitialized = false;
    public static GrpcClientManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeChannel();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeChannel()
    {
        try
        {
            var proxyAddress = new Uri("http://127.0.0.1:7070");

            var httpHandler = new HttpClientHandler()
            {
                Proxy = new System.Net.WebProxy(proxyAddress),
                UseProxy = true
            };
            channel = GrpcChannel.ForAddress("http://127.0.0.1:7070", new GrpcChannelOptions
            {
                HttpHandler = httpHandler
            });
            isInitialized = true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize gRPC channel: {ex.Message}");
        }
    }

    public AuthService.AuthServiceClient GetAuthClient()
    {
        if (!isInitialized || channel == null)
        {
            Debug.LogError("gRPC channel is null. Cannot create AuthServiceClient.");
            return null;
        }

        return new AuthService.AuthServiceClient(channel);
    }

    public MemberService.MemberServiceClient GetMemberClient()
    {
        if (!isInitialized || channel == null)
        {
            Debug.LogError("gRPC channel is null. Cannot create MemberServiceClient.");
            return null;
        }

        return new MemberService.MemberServiceClient(channel);
    }

    private async void OnDestroy()
    {
        await DisposeAsync();
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

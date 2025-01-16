using Grpc.Core;
using UnityEngine;
using Auth;
using Paket;

public class GrpcClient : MonoBehaviour
{
    private Channel channel;
    private readonly AuthService.AuthServiceClient

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        channel = new Channel("localhost:50051", ChannelCredentials.Insecure);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

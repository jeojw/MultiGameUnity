using System;
using System.Threading.Tasks;
using Grpc.Core;
using UnityEngine;
using Auth;
using Member;

public class GrpcClientManager : MonoBehaviour
{
    private readonly Channel channel;

    private static GrpcClientManager instance;
    public static GrpcClientManager Instance => instance ??= new GrpcClientManager();

    private GrpcClientManager()
    {
        channel = new Channel("localhost:50052", ChannelCredentials.Insecure);
    }
    
    public AuthService.AuthServiceClient GetAuthClient()
    {
        return new AuthService.AuthServiceClient(channel);
    }

    public MemberService.MemberServiceClient GetMemberClient()
    {
        return new MemberService.MemberServiceClient(channel);
    }

    public void Dispose()
    {
        channel.ShutdownAsync().Wait();
    }
}

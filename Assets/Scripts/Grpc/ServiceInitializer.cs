using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class ServiceInitializer : MonoBehaviour
{
    private static ServiceInitializer instance;
    private bool isDisposed = false;

    private GrpcClientManager grpcClientManager;
    private RoomServiceManager roomServiceManager;
    private AuthServiceManager authServiceManager;
    private MemberServiceManager memberServiceManager;
    private FusionServerServiceManager fusionServerServiceManager;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 유지됨
            InitializeServices();
            
        }
        else
        {
            Destroy(gameObject); // Singleton 패턴을 사용하여 중복 객체 방지
        }
    }

    private void InitializeServices()
    {
        grpcClientManager = new GrpcClientManager();
        authServiceManager = new AuthServiceManager(grpcClientManager);
        memberServiceManager = new MemberServiceManager(grpcClientManager);
        roomServiceManager = new RoomServiceManager(grpcClientManager);
        fusionServerServiceManager = new FusionServerServiceManager(grpcClientManager);
    }

    public static ServiceInitializer Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("ServiceInitializer");
                instance = obj.AddComponent<ServiceInitializer>();
                instance.InitializeServices();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    public AuthServiceManager GetAuthServiceManager() => authServiceManager;
    public MemberServiceManager GetMemberServiceManager() => memberServiceManager;
    public RoomServiceManager GetRoomServiceManager() => roomServiceManager;
    public FusionServerServiceManager GetFusionServerServiceManager() => fusionServerServiceManager;

    public async Task DisposeAsync()
    {
        if (isDisposed) return;

        try
        {
            //(authServiceManager as IDisposable)?.Dispose();
            //(memberServiceManager as IDisposable)?.Dispose();
            //(roomServiceManager as IDisposable)?.Dispose();
            //(fusionServerServiceManager as IDisposable)?.Dispose();

            await grpcClientManager.DisposeAsync();

            Debug.Log("ServiceInitializer: All services and gRPC channel disposed.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during ServiceInitializer disposal: {ex.Message}");
        }
        finally
        {
            isDisposed = true;
        }
    }

    public void Dispose()
    {
        DisposeAsync().Wait();
    }
}

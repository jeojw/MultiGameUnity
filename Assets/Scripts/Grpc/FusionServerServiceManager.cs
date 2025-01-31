using FusionServer;
using System.Threading.Tasks;
using UnityEngine;

public class FusionServerServiceManager : MonoBehaviour
{
    private FusionServerService.FusionServerServiceClient client;
    public FusionServerServiceManager(GrpcClientManager grpcClientManager)
    {
        client = grpcClientManager.GetFusionServiceClient();
    }

    public async Task<GenerateTokenResponse> GenerateTokenAsync()
    {
        var request = new GenerateTokenRequest();

        return await client.GenerateTokenAsync(request);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

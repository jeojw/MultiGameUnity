using UnityEngine;
using Fusion;
using Fusion.Sockets;

public class MatchMakingManager : MonoBehaviour
{
    private NetworkRunner networkRunner;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartMatchmaking();
    }

    async void StartMatchmaking()
    {
        networkRunner = gameObject.AddComponent<NetworkRunner>();
        networkRunner.ProvideInput = true;

        var result = await networkRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "QuickMatchRoom",
            SceneManager = networkRunner.GetComponent<NetworkSceneManagerDefault>()
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

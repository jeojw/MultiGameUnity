using Fusion.Sockets;
using Fusion;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameCallbacks : MonoBehaviour, INetworkRunnerCallbacks
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        GameObject playerPrefab = Resources.Load<GameObject>("Futuristic_soldier");

        if (playerPrefab == null)
        {
            Debug.LogError("LobbyPrefab not found in Resources folder.");
            return;
        }

        var playerObject = runner.Spawn(playerPrefab, new Vector3(-22.47882f, 0.061f, -65.91701f), Quaternion.identity, player);

        if (playerObject != null)
        {
            Debug.Log("Player spawned successfully.");

            DontDestroyOnLoad(playerObject);
            runner.SetPlayerObject(player, playerObject);

            if (playerObject.TryGetComponent<NetworkObject>(out var networkObject))
            {
                networkObject.AssignInputAuthority(player);
                Debug.Log("Input authority assigned to player: " + player);
            }
            else
            {
                Debug.LogError("NetworkObject component is missing on LobbyPrefab.");
            }
        }
        else
        {
            Debug.LogError("Failed to spawn player object. Ensure the prefab is properly registered with the NetworkRunner.");
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        var playerObject = runner.GetPlayerObject(player);

        playerObject.RemoveInputAuthority();

        Destroy(playerObject);

        runner.Despawn(playerObject);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {

    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }
}

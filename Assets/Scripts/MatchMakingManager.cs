using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System.Collections;

public class MatchMakingManager : MonoBehaviour
{
    private static readonly List<NetworkRunner> playerQueue = new List<NetworkRunner>();
    public void StartMatchmaking(NetworkRunner runner)
    {
        if (runner.IsServer)
        {
            playerQueue.Add(runner);

            if (playerQueue.Count >= 2)
            {
                StartGame();
            }
        }
    }

    private void StartGame()
    {
        List<NetworkRunner> playersToStartGame = new List<NetworkRunner>(playerQueue);
        playerQueue.Clear();

        foreach (NetworkRunner player in playersToStartGame)
        {
            StartCoroutine(LoadGameScene(player));
        }
    }

    private IEnumerator LoadGameScene(NetworkRunner player)
    {
        yield return new WaitForSeconds(1f);
        player.GetComponent<NetworkRunner>().LoadScene("MainScene");
    }
}

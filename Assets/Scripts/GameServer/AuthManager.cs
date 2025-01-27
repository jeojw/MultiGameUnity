using Fusion;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    private static AuthManager instance;
    public static AuthManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject(nameof(AuthManager));
                instance = obj.AddComponent<AuthManager>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    private PlayerRef currentPlayerRef;
    public PlayerRef CurrentPlayerRef => currentPlayerRef;

    private readonly Dictionary<PlayerRef, string> playerTokens = new Dictionary<PlayerRef, string>();

    public async Task SetAccessToken(PlayerRef playerRef, string token)
    {
        if (playerTokens.ContainsKey(playerRef))
        {
            playerTokens[playerRef] = token;
        }
        else
        {
            playerTokens.Add(playerRef, token);
        }

        currentPlayerRef = playerRef;
        await Task.Yield(); // 메인 스레드에서 실행
    }

    public async Task<string> GetAccessToken(PlayerRef playerRef)
    {
        await Task.Yield();

        if (playerTokens.TryGetValue(playerRef, out var token))
        {
            return token;
        }
        return null;
    }

    public async Task RemoveAccessToken(PlayerRef playerRef)
    {
        playerTokens.Remove(playerRef);
        if (currentPlayerRef == playerRef)
        {
            currentPlayerRef = PlayerRef.None;
        }

        await Task.Yield();
    }

    public async Task<bool> HasAccessToken(PlayerRef playerRef)
    {
        await Task.Yield();

        return playerTokens.ContainsKey(playerRef);
    }
}
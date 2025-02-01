using Fusion;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AuthManager
{
    private static AuthManager instance;
    public static AuthManager Instance => instance ??= new AuthManager();

    private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
    private string currentUserId;
    public string CurrentUserId => currentUserId;

    private readonly Dictionary<string, string> playerTokens = new Dictionary<string, string>();

    public async Task SetAccessToken(string userId, string token)
    {
        await semaphore.WaitAsync();
        try
        {
            playerTokens[userId] = token;
            currentUserId = userId;
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async Task<string> GetAccessToken()
    {
        await semaphore.WaitAsync();
        try
        {
            if (currentUserId != null && playerTokens.TryGetValue(currentUserId, out var token))
            {
                return token;
            }
            return null;
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async Task RemoveAccessToken()
    {
        await semaphore.WaitAsync();
        try
        {
            if (currentUserId != null)
            {
                playerTokens.Remove(currentUserId);
                currentUserId = null;
            }
        }
        finally
        {
            semaphore.Release();
        }
    }

}
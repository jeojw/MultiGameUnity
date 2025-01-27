using Fusion;
using Google.Protobuf;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyScript : MonoBehaviour
{
    [SerializeField]
    private Image profileImage;
    [SerializeField]
    private TextMeshProUGUI nickName;

    private PlayerRef playerRef;
    private string accessToken;
    private AuthManager authManager;
    private LobbyManager lobbyManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        authManager = AuthManager.Instance;
        playerRef = authManager.CurrentPlayerRef;
        accessToken = await authManager.GetAccessToken(playerRef);

        var memberServiceManager = MemberServiceManager.Instance;
        var response = await memberServiceManager.UserInfoAsync(accessToken);
        lobbyManager = LobbyManager.Instance;

        ByteString byteStringImage = response.ProfileData;
        Byte[] imageBytes = byteStringImage.ToByteArray();
        Texture2D defaultProfileImage = new Texture2D(2, 2);
        defaultProfileImage.LoadImage(imageBytes);
        Sprite sprite = Sprite.Create(
            defaultProfileImage,
            new Rect(0, 0, defaultProfileImage.width, defaultProfileImage.height),
            new Vector2(0.5f, 0.5f) // Pivot point at the center
        );
        profileImage.sprite = sprite;

        nickName.text = response.UserNickname;
    }

    public async void SignOut()
    {
        var authServiceManager = AuthServiceManager.Instance;

        var response = await authServiceManager.SignOutAsync(accessToken);

        if (response != null)
        {
            SceneManager.LoadScene("SigninScene");
            await lobbyManager.ShutDownLobbyAsync();
        }
    }

    public async void GameExit()
    {
        var authServiceManager = AuthServiceManager.Instance;

        var response = await authServiceManager.SignOutAsync(accessToken);

        if (response != null)
        {
            await lobbyManager.ShutDownLobbyAsync();
            Application.Quit();
        }
    }
}

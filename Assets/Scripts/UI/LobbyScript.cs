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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        var authManager = AuthManager.Instance;
        playerRef = authManager.CurrentPlayerRef;
        accessToken = await authManager.GetAccessToken(playerRef);

        var memberServiceManager = MemberServiceManager.Instance;
        var response = await memberServiceManager.UserInfoAsync(accessToken);

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
            try
            {
                var lobbyManager = LobbyManager.Instance;
                await lobbyManager.LeftLobbyAsync(playerRef);

                SceneManager.LoadScene("SigninScene");
            }
            catch (Exception ex)
            {
                Debug.LogError($"씬 로드 실패: {ex.Message}");
            }
        }
    }

    public async void GameExit()
    {
        var authServiceManager = AuthServiceManager.Instance;

        var response = await authServiceManager.SignOutAsync(accessToken);

        if (response != null)
        {
            var lobbyManager = LobbyManager.Instance;
            await lobbyManager.LeftLobbyAsync(playerRef);

            Application.Quit();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

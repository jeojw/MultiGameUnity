using Fusion;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyScript : MonoBehaviour
{
    private GameObject roomPrefab;
    [SerializeField]
    private Image profileImage;
    [SerializeField]
    private TextMeshProUGUI nickName;
    [SerializeField]
    private GameObject createRoomPopup;
    [SerializeField]
    private GameObject content;

    private string accessToken;
    private LobbyServerManager lobbyManager;

    private AuthManager authManager;
    private MemberServiceManager memberServiceManager;
    private AuthServiceManager authServiceManager;
    private FusionServerManager fusionServerManager;

    private List<RoomInfo> roomList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        authManager = AuthManager.Instance;
        accessToken = await authManager.GetAccessToken();

        fusionServerManager = FusionServerManager.Instance;

        authServiceManager = ServiceInitializer.Instance.GetAuthServiceManager();

        memberServiceManager = ServiceInitializer.Instance.GetMemberServiceManager();
        var response = await memberServiceManager.UserInfoAsync(accessToken);

        lobbyManager = LobbyServerManager.Instance;

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

        RefreshRoomList();
    }

    public async void RefreshRoomList()
    {
        roomList = await fusionServerManager.GetRoomList();

        int yValue = 0;
        foreach (var room in roomList)
        {
            roomPrefab = Resources.Load<GameObject>("RobyRoomPrefeb");
            GameObject roomItem = Instantiate(roomPrefab, new Vector3(0,yValue,0), Quaternion.identity);
            roomItem.transform.SetParent(content.transform);
            yValue-=200;
        }
    }

    public async void StartMatchmaking()
    {

    }

    public void CreateRoom()
    {
        createRoomPopup.gameObject.SetActive(true);
    }

    public async void SignOut()
    {
        var response = await authServiceManager.SignOutAsync(accessToken);

        if (response != null)
        {
            SceneManager.LoadScene("SigninScene");
            await authManager.RemoveAccessToken();
            await lobbyManager.ShutDownLobbyAsync();
        }
    }

    public async void GameExit()
    {
        var response = await authServiceManager.SignOutAsync(accessToken);

        if (response != null)
        {
            await authManager.RemoveAccessToken();
            await lobbyManager.ShutDownLobbyAsync();
            Application.Quit();
        }
    }
}

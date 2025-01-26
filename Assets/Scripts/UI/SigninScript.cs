
using Fusion;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SigninScript : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField idField;
    [SerializeField]
    private TMP_InputField pwField;

    private string idValue;
    private string pwValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        idField.Select();
    }

    public void IdInputValueChange(TMP_InputField id)
    {
        idValue = id.text;
    }

    public void PwInputValueChange(TMP_InputField pw)
    {
        pwValue = pw.text;
    }

    public async void Signin()
    {
        if (string.IsNullOrWhiteSpace(idValue) || string.IsNullOrWhiteSpace(pwValue))
        {
            return;
        }

        var authServiceManager = AuthServiceManager.Instance;

        var response = await authServiceManager.SignInAsync(idValue, pwValue);

        if (response != null)
        {
            var lobbyManager = LobbyManager.Instance;
            var authManager = AuthManager.Instance;

            PlayerRef playerRef = new PlayerRef();

            await authManager.SetAccessToken(playerRef, response.AccessToken);

            await lobbyManager.StartLobbyAsync(response.AccessToken);
            await lobbyManager.AutoJoinDefaultLobbyAsync(playerRef);
            try
            {
                SceneManager.LoadScene("RobyScene");
            }
            catch (Exception ex)
            {
                Debug.LogError($"씬 로드 실패: {ex.Message}");
            }
        }
    }
}

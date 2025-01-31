
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

    private AuthServiceManager authServiceManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        idField.Select();
        authServiceManager = ServiceInitializer.Instance.GetAuthServiceManager();
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

        var response = await authServiceManager.SignInAsync(idValue, pwValue);

        if (response != null)
        {
            var lobbyManager = LobbyServerManager.Instance;
            var authManager = AuthManager.Instance;

            await lobbyManager.StartLobbyAsync(response.AccessToken);

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

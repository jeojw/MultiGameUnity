using TMPro;
using UnityEngine;
using System;
using Google.Protobuf;

public class SignupScript : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField idField;
    [SerializeField]
    private TMP_InputField nicknameField;
    [SerializeField]
    private TMP_InputField pwField;

    private bool isIdDuplicate = true;
    private bool isNicknameDuplicate = true;

    private string idValue;
    private string nicknameValue;
    private string pwValue;

    private MemberServiceManager memberServiceManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        idField.Select();
        memberServiceManager = ServiceInitializer.Instance.GetMemberServiceManager();
    }

    public void IdInputValueChange(TMP_InputField id)
    {
        idValue = id.text;
    }

    public void NicknameInputValueChange(TMP_InputField nickname)
    {
        nicknameValue = nickname.text;
    }

    public void PwInputValueChange(TMP_InputField pw)
    {
        pwValue = pw.text;
    }

    public async void CheckIdDuplicate()
    {
        if (string.IsNullOrEmpty(idValue))
        {
            return;
        }

        var response = await memberServiceManager.CheckDuplicateIdAsync(idValue);

        isIdDuplicate = response.IsIdDuplicate;

        Debug.Log(isIdDuplicate);
    }
    public async void CheckNicknameDuplicate()
    {
        if (string.IsNullOrEmpty(nicknameValue))
        {
            return;
        }

        var response = await memberServiceManager.CheckDuplicateNicknameAsync(nicknameValue);

        isNicknameDuplicate = response.IsNicknameDuplicate;

        Debug.Log(isNicknameDuplicate);
    }

    public async void SignUp()
    {
        if (string.IsNullOrEmpty(idValue) || string.IsNullOrEmpty(pwValue) || string.IsNullOrEmpty(nicknameValue))
        {
            return;
        }

        if (isIdDuplicate)
        {
            Debug.Log("ID is duplicated!");
            return;
        }

        if (isNicknameDuplicate)
        {
            Debug.Log("Nickname is duplicated!");
            return;
        }

        Texture2D defaultProfileImage = Resources.Load<Texture2D>("default");

        Byte[] imageBytes = defaultProfileImage.EncodeToJPG();

        ByteString byteStringImage = ByteString.CopyFrom(imageBytes);

        var response = await memberServiceManager.SignUpAsync(idValue, pwValue, nicknameValue, "default", "jpg", byteStringImage);

        Debug.Log(response.Message);
    }
}

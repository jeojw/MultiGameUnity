using TMPro;
using UnityEngine;
using Grpc.Core;
using static UnityEngine.Rendering.DebugUI;
using Member;
using JetBrains.Annotations;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        idField.Select();
        
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

        var response = await MemberServiceManager.Instance.CheckDuplicateIdAsync(idValue);

        isIdDuplicate = response.IsIdDuplicate;
    }
    public async void CheckNicknameDuplicate()
    {
        if (string.IsNullOrEmpty(nicknameValue))
        {
            return;
        }

        var response = await MemberServiceManager.Instance.CheckDuplicateNicknameAsync(nicknameValue);

        isNicknameDuplicate = response.IsNicknameDuplicate;
    }

    public async void SignUp()
    {
        if (string.IsNullOrEmpty(idValue) || string.IsNullOrEmpty(pwValue) || string.IsNullOrEmpty(nicknameValue))
        {
            return;
        }

        if (isIdDuplicate)
        {
            return;
        }

        if (isNicknameDuplicate)
        {
            return;
        }

        //var response = await MemberServiceManager.Instance.SignUpAsync(idValue, pwValue, nicknameValue, "default", "jpg", "default");

        //if (respon)
    }
}

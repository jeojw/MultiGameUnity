using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SignupScript : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField idField;
    [SerializeField]
    private TMP_InputField nicknameField;
    [SerializeField]
    private TMP_InputField pwField;

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

    public void CheckIdDuplicate()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

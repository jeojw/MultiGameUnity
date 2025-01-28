using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomScript : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField roomTitle;
    [SerializeField]
    private TMP_InputField maxPlayer;
    [SerializeField]
    private TMP_InputField password;
    [SerializeField]
    private Toggle passwordCheck;

    private string roomTitleValue;
    private int maxPlayerValue;
    private string passwordValue;
    private bool passwordCheckValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
        maxPlayer.text = 2.ToString();
        passwordCheck.isOn = false;
        password.enabled = false;
    }

    public void OnToggleValueChanged(Toggle passwordCheck)
    {
        passwordCheckValue = passwordCheck.isOn;
    }

    public void RoomTitleValueChanged(TMP_InputField roomTitle)
    {
        roomTitleValue = roomTitle.text;
    }

    public void MaxPalyerValueChange(TMP_InputField maxPlayer)
    {
        maxPlayerValue = int.Parse(maxPlayer.text);
    }

    public void PasswordValueChange(TMP_InputField password)
    {
        passwordValue = password.text;
    }

    public async void Create()
    {

    }

    public void Cancel()
    {
        roomTitle.text = null;
        maxPlayer.text = 2.ToString();
        password.text = null;
        password.enabled = false;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

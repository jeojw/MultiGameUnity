using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SigninScript : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField idField;
    [SerializeField]
    private TMP_InputField pwField;

    private string idValue;
    private string pwValue;

    public string IdValue
    {
        get { return idValue; }
    }

    public string PwValue
    {
        get { return pwValue; }
    }
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

    public void Signin()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

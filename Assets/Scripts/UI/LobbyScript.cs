using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScript : MonoBehaviour
{
    [SerializeField]
    private Image profileImage;
    [SerializeField]
    private TextMeshProUGUI nickName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var image = LobbyManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

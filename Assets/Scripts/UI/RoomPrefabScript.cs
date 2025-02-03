using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPrefabScript : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI roomTitle;
    [SerializeField]
    private TextMeshProUGUI currentPlayer;
    [SerializeField]
    private TextMeshProUGUI maxPlayer;
    [SerializeField]
    private Image lockSymbol;

    private RoomServerManager roomServerManager;
    private AuthManager authManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        roomServerManager = RoomServerManager.Instance;
        authManager = AuthManager.Instance;
    }

}

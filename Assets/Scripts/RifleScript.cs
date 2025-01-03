using UnityEngine;

public class RifleScript : MonoBehaviour
{
    [SerializeField]
    private Transform grapSocket;
    [SerializeField]
    private Transform supportSocket;
    private PlayerControl playerControl;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerControl = GetComponentInParent<PlayerControl>();
        grapSocket.rotation = Quaternion.Euler(new Vector3(-5.161f, 11.112f, -106.935f));
        supportSocket.rotation = Quaternion.Euler(new Vector3(-0.432f, 79.44f, 189.004f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

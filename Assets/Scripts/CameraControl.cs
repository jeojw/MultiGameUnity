using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private float mouseSensitivity = 400f;

    private PlayerControl playerControl;

    private float mouseY;
    private float mouseX;

    private bool playerAiming;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerControl = GetComponentInParent<PlayerControl>();
        mouseY = 0;
        mouseX = 0;
    }

    void Update()
    {
        playerAiming = playerControl.IsAiming;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        mouseX += Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;
        mouseY = Mathf.Clamp(mouseY, -90f, 90f);

        transform.localRotation = Quaternion.Euler(mouseY, mouseX, 0f);
        transform.position = target.position + new Vector3(0.5f, -0.3f, -2f);
        transform.LookAt(transform.position);
    }
}

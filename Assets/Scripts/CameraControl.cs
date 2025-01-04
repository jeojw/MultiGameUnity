using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float rotateSpeed = 400f;

    private PlayerControl playerControl;

    private float xRotate, yRotate, xRotateMove, yRotateMove;

    private Vector3 correctionVector;

    private bool playerAiming;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerControl = GetComponentInParent<PlayerControl>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        playerAiming = playerControl.IsAiming;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        xRotateMove = -Input.GetAxis("Mouse Y") * Time.deltaTime * rotateSpeed;
        yRotateMove = Input.GetAxis("Mouse X") * Time.deltaTime * rotateSpeed;

        yRotate += yRotateMove;
        xRotate += xRotateMove;

        xRotate = Mathf.Clamp(xRotate, -90, 90); // 위, 아래 고정

        //Quaternion quat = Quaternion.Euler(new Vector3(xRotate, yRotate, 0));
        //transform.rotation
        //    = Quaternion.Slerp(transform.rotation, quat, Time.deltaTime);

        if (playerAiming)
        {
            correctionVector = new Vector3(0.45f, 0f, -0.7f);
        }
        else
        {
            correctionVector = new Vector3(0.5f, 0.3f, -1.7f);
        }
        transform.position = target.position - correctionVector;
    }
}

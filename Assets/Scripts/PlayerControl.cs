using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody playerRigidbody;
    private Animator playerAnimator;
       
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 rotDirection = Vector3.zero;

    private float _moveSpeed = 5.0f;
    private float _rotSpeed = 2.0f;
    private float _jumpForce = 10f;
    private bool _isMoving = false;
    private bool _isGrounded = false;
    private bool _isJumping = false;
    private bool _isAiming = false;

    private readonly float groundCheckDistance = 1.0f;
    private LayerMask groundMask;

    public float MoveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = value; }
    }

    public float RotSpeed
    {
        get { return _rotSpeed; }
        set { _rotSpeed = value; }
    }

    public float JumpForce
    {
        get { return _jumpForce; }
        set { _jumpForce = value; }
    }

    public bool IsMoving
    {
        get { return _isMoving; }
        private set { _isMoving = value; }
    }
    public bool IsGrounded
    {
        get { return _isGrounded; }
        private set { _isGrounded = value; }
    }

    public bool IsJumping
    {
        get { return _isJumping; }
        private set { _isJumping = value; }
    }

    public bool IsAiming
    {
        get { return _isAiming; }
        private set { _isAiming = value; }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();

        groundMask = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        IsGrounded = Physics.Raycast(transform.position + new Vector3(0, 1, 0), Vector3.down, groundCheckDistance, groundMask);

        Debug.Log(IsGrounded);

        if (Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.Space))
        {
            IsMoving = true;
        }
        else
        {
            IsMoving = false;
            moveDirection = Vector3.zero;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            moveDirection = transform.forward;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            moveDirection = -transform.right;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            moveDirection = -transform.forward;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            moveDirection = transform.right;
        }

        if (IsGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            IsJumping = true;
        }

        else
        {
            IsJumping = false;
        }
        
        moveDirection.Normalize();
        rotDirection.Normalize();

        if (IsJumping)
        {
            playerRigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }

    }

    void FixedUpdate()
    {
        if (moveDirection != Vector3.zero)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, 
                                                        transform.position + moveDirection * 1000.0f, 
                                                        Time.fixedDeltaTime * MoveSpeed);

            playerRigidbody.MovePosition(newPosition);
        }

        if (rotDirection != Vector3.zero)
        {
            Quaternion newRot = Quaternion.RotateTowards(transform.rotation,
                                                        Quaternion.LookRotation(rotDirection), 
                                                        Time.fixedDeltaTime * RotSpeed);

            playerRigidbody.MoveRotation(newRot);
        }

    }
}

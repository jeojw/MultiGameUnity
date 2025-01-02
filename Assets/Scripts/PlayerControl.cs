using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody playerRigidbody;
       
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 rotDirection = Vector3.zero;

    private float _moveSpeed = 5.0f;
    private float _rotSpeed = 2.0f;
    private float _jumpForce = 5f;
    private bool _isMoving = false;
    private bool _isGrounded = false;
    private bool _isJumping = false;
    private bool _tryJump = false;
    private bool _isAiming = false;

    private bool _isWalking = false;
    private bool _isRunning = false;
    private bool _isCrouching = false;
    private bool _isProning = false;

    private bool _isFire = false;

    private bool _getRifle = false;
    private bool _getPistol = false;
    private bool _getMelee = false;

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

    public bool IsWalking
    {
        get { return _isWalking; }
        private set { _isWalking = value; }
    }

    public bool IsRunning
    {
        get { return _isRunning; }
        private set { _isRunning = value; }
    }

    public bool IsCrouching
    {
        get { return _isCrouching; }
        private set { _isCrouching = value; }
    }

    public bool IsProning
    {
        get { return _isProning; }
        private set { _isProning = value; }
    }

    public bool IsFire
    {
        get { return _isFire; }
        private set { _isFire = value; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        playerRigidbody = GetComponent<Rigidbody>();

        groundMask = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        IsGrounded = Physics.Raycast(transform.position + new Vector3(0, 1, 0), Vector3.down, groundCheckDistance, groundMask);

        Debug.Log(IsWalking);
        if ((Input.GetKey(KeyCode.W) ||
             Input.GetKey(KeyCode.A) ||
             Input.GetKey(KeyCode.S) ||
             Input.GetKey(KeyCode.D) ||
             Input.GetKey(KeyCode.Space)) &&
            !IsProning)
        {
            IsMoving = true;
            IsWalking = true;
        }
        else
        {
            IsMoving = false;
            IsWalking = false;
            moveDirection = Vector3.zero;
        }

        if (Input.GetKey(KeyCode.W))
        {
            moveDirection = transform.forward;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveDirection = -transform.right;
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveDirection = -transform.forward;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveDirection = transform.right;
        }

        _tryJump = IsGrounded && Input.GetKeyDown(KeyCode.Space);

        moveDirection.Normalize();
        rotDirection.Normalize();

        if (_tryJump)
        {
            playerRigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            _tryJump = false;
        }

        IsJumping = !IsGrounded;

        IsCrouching = Input.GetKey(KeyCode.LeftShift);

        IsProning = Input.GetKey(KeyCode.Tab);

        IsAiming = Input.GetMouseButton(1);

        IsFire = IsAiming && Input.GetMouseButton(0);

        if (IsWalking && !IsCrouching && Input.GetKey(KeyCode.CapsLock))
        {
            IsRunning = true;
            IsWalking = false;
        }

        else if (IsMoving && !Input.GetKey(KeyCode.CapsLock))
        {
            IsRunning = false;
            IsWalking = true;
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

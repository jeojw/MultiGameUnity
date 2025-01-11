using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody playerRigidbody;
    private CapsuleCollider capsuleCollider;
    private PlayerAnimation playerAnimation;
    private PlayerState playerState;
       
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 playerDirection;

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

    private readonly float groundCheckDistance = 0.1f;
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

    public Vector3 PlayerDirection
    {
        get { return playerDirection; }
        private set { playerDirection = value; }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimation = GetComponent<PlayerAnimation>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        playerState = GetComponent<PlayerState>();

        groundMask = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerState.IsDead)
        {
            IsGrounded = Physics.Raycast(capsuleCollider.transform.position, Vector3.down, groundCheckDistance, groundMask);

            if ((Input.GetKey(KeyCode.W) ||
                 Input.GetKey(KeyCode.A) ||
                 Input.GetKey(KeyCode.S) ||
                 Input.GetKey(KeyCode.D)) &&
                !IsProning && !playerAnimation.ProneProcedure)
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
                moveDirection = -transform.forward;
            }

            if (Input.GetKey(KeyCode.A))
            {
                moveDirection = transform.right;
            }

            if (Input.GetKey(KeyCode.S))
            {
                moveDirection = transform.forward;
            }

            if (Input.GetKey(KeyCode.D))
            {
                moveDirection = -transform.right;
            }

            _tryJump = IsGrounded && !IsCrouching && !IsProning && Input.GetKeyDown(KeyCode.Space);

            if (_tryJump)
            {
                playerRigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
                _tryJump = false;
            }

            //IsJumping = !IsGrounded;

            IsCrouching = Input.GetKey(KeyCode.LeftShift);

            IsProning = Input.GetKey(KeyCode.Tab);

            IsAiming = Input.GetMouseButton(1);

            IsFire = IsAiming && Input.GetMouseButton(0);

            if (IsWalking && !IsCrouching && Input.GetKey(KeyCode.CapsLock))
            {
                IsRunning = true;
            }

            else if (IsMoving && !Input.GetKey(KeyCode.CapsLock))
            {
                IsRunning = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (!playerState.IsDead)
        {
            if (moveDirection != Vector3.zero && IsMoving)
            {
                Vector3 newPosition = Vector3.MoveTowards(transform.position,
                                                           transform.position + moveDirection * 1000.0f,
                                                            Time.fixedDeltaTime * MoveSpeed);

                playerRigidbody.MovePosition(newPosition);
            }

            float mouseX = Input.GetAxis("Mouse X") * 4f;

            transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y + mouseX, 0f);
            PlayerDirection = Quaternion.Euler(0, mouseX, 0) * -transform.forward;
        }
    }
}

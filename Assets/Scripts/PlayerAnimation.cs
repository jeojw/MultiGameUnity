using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Animator playerAnimator;
    private PlayerControl playerControl;
    private PlayerState playerState;

    private bool isWalking;
    private bool isRunning;

    private bool reloadProcedure;

    private bool crouchProcedure;
    private bool isCrouching;

    private bool proneProcedure;
    private bool isProning;
    
    public bool CrouchProcedure
    {
        get { return crouchProcedure; }
        private set { crouchProcedure = value; }
    }
    public bool ProneProcedure
    {
        get { return proneProcedure; }
        private set { crouchProcedure = value; }
    }
    public bool IsCrouching
    {
        get { return isCrouching; }
        private set { isCrouching = value; }
    }
    public bool IsProning
    {
        get { return isProning; }
        private set { isProning = value; }
    }

    public bool ReloadProcedure
    {
        get { return reloadProcedure; }
        private set { reloadProcedure = value; }
    }

    private bool getRifle;
    private bool getPistol;
    private bool getMelee;
    private bool isAiming;
    private bool isFire;

    public bool IsFire
    {
        get { return isFire; }
        private set { isFire = value; }
    }
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerControl = GetComponent<PlayerControl>();
        playerState = GetComponent<PlayerState>();
    }

    void Walk(Vector2 dir)
    {
        playerAnimator.SetFloat("Horizontal", dir.x);
        playerAnimator.SetFloat("Vertical", dir.y);
        playerAnimator.SetBool("isWalking", playerControl.IsWalking);
    }

    void Fire()
    {
        IsFire = playerControl.IsFire;
        playerAnimator.SetBool("isFire", IsFire);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!playerState.IsDead)
        {
            Vector2 dir = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Walk(dir);

            crouchProcedure = (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Crouch State.Stand_to_Crouch_Rifle_Ironsights") ||
                playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Crouch State.Crouch_to_Stand_Rifle_Ironsights")) &&
                playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
            playerAnimator.SetBool("isCrouching", playerControl.IsCrouching);
            IsCrouching = playerControl.IsCrouching && !crouchProcedure;

            proneProcedure = (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Prone State.Stand_To_Prone") ||
                playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Prone State.Prone_To_Stand")) &&
                playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
            playerAnimator.SetBool("isProning", playerControl.IsProning);
            IsProning = playerControl.IsProning && !proneProcedure;


            playerAnimator.SetBool("isRunning", playerControl.IsRunning);
            playerAnimator.SetBool("isJumping", playerControl.IsJumping);

            if (playerControl.IsAiming)
            {
                Fire();
            }
            else
            {
                playerAnimator.SetBool("isFire", false);
            }

            if (IsProning || !isFire)
            {
                playerAnimator.SetLayerWeight(1, 0);
            }
            else if (!IsProning && isFire)
            {
                playerAnimator.SetLayerWeight(1, 1);
            }
        }
        else
        {
            //if (IsProning)
            //{
            //    playerAnimator.
            //}
        }
    }
}

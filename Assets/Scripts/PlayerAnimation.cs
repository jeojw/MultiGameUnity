using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Animator playerAnimator;
    private PlayerControl playerControl;

    private bool isWalking;
    private bool isRunning;
    private bool isCrouching;
    private bool isProning;

    private bool getRifle;
    private bool getPistol;
    private bool getMelee;
    private bool isAiming;
    private bool isFire;
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerControl = GetComponent<PlayerControl>();
    }

    void Walk(Vector2 dir)
    {
        playerAnimator.SetFloat("Horizontal", dir.x);
        playerAnimator.SetFloat("Vertical", dir.y);
        playerAnimator.SetBool("isWalking", playerControl.IsWalking);
    }

    void Fire()
    {
        isFire = playerControl.IsFire;
        playerAnimator.SetBool("isFire", isFire);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 dir = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Walk(dir);

        playerAnimator.SetBool("isCrouching", playerControl.IsCrouching);
        playerAnimator.SetBool("isProning", playerControl.IsProning);
        isProning = playerControl.IsProning;
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

        if (isProning || !isFire)
        {
            playerAnimator.SetLayerWeight(1, 0);
        }
        else if (!isProning && isFire)
        { 
            playerAnimator.SetLayerWeight(1, 1);
        }
        
        
    }
}

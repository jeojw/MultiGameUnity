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

    private float getRifle;
    private float getPistol;
    private float getMelee;
    private float isAiming;
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerControl = GetComponent<PlayerControl>();
    }

    void Walk(Vector2 dir)
    {
        playerAnimator.SetFloat("Horizontal", dir.x);
        playerAnimator.SetFloat("Vertical", dir.y);
        isWalking = playerControl.IsWalking;
        playerAnimator.SetBool("isWalking", isWalking);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 dir = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Walk(dir);
    }
}

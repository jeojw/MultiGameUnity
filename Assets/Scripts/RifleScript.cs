using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

public class RifleScript : MonoBehaviour
{
    [SerializeField]
    private Transform grapSocket;
    [SerializeField]
    private Transform supportSocket;
    [SerializeField]
    private TwoBoneIKConstraint rightHandIK;
    [SerializeField]
    private TwoBoneIKConstraint leftHandIK;
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private Transform firePosition;
    [SerializeField]
    private Animator playerAnimator;
    [SerializeField]
    private Transform playerTransform;
    private PlayerAnimation playerAnimation;
    private PlayerState playerState;
    private PlayerControl playerControl;
    private Animator rifleAnimator;

    private LayerMask groundLayer;
    private Vector3 targetDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerAnimation = GetComponentInParent<PlayerAnimation>();
        playerState = GetComponentInParent<PlayerState>();
        playerControl = GetComponentInParent<PlayerControl>();
        rifleAnimator = GetComponentInParent<Animator>();

        transform.localPosition = new Vector3(-0.0786f, 0.3647f, 0.028f);
        //transform.localRotation = Quaternion.Euler(new Vector3(-109.534f, 204.619f, -24.03302f));

        groundLayer = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState.ShotPossible)
        {
            rifleAnimator.SetBool("isFire", true);
            Instantiate(bullet, firePosition.transform.position, Quaternion.identity);
        }
        else
        {
            rifleAnimator.SetBool("isFire", false);
        }
        

        if (playerAnimation.ProneProcedure)
        {
            leftHandIK.weight = 0f;
        }
        else
        {
            leftHandIK.weight = 1f;
        }

        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            targetDirection = playerControl.PlayerDirection;
            Vector3 surfaceNormal = hit.normal; // 지면 법선
            Vector3 zAxisAligned = Vector3.ProjectOnPlane(targetDirection, surfaceNormal).normalized;

            transform.rotation = Quaternion.LookRotation(zAxisAligned, surfaceNormal);
        }
    }
}

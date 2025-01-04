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
    private Transform spineTransform;

    private PlayerAnimation playerAnimator;

    private Transform initGrapSocketPos;
    private Transform initSupportSocketPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerAnimator = GetComponentInParent<PlayerAnimation>();
        grapSocket.rotation = Quaternion.Euler(new Vector3(-5.161f, 11.112f, -106.935f));
        supportSocket.rotation = Quaternion.Euler(new Vector3(-0.432f, 79.44f, 189.004f));
    }

    // Update is called once per frame
    void Update()
    {
        if (playerAnimator.CrouchProcedure || playerAnimator.ProneProcedure)
        {
            leftHandIK.weight = 0f;
        }
        else
        {
            leftHandIK.weight = 1f;
        }
        transform.position = spineTransform.position + new Vector3(-0.17f, -0.1f, -0.45f);
    }
}

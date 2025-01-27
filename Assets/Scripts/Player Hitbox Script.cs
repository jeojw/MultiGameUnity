using Fusion;
using UnityEngine;

public class PlayerHitboxScript : NetworkBehaviour
{
    private CapsuleCollider capsuleCollider;
    [SerializeField]
    private Transform headSpot;
    [SerializeField] 
    private Transform floorSpot;
    [SerializeField]
    private Transform rootSpot;
    private PlayerAnimation playerAnimation; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Spawned()
    {
        base.Spawned();

        if (HasInputAuthority && HasStateAuthority)
        {
            capsuleCollider = GetComponent<CapsuleCollider>();
            playerAnimation = GetComponent<PlayerAnimation>();
        }
    }

    // Update is called once per frame

    public override void Render()
    {
        base.Render();

        if (HasInputAuthority && HasStateAuthority)
        {
            if (playerAnimation.ProneProcedure)
            {
                capsuleCollider.direction = 1;
                capsuleCollider.radius = 0.4f;
                capsuleCollider.height = Vector3.Distance(headSpot.position, rootSpot.position);
                capsuleCollider.center = new Vector3(0, capsuleCollider.height / 2, 0);
            }
            else if (playerAnimation.IsProning)
            {
                capsuleCollider.direction = 2;
                capsuleCollider.radius = 0.5f;
                capsuleCollider.height = Vector3.Distance(headSpot.position, floorSpot.position);
                capsuleCollider.center = new Vector3(0, 0.5f, 0);
            }
            else
            {
                capsuleCollider.direction = 1;
                capsuleCollider.radius = 0.5f;
                capsuleCollider.height = Vector3.Distance(headSpot.position, rootSpot.position);
                capsuleCollider.center = new Vector3(0, capsuleCollider.height / 2, 0);
            }
        }
    }
}

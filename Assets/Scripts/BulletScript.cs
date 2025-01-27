using Fusion;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BulletScript : NetworkBehaviour
{
    private BoxCollider bulletHitbox;
    private Rigidbody rb;
    [SerializeField]
    private float damage;
    [SerializeField]
    private float speed;
    [SerializeField]
    private PlayerControl playerControl;
    private LayerMask groundLayer;
    private Vector3 targetDirection;
    public float Damage
    {
        get { return damage; }
        private set { damage = value; }
    }


    public override void Spawned()
    {
        base.Spawned();

        if (HasInputAuthority && HasStateAuthority)
        {
            bulletHitbox = GetComponent<BoxCollider>();
            rb = GetComponent<Rigidbody>();
            groundLayer = LayerMask.GetMask("Ground");
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            Destroy(this.gameObject);
        }
    }
    // Update is called once per frame

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (HasInputAuthority && HasStateAuthority)
        {
            Ray ray = new Ray(transform.position, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                targetDirection = playerControl.PlayerDirection;
                Vector3 surfaceNormal = hit.normal; // 지면 법선
                Vector3 zAxisAligned = Vector3.ProjectOnPlane(targetDirection, surfaceNormal).normalized;

                transform.rotation = Quaternion.LookRotation(zAxisAligned, surfaceNormal);
            }

            rb.AddForce(playerControl.PlayerDirection * speed, ForceMode.Impulse);
        }
    }
}

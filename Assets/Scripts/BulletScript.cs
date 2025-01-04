using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private BoxCollider bulletHitbox;
    private Rigidbody rb;
    [SerializeField]
    private float damage;
    [SerializeField]
    private float speed;
    public float Damage
    {
        get { return damage; }
        private set { damage = value; }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bulletHitbox = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        transform.rotation = new Quaternion(0, 180f, 0, 0);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            Destroy(this.gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        rb.AddForce(-Vector3.forward * speed, ForceMode.Impulse);
    }
}

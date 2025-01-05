using UnityEngine;

public class PlayerState : MonoBehaviour
{
    private PlayerHitboxScript playerHitboxScript;
    private float hp = 100f;
    private bool isDead;
    public bool IsDead
    {
        get { return isDead; }
        private set { isDead = value; }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerHitboxScript = GetComponent<PlayerHitboxScript>();
    }

    void getDamage(float damage)
    {
        hp -= damage;
    }

    // Update is called once per frame
    void Update()
    {
        IsDead = hp < 0;
    }
}

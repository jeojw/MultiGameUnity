using Fusion;
using UnityEngine;

public class PlayerState : NetworkBehaviour
{
    private PlayerHitboxScript playerHitboxScript;
    private PlayerControl playerControl;
    private float hp = 100f;

    private bool shotPossible;
    private float shotDelay = 0.4f;
    private float shotDelayStart = 0;
    private float shotDelayElapsed = 0;

    public bool ShotPossible
    {
        get { return shotPossible; }
        private set { shotPossible = value; }
    }
    public float ShotDelay
    {
        get { return shotDelay; }
        set { shotDelay = value; }
    }

    public float ShotDelayStart
    {
        get { return shotDelayStart; }
        private set { shotDelayStart = value; }
    }

    public float ShotDelayElapsed
    {
        get { return shotDelayElapsed; }
        private set { shotDelayElapsed = value; }
    }

    private bool isDead;
    public bool IsDead
    {
        get { return isDead; }
        private set { isDead = value; }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public override void Spawned()
    {
        base.Spawned();

        if (HasStateAuthority && HasInputAuthority)
        {
            playerHitboxScript = GetComponent<PlayerHitboxScript>();
            playerControl = GetComponent<PlayerControl>();
        }
    }

    void CalculateGunCoolDown()
    {
        if (playerControl.IsFire)
        {
            if (ShotDelayStart == 0f)
            {
                ShotPossible = true;
                ShotDelayStart = Time.time;
            }
            else
            {
                ShotPossible = false;
                ShotDelayElapsed = Time.time - ShotDelayStart;
                if (ShotDelayElapsed >= ShotDelay)
                {
                    ShotDelayElapsed = 0f;
                    ShotDelayStart = 0f;
                }
            }
        }
    }

    void getDamage(float damage)
    {
        hp -= damage;
    }

    public override void Render()
    {
        base.Render();

        if (HasStateAuthority && HasInputAuthority)
        {
            if (!(IsDead = hp < 0))
            {
                CalculateGunCoolDown();
            }
        }
    }
}

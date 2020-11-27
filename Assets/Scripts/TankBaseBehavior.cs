using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomEvents;

public class TankBaseBehavior : MonoBehaviour
{
    public event GameObjectEventHandler OnDestroyed;

    public GameObject bulletPrefab;
    public float gunCooldown = 3.0f;
    public float gunPower = 300.0f;

    public float timeInvincible = 5.0f;

    public int health = 1;

    public float speed = 3.0f;
    protected float extraSpeed = 0;

    protected Vector2 lookDirection = new Vector2(0, 1);

    protected Rigidbody2D rigidbody2d;
    protected Animator animator;
    protected BoxCollider2D collider2d;

    protected float gunCooldownTimer = 0.0f;
    protected bool isGunCooldown = false;

    protected float invincibleTimer = 0.0f;
    protected bool isInvincible = false;

    protected bool isFrozen = false;
    protected bool isDestroyOnDead = true;

    // Tank's informations
    protected int level = 1;

    protected List<RaycastHit2D> raycastResults = new List<RaycastHit2D>();
    protected ContactFilter2D contactFilter = new ContactFilter2D();

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider2d = GetComponent<BoxCollider2D>();

        contactFilter.NoFilter();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (health <= 0)
        {
            Dead();
        }
        else
        {
            if (isGunCooldown)
            {
                gunCooldownTimer -= Time.deltaTime;

                if (gunCooldownTimer < 0.0f)
                {
                    isGunCooldown = false;
                }
            }

            if (isInvincible)
            {
                invincibleTimer -= Time.deltaTime;

                if (invincibleTimer < 0.0f)
                {
                    isInvincible = false;

                    animator.SetBool("Invincible", false);
                }
            }
        }
    }

    protected float GetFinalSpeed()
    {
        return speed + extraSpeed;
    }

    protected void Launch(int layer)
    {
        if (!isGunCooldown && !isFrozen)
        {
            isGunCooldown = true;
            gunCooldownTimer = gunCooldown;

            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            Bullet bulletCs = bullet.GetComponent<Bullet>();

            bullet.layer = layer;

            bulletCs.power = level;
            bulletCs.Launch(lookDirection, gunPower);
        }
    }

    protected bool TryMoveForward(Vector2 origin)
    {
        raycastResults.Clear();

        Physics2D.BoxCast(origin, collider2d.size, 0.0f, lookDirection, contactFilter, raycastResults, 0.0f);

        foreach (RaycastHit2D hit in raycastResults)
        {
            Rigidbody2D otherRb = hit.rigidbody;
            if (otherRb != null)
                if (hit.rigidbody.position.x == rigidbody2d.position.x &&
                    hit.rigidbody.position.y == rigidbody2d.position.y)
                    continue;

            string hitLayerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
            if (hitLayerName.Contains("Bullet") || hitLayerName.Equals("PowerUp"))
                continue;

            if (hit.collider.CompareTag("Ice"))
                continue;

            Vector2 hitDirection = hit.point - origin;

            float dot = Vector2.Dot(lookDirection, hitDirection.normalized);

            if (dot > 0.0f)
            {
                return false;
            }
        }

        return true;
    }

    private void Dead()
    {
        gameObject.SetActive(false);

        if (OnDestroyed != null)
            OnDestroyed(gameObject);

        if (isDestroyOnDead)
            Destroy(gameObject);
    }

    public void AddExtraSpeed(float unit)
    {
        extraSpeed += unit;
    }

    public int GetLevel()
    {
        return level;
    }

    public void ChangeHealth(int amount)
    {
        if (isInvincible)
            return;

        health += amount;
    }

    public void Invincible()
    {
        isInvincible = true;
        invincibleTimer = timeInvincible;

        animator.SetBool("Invincible", true);
    }

    public void Freezing()
    {
        isFrozen = true;

        if (rigidbody2d != null)
            rigidbody2d.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void UnFreezing()
    {
        isFrozen = false;

        if (rigidbody2d != null)
            rigidbody2d.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}

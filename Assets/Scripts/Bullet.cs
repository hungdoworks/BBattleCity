using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int power { get; set; }

    private Rigidbody2D rigidbody2d;
    private BoxCollider2D collider2d;
    private Animator animator;

    private Vector2 previousPosition;
    private float minimumExtent;
    private float sqrMinimumExtent;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        previousPosition = rigidbody2d.position;
        minimumExtent = collider2d.bounds.extents.y;
        sqrMinimumExtent = minimumExtent * minimumExtent;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        Vector2 movementThisStep = rigidbody2d.position - previousPosition;
        float movementSqrMagnitude = movementThisStep.sqrMagnitude;

        int layerMask = LayerMask.GetMask("WallTerrain");

        if (movementSqrMagnitude > sqrMinimumExtent)
        {
            float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);

            RaycastHit2D hit = Physics2D.Raycast(previousPosition, movementThisStep, movementMagnitude, layerMask);

            if (hit.collider == null)
                return;

            if (hit.collider.isTrigger == false)
            {
                // rigidbody2d.position = hit.point - (movementThisStep / movementMagnitude);
                SoundManager.Instance.BulletOnWall();

                Destroy(gameObject);
            }
        }

        previousPosition = rigidbody2d.position;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        TankBaseBehavior tank = other.gameObject.GetComponent<TankBaseBehavior>();

        if (tank != null)
        {
            if (tank.GetLevel() > 3)
                SoundManager.Instance.BulletOnBigEnemy();

            // if (other.gameObject.CompareTag("Player"))
            //     tank.ChangeHealth(0);
            // else if (other.gameObject.CompareTag("Enemy"))
            //     tank.ChangeHealth(-99);

            tank.ChangeHealth(-1);
        }

        if (other.collider.gameObject.layer == LayerMask.NameToLayer("WallTerrain"))
            SoundManager.Instance.BulletOnWall();

        Destroy(gameObject);
    }

    public void Launch(Vector2 direction, float force)
    {
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);

        rigidbody2d.AddForce(direction * force);
    }
}

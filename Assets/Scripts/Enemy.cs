using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : TankBaseBehavior
{
	public float timeChangeDirection = 1.5f;
	public bool dropItemOnDead = false;
	
	// This should set to player's power core position
	private Vector2 target;
	private bool targetFounded = false;
	
	private Vector2 lastMovingPosition = Vector2.zero;
	
	private float changeDirectionTimer = 0.0f;
	private bool isDirectionChanging = false;
	
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
		
		lookDirection = new Vector2(0.0f, -1.0f); 
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
		
		if (isDirectionChanging)
		{
			changeDirectionTimer -= Time.deltaTime;
			
			if (changeDirectionTimer < 0.0f)
			{
				isDirectionChanging = false;
				
				lastMovingPosition = Vector2.zero;
				
				ChangeDirection(0, true);
			}
		}
		
		Launch(LayerMask.NameToLayer("EnemyBullet"));
    }
	
	void FixedUpdate()
	{		
		if (!targetFounded)
		{
			Vector2 position = rigidbody2d.position;

			bool changeDirection = (Mathf.Abs(position.x - lastMovingPosition.x) + Mathf.Abs(position.y - lastMovingPosition.y)) < 0.05f;
			if (changeDirection)
			{
				if (!isDirectionChanging)
				{
					isDirectionChanging = true;
					changeDirectionTimer = timeChangeDirection;
				}
			}
			else 
			{
				if (!isFrozen)
				{
					lastMovingPosition = position;
					
					bool canMove = true;
					
					position.x = position.x + GetFinalSpeed() * lookDirection.x * Time.deltaTime;
					position.y = position.y + GetFinalSpeed() * lookDirection.y * Time.deltaTime;
					
					canMove = TryMoveForward(position);

					if (canMove)
						rigidbody2d.MovePosition(position);
				}
			}
		}
		
		raycastResults.Clear();
		
		int result = Physics2D.Raycast(rigidbody2d.position, lookDirection, contactFilter, raycastResults, 2.0f);
		if (result > 0)
		{		
			foreach (var hit in raycastResults)
			{
				if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
				{
					if (!Mathf.Approximately(hit.transform.position.x, transform.position.x) ||
						!Mathf.Approximately(hit.transform.position.y, transform.position.y))
					{
						ChangeDirection(hit.distance);
					}
				}
				// else
				// if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain"))
				// {
					// if (hit.collider.gameObject.CompareTag("PowerCore"))
					// {
						// if (!targetFounded)
						// {
							// targetFounded = true;
						// }
						
						// break;
					// }
				// }
			}
		}
	}
	
	private bool ChangeDirection(float hitDistance, bool force = false)
	{
		if (force || (!targetFounded && hitDistance < 0.5f))
		{			
			float direction = Random.Range(-1.0f, 1.0f);
			// horizontal if lower than 0, vertical if greater than 0
			float axis = Random.Range(-1.0f, 1.0f);
			
			if (axis > 0.0f)
				lookDirection.Set(0.0f, direction);
			else
				lookDirection.Set(direction, 0.0f);
			
			lookDirection.Normalize();
			
			animator.SetFloat("Horizontal", lookDirection.x);
			animator.SetFloat("Vertical", lookDirection.y);			

			return true;
		}
		
		return false;
	}
}

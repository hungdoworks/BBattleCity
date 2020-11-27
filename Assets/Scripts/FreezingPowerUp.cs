using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezingPowerUp : PowerUp
{
	public float timeFreezing = 10.0f;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }
	
	void OnTriggerEnter2D (Collider2D other)
	{
		Player player = other.gameObject.GetComponent<Player>();
		
		if (player != null)
		{
			PowerUpObtained();
			
			GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
			gameManager.FreezingEnemies(timeFreezing);
			
			// Destroy(gameObject);
			gameObject.SetActive(false);
		}
	}
}

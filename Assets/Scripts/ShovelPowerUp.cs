using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShovelPowerUp : PowerUp
{	
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
			gameManager.PowerCoreRecoverWall();
			
			// Destroy(gameObject);
			gameObject.SetActive(false);
		}
	}
}

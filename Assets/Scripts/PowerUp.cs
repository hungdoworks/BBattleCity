using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomEvents;

public class PowerUp : MonoBehaviour
{
	public event GameObjectEventHandler OnTimeOut;
	
	public float timeAvailable = 15.0f;
	
	private float availableTimer = 0.0f;
	private bool isAvailable = false;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isAvailable)
		{
			availableTimer -= Time.deltaTime;
			
			if (availableTimer < 0.0f)
			{				
				if (OnTimeOut != null)
					OnTimeOut(gameObject);
				
				gameObject.SetActive(false);
			}
		}
    }

	protected void PowerUpObtained()
	{
		SoundManager.Instance.PowerUpObtained();
	}
	
	public void MakeAvailable()
	{
		isAvailable = true;
		availableTimer = timeAvailable;
		
		gameObject.SetActive(true);
	}
}

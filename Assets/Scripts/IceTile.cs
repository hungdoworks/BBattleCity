using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTile : MonoBehaviour
{
	[SerializeField] private float slippery = 5.0f;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	void OnTriggerEnter2D(Collider2D other)
	{
		Player player = other.GetComponent<Player>();
		
		if (player != null)
		{
			player.AddExtraSpeed(slippery);
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		Player player = other.GetComponent<Player>();
		
		if (player != null)
		{
			player.AddExtraSpeed(-1 * slippery);
		}
	}
}

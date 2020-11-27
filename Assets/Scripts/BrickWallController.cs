using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickWallController : MonoBehaviour, IWallController
{
	private List<BrickTile> bricks = new List<BrickTile>();
	
    // Start is called before the first frame update
    void Start()
    {
        GetComponentsInChildren(true, bricks);
		
		foreach (BrickTile brick in bricks)
		{
			brick.isPowerCoreWall = true;
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void Recover()
	{
		foreach (BrickTile brick in bricks)
		{
			brick.transform.gameObject.SetActive(true);
		}
	}
}

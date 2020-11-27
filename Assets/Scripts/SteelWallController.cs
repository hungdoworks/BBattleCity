using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelWallController : MonoBehaviour
{
	private List<SteelTile> steels = new List<SteelTile>();
	
    // Start is called before the first frame update
    void Start()
    {
        GetComponentsInChildren(true, steels);
		
		foreach (SteelTile steel in steels)
		{
			steel.isPowerCoreWall = true;
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void Recover()
	{
		foreach (SteelTile steel in steels)
		{
			steel.transform.gameObject.SetActive(true);
		}
	}
}

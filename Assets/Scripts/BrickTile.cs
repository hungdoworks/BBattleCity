using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickTile : MonoBehaviour
{
    public bool isPowerCoreWall = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Bullet bullet = other.gameObject.GetComponent<Bullet>();

        if (bullet != null)
        {
            SoundManager.Instance.BulletOnTile();

            if (isPowerCoreWall)
                gameObject.SetActive(false);
            else
                Destroy(gameObject);
        }
    }
}

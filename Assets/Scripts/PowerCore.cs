using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomEvents;

public class PowerCore : MonoBehaviour
{
    public event NoArgumentEventHandler OnDestroyed;

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
            SoundManager.Instance.PlayerDestroyed();

            SpawnManager.Instance.SpawnExplosiveAt(transform.position);

            if (OnDestroyed != null)
                OnDestroyed();

            Destroy(gameObject);
        }
    }
}

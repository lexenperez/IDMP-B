using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public CircleBullet vars;
    public GameObject bullet;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BulletHellFuncs.CircularBullet(vars, bullet, transform));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : LifeTimer
{
    // If this gets laggy then maybe combine the same spawn bullets into one bullet manager class

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckForExpire();
    }

    public override void Expire()
    {
        Destroy(gameObject);
    }
}

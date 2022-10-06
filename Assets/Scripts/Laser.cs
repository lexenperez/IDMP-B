using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : LifeTimer
{
    void Start()
    {
        transform.localScale = new Vector3(0, 50, 1);
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

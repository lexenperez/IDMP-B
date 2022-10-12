using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : LifeTimer
{
    private Rigidbody2D rb2d;
    public Vector2 startingVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.AddForce(startingVelocity);
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

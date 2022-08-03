using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLog : Bullet
{
    public MathFuncs.GrowthFactor gf;
    public float speed;
    public BulletVars bv;

    public float currentSpeed = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //spd = Mathf.Clamp((spd + speed) * Time.deltaTime, 0.0f, 25.0f);
        
        transform.position = MathFuncs.LogarithmicSpiral(bv.a, bv.b, bv.gf, currentSpeed);
        CheckForExpire();
    }

    private void FixedUpdate()
    {
        // This gets really fast
        currentSpeed += speed * Time.deltaTime;
    }

    public override void Expire()
    {
        base.Expire();
    }


}

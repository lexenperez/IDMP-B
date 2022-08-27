using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : LifeTimer
{
    // If this gets laggy then maybe combine the same spawn bullets into one bullet manager class
    public float tweenTime;
    // Start is called before the first frame update
    void Start()
    {
        Tween();
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
    
    private void Tween()
    {
        if (gameObject)
        {
            LeanTween.cancel(gameObject);
            LeanTween.rotateAround(gameObject, Vector3.forward, 360.0f, tweenTime)   
                .setRepeat(-1);
        }

    }
}

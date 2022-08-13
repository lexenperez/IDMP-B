using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LifeTimer : MonoBehaviour
{
    // General script for anything with a life
    public float lifetime;

    private float timeSinceExisting = 0;

    public bool CheckForExpire()
    {
        timeSinceExisting += Time.deltaTime;
        if (timeSinceExisting >= lifetime)
        {
            Expire();
            return true;
        }
        return false;
        
        
    }
    public abstract void Expire();
}

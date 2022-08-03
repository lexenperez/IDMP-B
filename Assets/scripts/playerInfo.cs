using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerInfo : MonoBehaviour
{
    
    public float health, mana, elapsedTime, timer = 0.2f;
    void Start()
    {
        health = 100;
        mana = 0;
        elapsedTime = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (mana < 100)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= timer)
            {
                elapsedTime = 0;
                mana++;
            }
        }
    }
}

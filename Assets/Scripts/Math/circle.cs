using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class circle : MonoBehaviour
{
    public Vector3 v,o;
    private float timer;
    private bool pause = false;
    public float speed; 
    void Start()
    {
        v = new Vector3(-10,0,0);
        transform.position = new Vector3(0, 0, 0) + v;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!pause)
        {
            timer += Time.deltaTime;
            v = Quaternion.AngleAxis(360 / speed * Time.deltaTime, Vector3.forward) * v;
            o = new Vector3(0, Mathf.Sin(2 * Mathf.PI / speed * timer), 0);
            transform.position = new Vector3(0, 0, 0) + v + o * 5;
            transform.eulerAngles = new Vector3(0, 0, 360 * timer*40);
        }
    }


    public void Pause()
    {
        pause = true;
    }

    public void Resume()
    {
        pause = false;
    }

    public void Reset()
    {
        v = new Vector3(-10, 0, 0);
        transform.position = new Vector3(0, 0, 0) + v;
        timer = 0;
    }
}

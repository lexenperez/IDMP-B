using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public CircleBullet vars;
    public GameObject bullet;

    private List<Vector2> points;
    private bool firstEnable = true;
    // Start is called before the first frame update
    void Start()
    {
        points = BulletHellFuncs.GetPointsInCircle(vars);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (!firstEnable)
        {
            StartCoroutine(BulletHellFuncs.CircularBullet(vars, points, bullet, transform));
        }
        else
        {
            firstEnable = !firstEnable;
        }
        
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void modRotation(float rotation)
    {
        vars.rotation += rotation;
        if (vars.rotation >= 360.0f)
        {
            vars.rotation = 360.0f - vars.rotation;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

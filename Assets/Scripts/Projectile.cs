using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public Vector2 startingVelocity;
    public float lifetime;

    private float timeSinceExisting = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.AddForce(startingVelocity);
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceExisting += Time.deltaTime;
        if (timeSinceExisting >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : Platform
{
    public float speed;
    public float upperBound;
    public float lowerBound;
    public bool horizontalMovement;
    public bool reverseStart;

    private Vector2 upperEndpoint;
    private Vector2 lowerEndpoint;
    private bool flip = false;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        Vector2 startingPosition = transform.position;
        // Setup Endpoints of moving platform

        // Should check whether end points collide with wall (probably no need since theyre directly placed)
        if (horizontalMovement)
        {
            upperEndpoint = startingPosition;
            upperEndpoint.x += upperBound;

            lowerEndpoint = startingPosition;
            lowerEndpoint.x -= lowerBound;
        }
        else
        {
            upperEndpoint = startingPosition;
            upperEndpoint.y += upperBound;

            lowerEndpoint = startingPosition;
            lowerEndpoint.y -= lowerBound;
        }

        if (reverseStart)
        {
            flip = true;
        }
        
    }
    void FixedUpdate()
    {
        if (horizontalMovement)
        {
            if (flip)
            {
                rb2d.velocity = Vector2.right * speed;

                if (transform.position.x >= upperEndpoint.x)
                {
                    flip = false;
                }
            }
            else
            {
                rb2d.velocity = Vector2.left * speed;

                if (transform.position.x <= lowerEndpoint.x)
                {
                    flip = true;
                }
            }

        }
        else
        {
            if (flip)
            {
                rb2d.velocity = Vector2.up * speed;

                if (transform.position.y >= upperEndpoint.y)
                {
                    flip = false;
                }
            }
            else
            {
                rb2d.velocity = Vector2.down * speed;

                if (transform.position.y <= lowerEndpoint.y)
                {
                    flip = true;
                }
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
       

    }

    //https://www.monkeykidgc.com/2021/03/unity-moving-platform.html
    // Lets object on top follow moving object
    void OnCollisionEnter2D(Collision2D col)
    {
        //col.gameObject.transform.SetParent(gameObject.transform, true);
    }
    void OnCollisionExit2D(Collision2D col)
    {
        //col.gameObject.transform.parent = null;
    }

    // Debug
    private void OnDrawGizmos()
    {
        Vector2 startingPosition = transform.position;
        Vector2 upperE;
        Vector2 lowerE;
        if (horizontalMovement)
        {
            upperE = startingPosition;
            upperE.x += upperBound;

            lowerE = startingPosition;
            lowerE.x -= lowerBound;
        }
        else
        {
            upperE = startingPosition;
            upperE.y += upperBound;

            lowerE = startingPosition;
            lowerE.y -= lowerBound;
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, upperE);
        Gizmos.DrawLine(transform.position, lowerE);
    }


}

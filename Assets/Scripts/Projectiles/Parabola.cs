using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parabola : LifeTimer
{
    private Rigidbody2D rb2d;
    public Vector2 startingVelocity;
    [SerializeField] private int debugLineLength;
    [SerializeField] private string platformTag;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = startingVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        DrawTrajectory(transform.position, rb2d.velocity, rb2d.gravityScale);

    }

    private void DrawTrajectory(Vector3 startPos, Vector2 velocity, float gravity)
    {
        // Draws a basic trajectory based on velocity, doesnt account for drag
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.positionCount = debugLineLength;

        Vector2 pos = startPos;
        for (int i = 0; i < debugLineLength; i++)
        {
            lr.SetPosition(i, pos);
            velocity += (Physics2D.gravity * gravity) * Time.fixedDeltaTime;
            pos += velocity * Time.fixedDeltaTime;
        }

    }

    private void OnDrawGizmos()
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        Rigidbody2D rg = GetComponent<Rigidbody2D>();
        lr.positionCount = debugLineLength;

        Vector2 pos = transform.position;
        Vector2 velocity = startingVelocity;
        for (int i = 0; i < debugLineLength; i++)
        {
            lr.SetPosition(i, pos);
            velocity += (Physics2D.gravity * rg.gravityScale) * Time.fixedDeltaTime;
            pos += velocity * Time.fixedDeltaTime;
        }
    }

    public override void Expire()
    {
        Destroy(gameObject);
    }

    public void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.CompareTag(platformTag))
        {
            if (collision.gameObject.GetComponent<DamagingPlatform>())
            {
                collision.gameObject.GetComponent<DamagingPlatform>().EnableDamage();
                //TODO add explosion particles
                Destroy(gameObject);
            }

        }
    }

}

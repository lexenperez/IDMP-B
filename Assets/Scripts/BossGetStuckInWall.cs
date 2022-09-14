using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGetStuckInWall : MonoBehaviour
{
    //boss can get the arrows stuck in the wall and each other
    private float distance = 0.3f;
    [SerializeField] Rigidbody2D rb2d;

    // Update is called once per frame
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Boss1") || collision.tag.Equals("Boss2"))
        {
            rb2d.velocity = Vector3.zero;
        }
        else if (collision.tag.Equals("Walls"))
        {
            rb2d.velocity = Vector3.zero;
        }
    }
}

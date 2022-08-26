using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGetStuckInWall : MonoBehaviour
{
    //Raycast infront of the boss so they can get the spears stuck in the wall 
    private float distance = 0.1f;
    private Rigidbody2D rb2d;
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.DrawRay(transform.position + transform.TransformDirection(Vector3.left) * 4.01f, transform.TransformDirection(Vector3.left) * distance, Color.red);

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + transform.TransformDirection(Vector3.left) * 4.01f, transform.TransformDirection(Vector3.left), distance);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.name.Equals("Tilemap"))
            {
                rb2d.velocity = Vector3.zero;
            }
            if (hit.transform.tag.Equals("Player"))
            {
                //add player damage here maybe
            }
            else if (hit.transform.name.Equals("boss 1") || hit.transform.name.Equals("boss 2"))
            {
                rb2d.velocity = Vector3.zero;
            }
        }
        
    }
}

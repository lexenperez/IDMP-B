using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGetStuckInWall : MonoBehaviour
{
    //boss can get the arrows stuck in the wall and each other
    [SerializeField] Rigidbody2D rb2d;
    [SerializeField] Collider2D coll;
    [SerializeField] Collider2D parentcoll;
    [SerializeField] Boss2 parent;
    private float damage;
    [SerializeField] DamageDealer attack;


    public void Start()
    {
        damage = attack.GetDamage();
    }
    // Update is called once per frame
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ArrowHead"))
        {
            rb2d.velocity = Vector3.zero;
        }
        if (collision.CompareTag("Walls"))
        {
            rb2d.velocity = Vector3.zero;
        }
        //if attacked by player in the arrow point deflect the attack by making the boss invincible
        //doesnt really work that well
        if (collision.CompareTag("PlayerAttack"))
        {
            //parent.Deflect();
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            //parent.UnDeflect();
        }
    }
}

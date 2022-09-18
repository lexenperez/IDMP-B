using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGetStuckInWall : MonoBehaviour
{
    //boss can get the arrows stuck in the wall and each other
    [SerializeField] Rigidbody2D rb2d;
    [SerializeField] Collider2D coll;
    [SerializeField] Collider2D parentcoll;
    private float damage;
    [SerializeField] DamageDealer attack;

    // Update is called once per frame
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss2"))
        {
            rb2d.velocity = Vector3.zero;
        }
        else if (collision.CompareTag("Walls"))
        {
            rb2d.velocity = Vector3.zero;
        }
        //if attacked by player in the arrow point disable all colliders for short time. i.e deflect the attack
        //only works sometimes
        //if (collision.CompareTag("PlayerAttack"))
        //{
        //    damage = attack.GetDamage();
        //    attack.SetDamage(0);
        //}
    }

    //public void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("PlayerAttack"))
    //    {
    //        attack.SetDamage(damage);
    //    }
    //}
}

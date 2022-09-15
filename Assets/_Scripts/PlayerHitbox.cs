using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    // Tags for which GameObject allows damage
    [SerializeField] private string[] damageTags;
    [SerializeField] private PlayerHealth health;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (string tag in damageTags)
        {
            if (collision.CompareTag(tag))
            {
                //Debug.Log("Player taking dmg");
                health.TakeDamage(collision.GetComponent<DamageDealer>().GetDamage());
            }
        }
    }
}

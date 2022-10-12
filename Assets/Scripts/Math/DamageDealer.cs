using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    // Simple Script for any gameobject to just deal the damage given from this script
    // Could add other different damage fields such as damage over time
    [SerializeField] private float damage;

    public float GetDamage()
    {
        return damage;
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }
}

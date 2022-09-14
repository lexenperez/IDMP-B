using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : NPC
{
    // Abstract class should handle all general animation changes and hitboxes to enable

    // Same as playerHealth
    [SerializeField] private float invincibilityTime = 0.5f;
    private const float TOTAL_FLICKER_TIME = 0.2f;
    private bool isInvincible = false;
    [SerializeField] private string[] damageTags;
    private Color ogColor;

    public bool allowSelfHitbox;

    protected void Init()
    {
        base.Init();
        if (allowSelfHitbox)
        {
            base.baseCollider.enabled = true;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible)
            return;
        ogColor = sprite.color;
        hp -= damage;
        UpdateHealthBar();

        StartCoroutine(DamageFlicker());
    }

    private IEnumerator DamageFlicker()
    {
        isInvincible = true;

        for (int i = 0; i < invincibilityTime / TOTAL_FLICKER_TIME; i++)
        {
            Color c = sprite.color / 2.0f;
            sprite.color = c;

            yield return new WaitForSeconds(TOTAL_FLICKER_TIME / 2f);

            sprite.color = ogColor;

            yield return new WaitForSeconds(TOTAL_FLICKER_TIME / 2f);

        }

        isInvincible = false;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (string tag in damageTags)
        {
            if (collision.CompareTag(tag))
            {
                //Debug.Log("Boss taking dmg");
                TakeDamage(collision.GetComponent<DamageDealer>().GetDamage());
            }
        }
    }
}

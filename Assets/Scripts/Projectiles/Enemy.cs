using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : NPC
{
    // Abstract class should handle all general animation changes and hitboxes to enable

    // Same as playerHealth
    [SerializeField] private float invincibilityTime = 0.5f;
    [SerializeField] private string[] damageTags;

    private Color ogColor;
    private const float TOTAL_FLICKER_TIME = 0.5f;
    public bool isInvincible = false;

    public bool allowSelfHitbox;

    // Parent class possibly too messy
    [SerializeField] private AudioClip takeDamageSfx;

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

        if (audioSource)
        {
            audioSource.PlayOneShot(takeDamageSfx);
        }

    }

    private IEnumerator DamageFlicker()
    {
        isInvincible = true;

        for (int i = 0; i < invincibilityTime / TOTAL_FLICKER_TIME; i++)
        {
            Color c = sprite.color / 1.2f;
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

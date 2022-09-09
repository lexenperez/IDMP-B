using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerHealth : MonoBehaviour
{
    // Constants
    private const float TOTAL_FLICKER_TIME = 0.2f;
    // Tags for which GameObject allows damage
    [SerializeField] private string[] damageTags;

    // References
    private SpriteRenderer spriteRenderer;
    private bool isInvincible = false;
    [SerializeField] private Image healthBarImg;

    [Header("Health Configurations")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float invincibilityTime = 0.5f;
    private float currentHealth;

    // Other
    private Color originalColor;
 
    // Start is called before the first frame update
    void Start()
    {
        // Set current health
        currentHealth = maxHealth;
        
        // Store sprite color
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    //private void OnMouseDown()
    //{
    //    TakeDamage(10);
    //}

    protected void TakeDamage(float damage)
    {
        // Prevent player taking damage
        if (isInvincible)
            return;

        // Damage the player
        currentHealth -= damage;

        // Update health bar
        healthBarImg.fillAmount = Mathf.Clamp(currentHealth / maxHealth, 0, 1f);

        // Run flicker animation
        StartCoroutine(DamageFlicker());
        
        // Game Over
        if (currentHealth <= 0)
        {
            // Play death animation (if we add one)
            // Then game over screen
            Debug.Log("Game Over! Player has died!");
        }
    }

    private IEnumerator DamageFlicker()
    {
        isInvincible = true;

        for (int i = 0; i < invincibilityTime / TOTAL_FLICKER_TIME; i++)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0);

            yield return new WaitForSeconds(TOTAL_FLICKER_TIME/2f);

            spriteRenderer.color = originalColor;

            yield return new WaitForSeconds(TOTAL_FLICKER_TIME/2f);

        }

        isInvincible = false;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (string tag in damageTags)
        {
            if (collision.CompareTag(tag))
            {
                Debug.Log("Player taking dmg");
                TakeDamage(collision.GetComponent<DamageDealer>().GetDamage());
            }
        }
    }
}

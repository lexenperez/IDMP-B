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

    // References
    private SpriteRenderer spriteRenderer;
    private bool isInvincible = false;
    [SerializeField] private Image healthBarImg;
    [SerializeField] private bool takeNoDamage = false;

    [Header("Health Configurations")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float invincibilityTime = 0.5f;
    private float currentHealth;

    // Other
    private Color originalColor;

    // Get - Setter
    public bool IsInvincible {
        get { return isInvincible; }
        set { isInvincible = value; }
    }

    [SerializeField] private GameObject particleSpawner;
    private AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        // Set current health
        currentHealth = maxHealth;

        // Store sprite color
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        // Store take damage sfx
        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(float damage)
    {
        if (takeNoDamage)
            damage = 0;        

        // Prevent player taking damage
        if (isInvincible || GameManager.gameEnded)
            return;

        // Damage the player
        currentHealth -= damage;

        // Update health bar
        healthBarImg.fillAmount = Mathf.Clamp(currentHealth / maxHealth, 0, 1f);

        // Run flicker animation
        StartCoroutine(DamageFlicker());

        // Play sfx
        audioSource.Play();

        // Game Over
        if (currentHealth <= 0)
        {
            // Have particle spawner have the death sfx
            Instantiate(particleSpawner).transform.position = transform.position;
            // Play death animation (if we add one)
            // Then game over screen
            Debug.Log("Game Over! Player has died!");
            // Could add a explosion here
            Destroy(gameObject);
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
}

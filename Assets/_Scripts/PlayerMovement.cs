using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    // Consts
    private const float FIXED_UPDATE_TIME_INTERVAL = 20f / 1000f;

    // References
    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;
    private TrailRenderer trailRenderer;
    private PlayerWallSlide playerWallSlideScript;
    private PlayerJump playerJumpScript;
    private PlayerHealth playerHealthScript;
    private AudioSource audioSource;
    [SerializeField] private Image energyBarImg;
    [SerializeField] private AudioClip dashSfx;

    // Input
    private PlayerInput playerInput;
    private InputAction moveAction;

    // Movement
    private float directionX;
    private Vector2 desiredVelocity;

    [Header("Movement & Dash Configurations")]
    [SerializeField, Range(2f, 20f)] private float maxSpeed = 5f;
    [SerializeField, Range(0.01f, 1f)] private float dashTime = 0.8f;
    [SerializeField, Range(1f, 20f)] private float dashDistance = 5f;
    [SerializeField, Range(0.1f, 5f)] private float dashCooldown = 1f;
    [SerializeField, Range(0.01f, 1f)] private float dashInvincibilityFrame = 0.8f;

    [Header("Stamina Configurations")]
    [SerializeField, Range(1f, 500f)] private float maxStamina = 100f;
    [SerializeField, Range(1f, 100f)] private float staminaConsumption = 33f;
    [SerializeField, Range(1f, 20f)] private float staminaRefill = 1f;
    private float currentStamina;

    // Dash
    [Header("Dash States")]
    [SerializeField] private bool currentlyDashing;
    [SerializeField] private bool dashOnCooldown;

    private float dashSpeed;

    // Get - Setters
    public float GetDirectionX { get { return directionX; } }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        playerWallSlideScript = GetComponent<PlayerWallSlide>();
        playerJumpScript = GetComponent<PlayerJump>();
        playerHealthScript = GetComponent<PlayerHealth>();
        audioSource = GetComponent<AudioSource>();
        currentStamina = maxStamina;

        // Inputs
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];

        if (dashInvincibilityFrame > dashTime)
        {
            dashInvincibilityFrame = dashTime;
            Debug.LogWarning("Dash Invincibility Time cannot be greater than Dash Time. Setting Dash Invincibility Time to Dash Time");
        }
    }

    // Update is called once per frame
    void Update()
    {
        FaceDirection();

        // Set dash velocity
        if (currentlyDashing)
            rigidBody.velocity = new Vector2(dashSpeed, 0);

        // Refill Stamina
        if (currentStamina < maxStamina)
        {
            currentStamina = Mathf.Clamp(currentStamina + (staminaRefill * Time.deltaTime), 0, maxStamina);
            energyBarImg.fillAmount = Mathf.Clamp(currentStamina / maxStamina, 0, 1f);
        }
    }

    private void FaceDirection()
    {
        if (directionX > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (directionX < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started)
            if (!PauseMenu.gameIsPaused)
                if (!dashOnCooldown && (currentStamina >= staminaConsumption))
                {
                  StartCoroutine(Dash());
                  audioSource.PlayOneShot(dashSfx, 0.2f);
                }
    }

    private IEnumerator Dash()
    {
        currentlyDashing = true;
        dashOnCooldown = true;

        // Stamina Consumption
        currentStamina = Mathf.Clamp(currentStamina - staminaConsumption, 0, maxStamina);
        energyBarImg.fillAmount = Mathf.Clamp(currentStamina / maxStamina, 0, 1f);

        // Store Gravity Scale
        float origGravScale = rigidBody.gravityScale;
        rigidBody.gravityScale = 0;

        // Calculate dash power
        dashSpeed = dashDistance / dashTime;
        if (spriteRenderer.flipX)
            dashSpeed = -dashSpeed;

        // Dashing
        playerHealthScript.IsInvincible = true;
        trailRenderer.emitting = true;

        // Turn off invincibility
        yield return new WaitForSeconds(dashInvincibilityFrame);
        playerHealthScript.IsInvincible = false;

        // Finish Dashing
        if (dashTime != dashInvincibilityFrame)
            yield return new WaitForSeconds(dashTime - dashInvincibilityFrame);
        currentlyDashing = false;
        trailRenderer.emitting = false;

        // Set back gravity scale
        rigidBody.gravityScale = origGravScale;

        // Cooldown Finished
        yield return new WaitForSeconds(Mathf.Max(dashCooldown - dashTime, 0f));
        dashOnCooldown = false;
    }

    private void MovePlayer()
    {
        if (!currentlyDashing)
        {
            // Read Direction Input
            directionX = moveAction.ReadValue<float>();

            // Get Input and Desired Velocity
            desiredVelocity = new Vector2(directionX, 0f) * maxSpeed;

            // Disable movement inputs if wall sliding
            if (playerWallSlideScript.IsTouchingLeftWall && directionX == -1 && !playerJumpScript.IsOnGround)
                desiredVelocity.x = 0;
            else if (playerWallSlideScript.IsTouchingRightWall && directionX == 1 && !playerJumpScript.IsOnGround)
                 desiredVelocity.x = 0;

            // Set Velocity
            if (!playerJumpScript.GetLimitPlayerMovement)
            {
                // Player jumps off a wall, holding down jump and not pressing any directional inputs. Prevents setting the wall jump velocity.x to 0
                if (playerJumpScript.GetCurrentJumpType == PlayerJump.CurrentJumpType.Wall && playerJumpScript.IsPressingJump && directionX == 0)
                    desiredVelocity.x = rigidBody.velocity.x;

                rigidBody.velocity = new Vector2(desiredVelocity.x, rigidBody.velocity.y);
            }
        }
    }
}

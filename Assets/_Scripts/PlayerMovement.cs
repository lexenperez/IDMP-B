using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Consts
    private const float FIXED_UPDATE_TIME_INTERVAL = 20f / 1000f;

    // References
    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;
    private TrailRenderer trailRenderer;

    // Movement
    private float directionX;
    private Vector2 desiredVelocity;

    [Header("Configurations")]
    [SerializeField, Range(2f, 20f)] private float maxSpeed = 5f;
    [SerializeField, Range(0.01f, 1f)] private float dashTime = 0.8f;
    [SerializeField, Range(1f, 20f)] private float dashDistance = 5f;
    [SerializeField, Range(0.1f, 5f)] private float dashCooldown = 1f;

    // Dash
    [Header("Dash States")]
    [SerializeField] private bool currentlyDashing;
    [SerializeField] private bool dashOnCooldown;
    private float dashSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        FaceDirection();

        if (currentlyDashing)
            rigidBody.velocity = new Vector2(dashSpeed, 0);
    }

    private void FaceDirection()
    {
        if (directionX > 0)
            spriteRenderer.flipX = false;
        else if (directionX < 0)
            spriteRenderer.flipX = true;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        directionX = context.ReadValue<float>();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started)
            if (!dashOnCooldown)
                StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        currentlyDashing = true;
        dashOnCooldown = true;

        // Store Gravity Scale
        float origGravScale = rigidBody.gravityScale;
        rigidBody.gravityScale = 0;

        // Calculate dash power
        dashSpeed = dashDistance / dashTime;
        if (spriteRenderer.flipX)
            dashSpeed = -dashSpeed;

        trailRenderer.emitting = true;
        // Finish Dashing
        yield return new WaitForSeconds(dashTime);
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
            // Get Input and Desired Velocity
            desiredVelocity = new Vector2(directionX, 0f) * maxSpeed;

            // Set Velocity
            rigidBody.velocity = new Vector2(desiredVelocity.x, rigidBody.velocity.y);
        }
    }
}

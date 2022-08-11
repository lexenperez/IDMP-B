using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    // References
    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;
    private PlayerWallSlide playerWallSlideScript;
    private PlayerMovement playerMovementScript;

    [Header("Script Configurations")]
    [SerializeField] private bool debug = false;
    [SerializeField] private LayerMask groundLayer;

    public enum CurrentJumpType
    {
        None,
        Ground,
        Wall
    };

    [Header("Jumping Configurations")]
    [SerializeField, Range(1f, 5f),      Tooltip("Maximum jump height")]                                       private float jumpHeight = 2f;
    [SerializeField, Range(0f, 2f),      Tooltip("Minimum jump height")]                                       private float minJumpHeight = 0.2f;
    [SerializeField,                     Tooltip("Terminal Velocity")]                                         private float maxFallSpeed = 10f;
    [SerializeField, Range(0f, 0.3f),    Tooltip("How far from ground should we cache your jump?")]            private float jumpBuffer = 0.15f;
    [SerializeField, Range(0f, 1f),    Tooltip("A buffer period allowing jump when off the wall")]             private float coyoteTime = 0.2f;

    [Header("Wall Jump Configurations")]
    [SerializeField, Range(0.5f, 5f),    Tooltip("Maximum wall jump height")]                                  private float wallJumpHeight = 1f;
    [SerializeField, Range(0.5f, 10f),   Tooltip("Maximum wall jump power")]                                   private float wallJumpPower = 1f;
    [SerializeField, Range(0.1f, 10f),   Tooltip("Maximum wall jump time")]                                    private float wallJumpTime = 0.5f;
    [SerializeField, Range(0.1f, 1f),    Tooltip("Reduces wall jump time when holding input key")]             private float wallJumpWithInputMultiplier = 0.5f;

    [Header("Jump States")] // Shown in inspector for debugging
    [SerializeField] protected bool onGround = false;
    [SerializeField] private bool desiredJump;
    [SerializeField] private bool pressingJump;
    [SerializeField] private bool currentlyJumping;
    [SerializeField] private CurrentJumpType currentJumpType = CurrentJumpType.None;

    // Private
    private bool limitPlayerMovement;
    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private float lastJumpPositionY = 0f;

    // Offset Raycasts for ground detection
    [SerializeField, Range(0.001f, 0.5f), Tooltip("Distance from bottom of collision box to ground layer")] private float groundLength = 0.05f;
    private float groundOffset;
    private Vector2 colliderOffset;

    // Get - Setters
    public bool GetLimitPlayerMovement { get { return limitPlayerMovement; } }
    public CurrentJumpType GetCurrentJumpType { get { return currentJumpType; } }
    public bool IsPressingJump { get { return pressingJump; } }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerWallSlideScript = GetComponent<PlayerWallSlide>();
        playerMovementScript = GetComponent<PlayerMovement>();

        colliderOffset = new Vector2(boxCollider.bounds.size.x / 2f, 0);
        groundOffset = (boxCollider.bounds.size.y / 2f);
    }

    // Update is called once per frame
    void Update()
    {
        IsGrounded();
        JumpBuffer();

        // Player not touching walls and is in the air
        if (!(playerWallSlideScript.IsTouchingLeftWall || playerWallSlideScript.IsTouchingRightWall) && !onGround)
        {
            coyoteTimeCounter += Time.deltaTime;
        }
        else
        {
            coyoteTimeCounter = 0;
        }
    }

    private void JumpBuffer()
    {
        if (jumpBuffer > 0 && desiredJump)
        {
            jumpBufferCounter += Time.deltaTime;

            if (jumpBufferCounter > jumpBuffer)
            {
                desiredJump = false;
                jumpBufferCounter = 0;
            }
        }
    }

    private void IsGrounded()
    {
        // Shoots 2 raycasts downwards on either side of the collision box of the player
        onGround = Physics2D.Raycast((Vector2) transform.position - colliderOffset, Vector2.down, groundLength + groundOffset, groundLayer) ||
                   Physics2D.Raycast((Vector2) transform.position + colliderOffset, Vector2.down, groundLength + groundOffset, groundLayer);

        if (onGround)
            playerWallSlideScript.PrevTouchState = PlayerWallSlide.TouchState.Ground;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            desiredJump = true;
            pressingJump = true;
        }

        if (context.canceled)
            pressingJump = false;
    }

    private void FixedUpdate()
    {
        if (desiredJump)
            Jump();
        else
            CalculateJump();
    }

    private void CalculateJump()
    {
        if (onGround)
        {
            // Not moving vertically and is on ground
            if (rigidBody.velocity.y == 0f)
            {
                currentlyJumping = false;
                currentJumpType = CurrentJumpType.None;
            }
        }
        else
        {
            if (rigidBody.velocity.y > 0.01f)
            {
                // Variable Jump Height
                if (!pressingJump || !currentlyJumping)
                {
                    // Forces player to jump a minimum distance before dropping down
                    if (Mathf.Abs(transform.position.y - lastJumpPositionY) >= minJumpHeight)
                    {
                        // Causes player to quickly fall down
                        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
                        currentJumpType = CurrentJumpType.None;
                    }
                }
            }
        }

        // Limit the speed the player will fall
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, Mathf.Clamp(rigidBody.velocity.y, -maxFallSpeed, 100));
    }

    private void Jump()
    {
        // Normal Jump
        if (onGround || coyoteTimeCounter < coyoteTime)
        {
            // Set jump settings
            desiredJump = false;
            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;
            lastJumpPositionY = transform.position.y;
            currentJumpType = CurrentJumpType.Ground;

            // Calculate jump power
            float jumpVelocity = Mathf.Sqrt(-2f * Physics2D.gravity.y * rigidBody.gravityScale * jumpHeight);

            // Apply jump velocity
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpVelocity);
            currentlyJumping = true;
        }

        // Wall Jump
        if (!limitPlayerMovement && !onGround && playerWallSlideScript.PrevTouchState != PlayerWallSlide.TouchState.Ground)
            if (playerWallSlideScript.IsTouchingLeftWall || playerWallSlideScript.IsTouchingRightWall || coyoteTimeCounter < coyoteTime)
            {
                // Set wall jump settings
                desiredJump = false;
                jumpBufferCounter = 0;
                coyoteTimeCounter = 0;
                lastJumpPositionY = transform.position.y;
                currentJumpType = CurrentJumpType.Wall;
                playerWallSlideScript.CanSlide = false;

                StartCoroutine(WallJump());
            }

        if (jumpBuffer == 0)
            desiredJump = false;
    }

    private IEnumerator WallJump()
    {
        // Prevent player from moving horizontally when wall jumping
        limitPlayerMovement = true;

        // Calculate jump power
        float jumpVelocityY = Mathf.Sqrt(-2f * Physics2D.gravity.y * rigidBody.gravityScale * wallJumpHeight);

        // Calculate wall jump power
        float jumpVelocityX = wallJumpPower;
        if (playerWallSlideScript.PrevTouchState == PlayerWallSlide.TouchState.RightWall)
            jumpVelocityX = -jumpVelocityX;

        // Apply jump velocity
        rigidBody.velocity = new Vector2(jumpVelocityX, jumpVelocityY);
        currentlyJumping = true;

        // How long wall jump should last for. If holding input key towards wall then reduce wall jump time. (Allows to scale up the wall)
        if (playerWallSlideScript.IsTouchingLeftWall && playerMovementScript.GetDirectionX == -1 || 
            playerWallSlideScript.IsTouchingRightWall && playerMovementScript.GetDirectionX == 1)
            yield return new WaitForSeconds(wallJumpTime * wallJumpWithInputMultiplier);
        else
            yield return new WaitForSeconds(wallJumpTime);

        // Wall jump finish, enable player movement
        currentJumpType = CurrentJumpType.None;
        limitPlayerMovement = false;
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine((Vector2) transform.position - colliderOffset, (Vector2) transform.position - colliderOffset + Vector2.down * (groundLength + groundOffset));
            Gizmos.DrawLine((Vector2) transform.position + colliderOffset, (Vector2) transform.position + colliderOffset + Vector2.down * (groundLength + groundOffset));
        }
    }
}

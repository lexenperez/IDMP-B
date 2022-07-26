using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;
    private PlayerWallSlide playerWallSlideScript;
    private PlayerMovement playerMovementScript;
    private Animator animator;
    [SerializeField] private GameObject jumpParticle;
    [SerializeField] private GameObject landParticle;

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
    [SerializeField, Range(1f, 10f),      Tooltip("Maximum jump height")]                                       private float jumpHeight = 2f;
    [SerializeField, Range(0f, 2f),      Tooltip("Minimum jump height")]                                       private float minJumpHeight = 0.2f;
    [SerializeField,                     Tooltip("Terminal Velocity")]                                         private float maxFallSpeed = 10f;
    [SerializeField, Range(0f, 0.3f),    Tooltip("How far from ground should we cache your jump?")]            private float jumpBuffer = 0.15f;
    [SerializeField, Range(0f, 1f),    Tooltip("A buffer period allowing jump when off the ground")]           private float groundCoyoteTime = 0.2f;
    [SerializeField, Range(0f, 1f),    Tooltip("A buffer period allowing jump when off the wall")]             private float wallCoyoteTime = 0.2f;

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
    private float groundCoyoteTimeCounter;
    private float wallCoyoteTimeCounter;
    private float lastJumpPositionY = 0f;

    // Offset Raycasts for ground detection
    [SerializeField, Range(0.001f, 0.5f), Tooltip("Distance from bottom of collision box to ground layer")] private float groundSize = 0.05f;
    private float groundOffset;
    private Vector2 colliderOffset;

    // Get - Setters
    public bool GetLimitPlayerMovement { get { return limitPlayerMovement; } }
    public bool IsOnGround { get { return onGround; } }
    public CurrentJumpType GetCurrentJumpType { get { return currentJumpType; } }
    public bool IsPressingJump { get { return pressingJump; } }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerWallSlideScript = GetComponent<PlayerWallSlide>();
        playerMovementScript = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();

        colliderOffset = new Vector2(boxCollider.bounds.size.x / 2f, 0);
        groundOffset = (boxCollider.bounds.size.y / 2f);
    }

    // Update is called once per frame
    void Update()
    {
        IsGrounded();
        JumpBuffer();
        CalculateCoyoteTime();
    }

    private void CalculateCoyoteTime()
    {
        // Is player touching walls?
        if (!(playerWallSlideScript.CanLeftWallSlide || playerWallSlideScript.CanRightWallSlide))
            wallCoyoteTimeCounter += Time.deltaTime;
        else
            wallCoyoteTimeCounter = 0;

        // Is player in the air?
        if (!onGround)
            groundCoyoteTimeCounter += Time.deltaTime;
        else
            groundCoyoteTimeCounter = 0;
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
        // Uses an overlap box to detect ground
        onGround = Physics2D.OverlapBox((Vector2)transform.position - new Vector2(0, groundOffset), new Vector2(boxCollider.bounds.size.x - 0.05f, groundSize), 0, groundLayer);

        if (onGround)
            playerWallSlideScript.PrevTouchState = PlayerWallSlide.TouchState.Ground;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && !PauseMenu.gameIsPaused)
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
                if (animator.GetBool("isFalling"))
                {
                    currentlyJumping = false;
                    currentJumpType = CurrentJumpType.None;
                    animator.SetBool("isFalling", false);
                    // Play Particle
                    Instantiate(landParticle, transform.position - new Vector3(0, groundOffset), landParticle.transform.rotation);
                }
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
            else if (rigidBody.velocity.y < 0.01f)
            {
                // Is player animation not falling? - Prevent triggering animation multiple times
                if (!animator.GetBool("isFalling"))
                {
                    // Velocity going down
                    animator.SetTrigger("fall");
                    animator.SetBool("isFalling", true);
                }
            }
        }

        // Limit the speed the player will fall
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, Mathf.Clamp(rigidBody.velocity.y, -maxFallSpeed, 100));
    }

    private void Jump()
    {
        // Normal Jump
        if (onGround || groundCoyoteTimeCounter < groundCoyoteTime)
        {
            // Set jump settings
            desiredJump = false;
            jumpBufferCounter = 0;
            groundCoyoteTimeCounter = 0;
            lastJumpPositionY = transform.position.y;
            currentJumpType = CurrentJumpType.Ground;

            // Animate Player
            animator.SetBool("isFalling", false);
            animator.SetTrigger("takeOff");

            // Play Particle
            Instantiate(jumpParticle, transform.position - new Vector3(0, groundOffset), jumpParticle.transform.rotation);

            // Calculate jump power
            float jumpVelocity = Mathf.Sqrt(-2f * Physics2D.gravity.y * rigidBody.gravityScale * jumpHeight);

            // Apply jump velocity
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpVelocity);
            currentlyJumping = true;
        }

        // Wall Jump
        if (!limitPlayerMovement && !onGround && playerWallSlideScript.PrevTouchState != PlayerWallSlide.TouchState.Ground)
            if (playerWallSlideScript.CanLeftWallSlide || playerWallSlideScript.CanRightWallSlide || wallCoyoteTimeCounter < wallCoyoteTime)
            {
                // Set wall jump settings
                desiredJump = false;
                jumpBufferCounter = 0;
                wallCoyoteTimeCounter = 0;
                lastJumpPositionY = transform.position.y;
                currentJumpType = CurrentJumpType.Wall;
                playerWallSlideScript.CanSlide = false;
                animator.SetBool("isFalling", false);
                animator.SetTrigger("takeOff");

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
        if (playerWallSlideScript.CanLeftWallSlide && playerMovementScript.GetDirectionX == -1 || 
            playerWallSlideScript.CanRightWallSlide && playerMovementScript.GetDirectionX == 1)
            yield return new WaitForSeconds(wallJumpTime * wallJumpWithInputMultiplier);
        else
            yield return new WaitForSeconds(wallJumpTime);

        // Wall jump finish, enable player movement
        currentJumpType = CurrentJumpType.None;
        limitPlayerMovement = false;
    }

    private void OnDrawGizmos()
    {
        if (debug && boxCollider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube((Vector2)transform.position - new Vector2(0, groundOffset), new Vector2(boxCollider.bounds.size.x, groundSize));
        }
    }
}

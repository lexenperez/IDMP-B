using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlide : MonoBehaviour
{
    public enum TouchState
    {
        Ground,
        LeftWall,
        RightWall
    };

    // References
    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;

    [Header("Script Configurations")]
    [SerializeField] private bool debug = false;
    [SerializeField] private LayerMask groundLayer;

    [Header("Wall Slide Configurations")]
    [SerializeField, Range(0.01f, 5f), Tooltip("Maximum wall slide speed")] private float slideSpeed = 0.5f;

    [Header("Wall States")]
    [SerializeField] private bool touchLeftWall = false;
    [SerializeField] private bool touchRightWall = false;
    [SerializeField] private TouchState previousTouchState = TouchState.Ground;
    [SerializeField] private bool canSlide = true;

    // Raycast offset
    [SerializeField, Range(0.001f, 0.5f), Tooltip("Distance from side of collision box to the wall")] private float sideLength = 0.05f;
    [SerializeField, Range(-0.5f, 0.5f), Tooltip("Offset the y position of the wall raycasts")] private float heightOffset = 0.05f;
    private Vector2 leftColliderOffset;
    private Vector2 rightColliderOffset;

    // Get - Setters
    public bool IsTouchingLeftWall { get { return touchLeftWall; } }
    public bool IsTouchingRightWall { get { return touchRightWall; } }
    public TouchState PrevTouchState
    { 
        get { return previousTouchState; } 
        set { previousTouchState = value; }
    }

    public bool CanSlide { get { return canSlide; }  set { canSlide = value; } }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        leftColliderOffset = new Vector2(-boxCollider.bounds.size.x / 2f, heightOffset);
        rightColliderOffset = new Vector2(boxCollider.bounds.size.x / 2f, heightOffset);
    }

    // Update is called once per frame
    void Update()
    {
        IsTouchingWall();
    }

    private void IsTouchingWall()
    {
        touchLeftWall = Physics2D.Raycast((Vector2)transform.position + leftColliderOffset, Vector2.left, sideLength, groundLayer);
        touchRightWall = Physics2D.Raycast((Vector2)transform.position + rightColliderOffset, Vector2.right, sideLength, groundLayer);

        if (touchLeftWall)
            previousTouchState = TouchState.LeftWall;
        else if (touchRightWall)
            previousTouchState = TouchState.RightWall;
    }

    private void FixedUpdate()
    {
        WallSlide();
    }

    private void WallSlide()
    {
        // If going down and touching a wall. Then slide down.
        if (touchLeftWall || touchRightWall)
        {
            if (canSlide && rigidBody.velocity.y < -0.01f)
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, -slideSpeed);
        }

        // Allows player to jump, not overriding the jump velocity with slideSpeed.
        if (!touchLeftWall && !touchRightWall)
            canSlide = true;
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine((Vector2)transform.position + leftColliderOffset, (Vector2)transform.position + leftColliderOffset + Vector2.left * sideLength);
            Gizmos.DrawLine((Vector2)transform.position + rightColliderOffset, (Vector2)transform.position + rightColliderOffset + Vector2.right * sideLength);
        }
    }
}

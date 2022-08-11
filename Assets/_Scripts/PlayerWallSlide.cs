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
    [SerializeField] private bool leftWallSlide = false;
    [SerializeField] private bool rightWallSlide = false;
    [SerializeField] private bool touchingLeftWall = false;
    [SerializeField] private bool touchingRightWall = false;
    [SerializeField] private TouchState previousTouchState = TouchState.Ground;
    [SerializeField] private bool canSlide = true;

    // Raycast offset
    [SerializeField, Range(0.001f, 0.5f), Tooltip("Distance from side of collision box to the wall")] private float sideLength = 0.05f;
    [SerializeField, Range(-0.5f, 0.5f), Tooltip("Offset the y position of the wall raycasts")] private float heightOffset = 0.05f;
    private Vector2 leftRaycastOffset;
    private Vector2 rightRaycastOffset;
    private Vector2 leftBoxOffset;
    private Vector2 rightBoxOffset;

    // Get - Setters
    public bool CanLeftWallSlide { get { return leftWallSlide; } }
    public bool CanRightWallSlide { get { return rightWallSlide; } }
    public bool IsTouchingLeftWall { get { return touchingLeftWall; } }
    public bool IsTouchingRightWall { get { return touchingRightWall; } }
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

        leftRaycastOffset = new Vector2(-boxCollider.bounds.size.x / 2f, heightOffset);
        rightRaycastOffset = new Vector2(boxCollider.bounds.size.x / 2f, heightOffset);

        leftBoxOffset = new Vector2(-boxCollider.bounds.size.x / 2f, boxCollider.offset.y);
        rightBoxOffset = new Vector2(boxCollider.bounds.size.x / 2f, boxCollider.offset.y);
    }

    // Update is called once per frame
    void Update()
    {
        IsTouchingWall();
    }

    private void IsTouchingWall()
    {
        leftWallSlide = Physics2D.Raycast((Vector2)transform.position + leftRaycastOffset, Vector2.left, sideLength, groundLayer);
        rightWallSlide = Physics2D.Raycast((Vector2)transform.position + rightRaycastOffset, Vector2.right, sideLength, groundLayer);

        touchingLeftWall = Physics2D.OverlapBox((Vector2)transform.position + leftBoxOffset, new Vector2((sideLength - 0.05f) * 2, boxCollider.bounds.size.y), 0, groundLayer);
        touchingRightWall = Physics2D.OverlapBox((Vector2)transform.position + rightBoxOffset, new Vector2((sideLength - 0.05f) * 2, boxCollider.bounds.size.y), 0, groundLayer);

        if (leftWallSlide)
            previousTouchState = TouchState.LeftWall;
        else if (rightWallSlide)
            previousTouchState = TouchState.RightWall;
    }

    private void FixedUpdate()
    {
        WallSlide();
    }

    private void WallSlide()
    {
        // If going down and touching a wall. Then slide down.
        if (leftWallSlide || rightWallSlide)
        {
            if (canSlide && rigidBody.velocity.y < -0.01f)
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, -slideSpeed);
        }

        // Allows player to jump, not overriding the jump velocity with slideSpeed.
        if (!leftWallSlide && !rightWallSlide)
            canSlide = true;
    }

    private void OnDrawGizmos()
    {
        if (debug && Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine((Vector2)transform.position + leftRaycastOffset, (Vector2)transform.position + leftRaycastOffset + Vector2.left * sideLength);
            Gizmos.DrawLine((Vector2)transform.position + rightRaycastOffset, (Vector2)transform.position + rightRaycastOffset + Vector2.right * sideLength);

            Gizmos.DrawCube((Vector2)transform.position + leftBoxOffset, new Vector2((sideLength - 0.05f) * 2, boxCollider.bounds.size.y));
            Gizmos.DrawCube((Vector2)transform.position + rightBoxOffset, new Vector2((sideLength - 0.05f) * 2, boxCollider.bounds.size.y));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rigidBody;

    // Movement
    private float directionX;
    private Vector2 desiredVelocity;

    [Header("Configurations")]
    [SerializeField, Range(2f, 20f)] private float maxSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        directionX = context.ReadValue<float>();
    }

    private void MovePlayer()
    {
        // Get Input and Desired Velocity
        desiredVelocity = new Vector2(directionX, 0f) * maxSpeed;

        // Set Velocity
        rigidBody.velocity = new Vector2(desiredVelocity.x, rigidBody.velocity.y);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rigidBody;

    // Input
    private PlayerInput playerInput;
    private InputAction horizontalMovement;

    // Movement
    private float directionX;
    private Vector2 desiredVelocity;

    [Header("Configurations")]
    [SerializeField, Range(2f, 20f)] private float maxSpeed = 5f;


    private void OnEnable()
    {
        horizontalMovement.Enable();
    }

    private void OnDisable()
    {
        horizontalMovement.Disable();
    }
   
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        horizontalMovement = playerInput.actions["Move"];
    }

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

    private void MovePlayer()
    {
        // Get Input and Desired Velocity
        directionX = horizontalMovement.ReadValue<float>();
        desiredVelocity = new Vector2(directionX, 0f) * maxSpeed;

        // Set Velocity
        rigidBody.velocity = desiredVelocity;
    }
}

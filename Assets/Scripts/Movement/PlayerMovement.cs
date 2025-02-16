using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    private CustomRigidBody rb;

    [Header("Movement")]
    private float horizontalInput;
    private float verticalInput;
    Vector3 moveDirection;
    public float movementSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float swingSpeed;
    private bool readyToJump = true;
    [HideInInspector] public bool freeze;
    [HideInInspector] public bool activeGrapple;
    [HideInInspector] public bool swinging;
    private Vector3 velocityToSet;
    private bool enableMovement;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundLayer;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<CustomRigidBody>();
        rb.freezeRotation = true; // Freeze rotation to ensure player doesn't flip
        enableMovement = true;

    }

    private void Update()
    {
        
        // Ground Check using raycast
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

        MovementInput(); // Taking inputs
        Move();

        // Ground Drag logic
        if (isGrounded && !activeGrapple && !swinging)
        {
            rb.drag = groundDrag; // Apply ground drag when grounded
        }
        else
        {
            rb.drag = 0f; // No drag when in the air
        }

        // Freeze Player
        if (freeze)
        {
            rb.velocity = Vector3.zero; // Freeze movement if needed
        }
    }

    private void MovementInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jump input handling
        if (Input.GetKey(KeyCode.Space) && isGrounded && readyToJump)
        {
            
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown); // Cooldown before next jump
            readyToJump = false;  // Prevent multiple jumps
        }
    }

    private void Jump()
    {
        Debug.Log("Jump() called, isGrounded: " + isGrounded + ", readyToJump: " + readyToJump);

        if (isGrounded && readyToJump) // Ensure we're grounded before jumping
        {
            readyToJump = false;  // Prevent multiple jumps

            // Set velocity directly instead of ApplyForce()
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);

            Debug.Log("Jump applied with force: " + jumpForce);
            if(!isGrounded)
            {
                Invoke(nameof(ResetJump), jumpCooldown); // Reset jump after cooldown
            }
            
        }
    }

    private void ResetJump()
    {
        
        
        readyToJump = true; // Allow jumping again
        
    }

    private void Move()
    {
        if (!enableMovement) return;
        if (activeGrapple || swinging) return; // Skip movement while grappling or swinging

        // Calculate movement direction based on input
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (moveDirection.magnitude > 0) // If there is any movement input, apply force
        {
            // Apply movement force with air multiplier if in the air, or normal force if grounded
            if (isGrounded)
            {
                rb.ApplyForce(moveDirection.normalized * movementSpeed * 10f, ForceMode.Force);
                Debug.Log("Player grounded, applying force: " + moveDirection.normalized * movementSpeed * 10f);
            }
            else
            {
                rb.ApplyForce(moveDirection.normalized * movementSpeed * 10f * airMultiplier, ForceMode.Force);
                Debug.Log("Player in air, applying force: " + moveDirection.normalized * movementSpeed * 10f * airMultiplier);
            }
        }
    }

   private void OnCollisionEnter(Collision collision)
    {
        // Ensure player stays grounded after collision with the ground
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true; // Ensure player is grounded
            readyToJump = true; // Allow jumping again once grounded
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Ensure player is no longer grounded when they exit the ground
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false;
        }
    }


    /// <summary>
    /// Initiates a jump towards the target position following a parabolic trajectory.
    /// </summary>
    /// <param name="targetPosition">The target position</param>
    /// <param name="trajectoryHeight">The height of the arc</param>
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
    }

    /// <summary>
    /// Sets the velocity of the rigidbody to the precalculated jump velocity.
    /// </summary>
    private void SetVelocity()
    {
        enableMovement = true;
        rb.velocity = velocityToSet;
    }

    // Apply force by using parabolic trajectory formula
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y; // Get gravity
        float displacementY = endPoint.y - startPoint.y; // Calculate vertical displacement between player and grapple point
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z); // Calculate horizontal displacement between player and grapple point

        // Use v^2 = u^2 + 2as (0 = u upward ^2 + (2 * gravity * height)) to calculate the upward force (u upward)
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);

        /* Use s = ut + (at^2) / 2 (displacement = u horizontal * (time up + time down) + (0 * (time up + time down)^2) / 2)
        *  to calculate the horizontal force (u horizontal)
        */
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

   
}

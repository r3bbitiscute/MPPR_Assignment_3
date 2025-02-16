using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRigidBody : MonoBehaviour
{
    public Vector3 acceleration;
    public Vector3 velocity;
    public float mass = 1f;

    public bool useGravity = true;
    public bool isGrounded = false;
    public bool freezeRotation = true;
    public float drag = 0.1f; // Added drag for smoother stopping
    public LayerMask groundLayer; // Layer mask for proper ground detection

    private Vector3 gravity = new Vector3(0,-9.81f, 0);
    public float groundCheckDistance = 0.2f; // Distance for ground detection
    private Quaternion initialRotation;
    public float dropSpeed = 3.5f;

    private void Start()
    {
        initialRotation = transform.rotation;

    }
    private void FixedUpdate()
    {
        // Ground check using raycast
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        // Apply gravity only when not grounded
        if (useGravity && !isGrounded && velocity.y > 0f)
        {
            ApplyForce(gravity * mass* dropSpeed);
        }

        // Apply acceleration to velocity
        velocity += acceleration * Time.fixedDeltaTime;

        // Apply drag (reduces velocity over time)
        ApplyDrag();

        // Apply movement
        transform.position += velocity * Time.fixedDeltaTime;

        // Reset acceleration
        acceleration = Vector3.zero;

        if (freezeRotation)
        {
            transform.rotation = initialRotation;
        }
    }

    private void ApplyDrag()
    {
        // Apply drag on velocity for smooth stopping
        if (isGrounded)
        {
            // Reduce both X, Z and Y velocities to slow down when grounded
            velocity.x *= 1f / (1f + drag * Time.fixedDeltaTime);
            velocity.z *= 1f / (1f + drag * Time.fixedDeltaTime);
            // On the ground, do not reset Y velocity to 0 immediately
            if (velocity.y < 0f) // If falling, apply drag but don't set to zero
            {
                velocity.y *= 0.2f; // Soft landing
            }
        }
        else
        {
            // Air drag applies only to horizontal movement
            velocity.x *= 1f / (1f + drag * Time.fixedDeltaTime);
            velocity.z *= 1f / (1f + drag * Time.fixedDeltaTime);
        }
    }
    public void ApplyForce(Vector3 force,ForceMode mode = ForceMode.Force)
    {
        if (mode == ForceMode.Force) 
        {
            acceleration += force / mass; // Normal continuous force
        }
        else if (mode == ForceMode.Impulse)
        {
            velocity += force / mass; //Instant velocity change
        }
        
    }

    public void ApplyImpulse(Vector3 impulse)
    {
        velocity += impulse / mass;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Soft landing instead of instantly setting velocity to zero
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;

            // Do not reset Y velocity to zero, just ensure it doesn't keep falling
            if (velocity.y < 0f) // When falling, apply soft landing (optional)
            {
                velocity.y *= 0.2f; // Or leave as zero depending on how you want the player to land
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false;
        }
    }
}

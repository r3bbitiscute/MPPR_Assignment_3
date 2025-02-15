using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSpringJoint : MonoBehaviour
{
    public Transform connectedBody;  // The object we're "attached" to
    public CustomRigidBody rb;       // Reference to our custom Rigidbody
    public float springStrength = 4.5f;
    public float damping = 7f;
    public float restLength = 5f;    // Desired length of the "rope"

    private void FixedUpdate()
    {
        if (rb == null || connectedBody == null)
            return;

        // Direction and current distance from anchor
        Vector3 direction = connectedBody.position - transform.position;
        float currentLength = direction.magnitude;

        // Hooke's Law: F = -k(x - restLength) - damping * velocity
        float stretch = currentLength - restLength;
        Vector3 force = direction.normalized * (-springStrength * stretch);

        // Apply damping
        Vector3 relativeVelocity = rb.velocity;  // Assuming `velocity` exists in CustomRigidbody
        force -= damping * relativeVelocity;

        // Apply force to our rigidbody
        rb.ApplyForce(force);
    }
}

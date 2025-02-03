using System.Collections;
using UnityEngine;

public class GrapplingRope : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lineRenderer;
    private Vector3 currentGrapplePosition;
    private Spring springScript;

    public Grappling grapplingScript;
    public int ropeSegments;
    public float damper;
    public float strength;
    public float velocity;
    public float waveCount;
    public float waveHeight;
    public AnimationCurve waveCurve;

    
    private void Awake()
    {
        lineRenderer.GetComponent<LineRenderer>();
        springScript = new Spring(); // Instantiate a new Spring object for simulating rope movement
        springScript.SetTarget(0); // Set the spring's target position to 0
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void DrawRope()
    {
        // If grappling is not active, reset the rope and remove any line renderer positions
        if (!grapplingScript.grappling)
        {
            currentGrapplePosition = grapplingScript.gunTip.position; // Set the current grapple position to gun tip
            springScript.Reset(); // Reset the spring physics simulation
            if (lineRenderer.positionCount > 0)
            {
                lineRenderer.positionCount = 0; // Clear the line renderer if no grappling occurs
            }
            return;
        }

        // If this is the first time drawing, initialize the rope segment count and velocity
        if (lineRenderer.positionCount == 0)
        {
            if (ropeSegments <= 0)
            {
                ropeSegments = 1; // Ensure there is at least one segment in the rope
            }
            springScript.SetVelocity(velocity); // Set the velocity of the spring-based rope simulation
            lineRenderer.positionCount = ropeSegments + 1; // Set the number of points in the line renderer
        }

        // Set up spring parameters
        springScript.SetDamper(damper);
        springScript.SetStrength(strength);
        springScript.Update(Time.deltaTime); // Update the spring simulation with time delta

        // Get the grapple point and gun tip position from the grappling script
        var grapplePoint = grapplingScript.grapplePoint;
        var gunTipPosition = grapplingScript.gunTip.position;
        var up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up; // Calculate the up direction based on the grapple

        // Linear interpolate the current grapple position towards the grapple point based on spring simulation
        currentGrapplePosition += (grapplePoint - currentGrapplePosition) * Time.deltaTime * 12f;

        // Loop through each segment of the rope and position the line renderer points
        for (var i = 0; i < ropeSegments + 1; i++)
        {
            // Calculate the fraction of the rope's total length for each segment
            var delta = i / (float)ropeSegments;

            // Calculate the offset for wave-like behavior based on sine function
            // Apply wave height, wave count, and the spring value modulated by the wave curve
            var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * springScript.Value * waveCurve.Evaluate(delta);

            // We calculate the exact position by adding the offset to the gun tip to current grapple position (Linear Interpolation)
            Vector3 segmentPosition = gunTipPosition + (currentGrapplePosition - gunTipPosition) * delta + offset;

            // Set the position for the current segment in the line renderer
            lineRenderer.SetPosition(i, segmentPosition);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingDone : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lr;
    public Transform leftGunTip, rightGunTip, cam, player;
    public LayerMask whatIsGrappleable;
    private PlayerMovement pm;

    [Header("Swinging")]
    private float maxSwingDistance = 25f;
    private Vector3 leftSwingPoint, rightSwingPoint;
    private CustomSpringJoint leftJoint, rightJoint;

    [Header("SwingingBoostMovement")]
    public Transform orientation;
    public CustomRigidBody rb;
    public float horizontalThrustForce;
    public float forwardThrustForce;
    public float extendCableSpeed;

    [Header("Prediction")]
    public RaycastHit predictionHit;
    public float predictionSphereCastRadius;
    public Transform predictionPointRight;
    public Transform predictionPointLeft;

    [Header("Speed")]
    private bool inSpeedBlockSlow = false;
    private bool inSpeedBlockFast = false;
    public float slowSpeedMultipliyer = 0.5f;
    public float fastSpeedMultipliyer = 2.0f;

    /// <summary>
    /// Gets the player movement
    /// </summary>
    private void Start()
    {
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<CustomRigidBody>();
    }
    private void Update()
    {
        // Start swinging with the left hand (left click)
        if (Input.GetMouseButtonDown(0)) StartSwing(true);  // Left click (left hand)

        // Start swinging with the right hand (right click)
        if (Input.GetMouseButtonDown(1)) StartSwing(false); // Right click (right hand)

        // Stop swinging for the left hand (left click release)
        if (Input.GetMouseButtonUp(0)) StopSwing(true);  // Left click release (left hand)

        // Stop swinging for the right hand (right click release)
        if (Input.GetMouseButtonUp(1)) StopSwing(false); // Right click release (right hand)

        // Check for swing points, handle boost and other movements
        CheckForSwingPoints();

        if (leftJoint != null) SwingingBoostMovement(true);
        if (rightJoint != null) SwingingBoostMovement(false);
    }

    private void LateUpdate()
    {
        // Draw the rope (update line renderer positions) based on the hand
        if (leftJoint)
            DrawRope(leftGunTip.position, leftSwingPoint);

        if (rightJoint)
            DrawRope(rightGunTip.position, rightSwingPoint);
    }
    private void CheckForSwingPoints()
    {
        // Check swing points for the left hand (if left joint is not already set)
        if (leftJoint == null)
        {
            CheckForLeftHandSwingPoint();
        }

        // Check swing points for the right hand (if right joint is not already set)
        if (rightJoint == null)
        {
            CheckForRightHandSwingPoint();
        }
    }
    private void CheckForLeftHandSwingPoint()
    {
        RaycastHit sphereCastHit;
        Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward,
                            out sphereCastHit, maxSwingDistance, whatIsGrappleable);

        RaycastHit raycastHit;
        Physics.Raycast(cam.position, cam.forward,
                            out raycastHit, maxSwingDistance, whatIsGrappleable);

        Vector3 realHitPoint;

        // Option 1 - Direct Hit
        if (raycastHit.point != Vector3.zero)
            realHitPoint = raycastHit.point;

        // Option 2 - Indirect (predicted) Hit
        else if (sphereCastHit.point != Vector3.zero)
            realHitPoint = sphereCastHit.point;

        // Option 3 - Miss
        else
            realHitPoint = Vector3.zero;

        // If a valid swing point is found
        if (realHitPoint != Vector3.zero)
        {
            leftSwingPoint = realHitPoint;  // Store the left hand swing point
            predictionPointLeft.gameObject.SetActive(true);
            predictionPointLeft.position = realHitPoint;
        }
        else
        {
            predictionPointLeft.gameObject.SetActive(false);
        }
    }

    // Check for right hand swing point
    private void CheckForRightHandSwingPoint()
    {
        RaycastHit sphereCastHit;
        Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward,
                            out sphereCastHit, maxSwingDistance, whatIsGrappleable);

        RaycastHit raycastHit;
        Physics.Raycast(cam.position, cam.forward,
                            out raycastHit, maxSwingDistance, whatIsGrappleable);

        Vector3 realHitPoint;

        // Option 1 - Direct Hit
        if (raycastHit.point != Vector3.zero)
            realHitPoint = raycastHit.point;

        // Option 2 - Indirect (predicted) Hit
        else if (sphereCastHit.point != Vector3.zero)
            realHitPoint = sphereCastHit.point;

        // Option 3 - Miss
        else
            realHitPoint = Vector3.zero;

        // If a valid swing point is found
        if (realHitPoint != Vector3.zero)
        {
            rightSwingPoint = realHitPoint;  // Store the right hand swing point
            predictionPointRight.gameObject.SetActive(true);
            predictionPointRight.position = realHitPoint;
        }
        else
        {
            predictionPointRight.gameObject.SetActive(false);
        }
    }


    private void StartSwing(bool isLeftHand)
    {
        RaycastHit hit;
        Transform gunTip = isLeftHand ? leftGunTip : rightGunTip;
        Vector3 swingPoint = isLeftHand ? leftSwingPoint : rightSwingPoint;

        // Do the raycasting and prediction logic for each hand
        Physics.Raycast(gunTip.position, cam.forward, out hit, maxSwingDistance, whatIsGrappleable);

        if (hit.point != Vector3.zero)
        {
            // If a swing point is found, start swinging
            if (isLeftHand)
                leftSwingPoint = hit.point;
            else
                rightSwingPoint = hit.point;

            // Create new spring joint for the respective hand
            CustomSpringJoint joint = isLeftHand ? leftJoint : rightJoint;
            if (joint == null)
            {
                joint = player.gameObject.AddComponent<CustomSpringJoint>();
            }

            joint.connectedBody = hit.transform;
            joint.rb = player.GetComponent<CustomRigidBody>();
            joint.restLength = Vector3.Distance(player.position, hit.point);
            joint.springStrength = 4.5f;
            joint.damping = 7f;

            // Update the joint for the correct hand
            if (isLeftHand)
                leftJoint = joint;
            else
                rightJoint = joint;
        }
    }
    private void StopSwing(bool isLeftHand)
    {
        if (isLeftHand && leftJoint != null)
        {
            Destroy(leftJoint);
            leftJoint = null;
        }
        else if (rightJoint != null)
        {
            Destroy(rightJoint);
            rightJoint = null;
        }

        // Set rope line renderer to 0 points if no swinging
        lr.positionCount = 0;
    }

    private void SwingingBoostMovement(bool isLeftHand)
    {
        // Apply force to the character from the respective hand (left or right)
        CustomSpringJoint joint = isLeftHand ? leftJoint : rightJoint;
        Vector3 swingPoint = isLeftHand ? leftSwingPoint : rightSwingPoint;

        // Apply forces, movement logic, etc., specific to the hand
        if (Input.GetKey(KeyCode.D)) rb.ApplyForce(orientation.right * horizontalThrustForce * Time.deltaTime);
        if (Input.GetKey(KeyCode.A)) rb.ApplyForce(-orientation.right * horizontalThrustForce * Time.deltaTime);
        if (Input.GetKey(KeyCode.W)) rb.ApplyForce(orientation.forward * horizontalThrustForce * Time.deltaTime);

        // Handle shortening or extending cable, etc.
    }


    private Vector3 currentGrapplePosition;

    private void DrawRope(Vector3 gunTipPosition, Vector3 swingPoint)
    {
        // if not grappling, don't draw rope
        if (!leftJoint && ! rightJoint) return;

        currentGrapplePosition =
            Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTipPosition);
        lr.SetPosition(1, currentGrapplePosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SlowBlock"))
        {
            inSpeedBlockFast = true;
        }
        if (other.CompareTag("FastBlock"))
        {
            inSpeedBlockFast = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SlowBlock"))
        {
            inSpeedBlockFast = false;
        }
        if (other.CompareTag("FastBlock"))
        {
            inSpeedBlockFast = false;
        }
    }
}

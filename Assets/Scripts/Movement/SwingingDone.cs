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
    private SpringJoint leftJoint, rightJoint;

    [Header("Swinging Boost Movement")]
    public Transform orientation;
    public CustomRigidBody rb; // Using Custom Rigidbody
    public float horizontalThrustForce = 500f;
    public float forwardThrustForce = 500f;
    public float extendCableSpeed = 5f;

    [Header("Prediction")]
    public float predictionSphereCastRadius = 2f;
    public Transform predictionPointRight;
    public Transform predictionPointLeft;

    [Header("Speed Modifiers")]
    private bool inSpeedBlockSlow = false;
    private bool inSpeedBlockFast = false;
    public float slowSpeedMultiplier = 0.5f;
    public float fastSpeedMultiplier = 2.0f;

    private void Start()
    {
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<CustomRigidBody>(); // Assign Custom Rigidbody
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) StartSwing(true);  // Left click (left hand)
        if (Input.GetMouseButtonDown(1)) StartSwing(false); // Right click (right hand)

        if (Input.GetMouseButtonUp(0)) StopSwing(true);
        if (Input.GetMouseButtonUp(1)) StopSwing(false);

        CheckForSwingPoints();

        if (leftJoint != null) SwingingBoostMovement(true);
        if (rightJoint != null) SwingingBoostMovement(false);
    }

    private void LateUpdate()
    {
        if (leftJoint)
            DrawRope(leftGunTip.position, leftSwingPoint);

        if (rightJoint)
            DrawRope(rightGunTip.position, rightSwingPoint);
    }

    private void CheckForSwingPoints()
    {
        if (leftJoint == null) CheckForLeftHandSwingPoint();
        if (rightJoint == null) CheckForRightHandSwingPoint();
    }

    private void CheckForLeftHandSwingPoint()
    {
        RaycastHit hit;
        if (Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, out hit, maxSwingDistance, whatIsGrappleable))
        {
            leftSwingPoint = hit.point;
            predictionPointLeft.gameObject.SetActive(true);
            predictionPointLeft.position = hit.point;
        }
        else
        {
            predictionPointLeft.gameObject.SetActive(false);
        }
    }

    private void CheckForRightHandSwingPoint()
    {
        RaycastHit hit;
        if (Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, out hit, maxSwingDistance, whatIsGrappleable))
        {
            rightSwingPoint = hit.point;
            predictionPointRight.gameObject.SetActive(true);
            predictionPointRight.position = hit.point;
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

        if (Physics.Raycast(gunTip.position, cam.forward, out hit, maxSwingDistance, whatIsGrappleable))
        {
            if (isLeftHand)
                leftSwingPoint = hit.point;
            else
                rightSwingPoint = hit.point;

            SpringJoint joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = hit.point;

            float distanceFromPoint = Vector3.Distance(player.position, hit.point);

            joint.maxDistance = distanceFromPoint * 0.8f; // Shorten the rope slightly
            joint.minDistance = distanceFromPoint * 0.25f; // Allow some slack

            joint.spring = 4.5f;   // Adjust for more or less bounce
            joint.damper = 7f;     // Controls how much energy is lost (higher = more damping)
            joint.massScale = 4.5f; // Adjust mass scaling

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
        else if (!isLeftHand && rightJoint != null)
        {
            Destroy(rightJoint);
            rightJoint = null;
        }

        lr.positionCount = 0;
    }

    private void SwingingBoostMovement(bool isLeftHand)
    {
        SpringJoint joint = isLeftHand ? leftJoint : rightJoint;
        if (joint == null) return;

        Vector3 moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.D)) moveDir += orientation.right;
        if (Input.GetKey(KeyCode.A)) moveDir -= orientation.right;
        if (Input.GetKey(KeyCode.W)) moveDir += orientation.forward;

        // Use CustomRigidbody movement method
        rb.ApplyForce(moveDir.normalized * horizontalThrustForce * Time.deltaTime);

        // Adjust rope length for better control
        if (Input.GetKey(KeyCode.Space)) joint.maxDistance = Mathf.Max(joint.maxDistance - extendCableSpeed * Time.deltaTime, 2f);
        if (Input.GetKey(KeyCode.LeftShift)) joint.maxDistance = Mathf.Min(joint.maxDistance + extendCableSpeed * Time.deltaTime, maxSwingDistance);
    }

    private Vector3 currentGrapplePosition;

    private void DrawRope(Vector3 gunTipPosition, Vector3 swingPoint)
    {
        if (leftJoint == null && rightJoint == null) return;

        if (lr.positionCount < 2)
        {
            lr.positionCount = 2;
        }

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTipPosition);
        lr.SetPosition(1, currentGrapplePosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SlowBlock"))
        {
            inSpeedBlockSlow = true;
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
            inSpeedBlockSlow = false;
        }
        if (other.CompareTag("FastBlock"))
        {
            inSpeedBlockFast = false;
        }
    }
}


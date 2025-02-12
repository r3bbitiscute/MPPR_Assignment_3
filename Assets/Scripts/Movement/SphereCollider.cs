using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCollider : MonoBehaviour
{
    private float height;
    private float radius;

    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        CalculatingRadiusAndHeight();
    }

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, radius))
        {
            float heightOffset = height / 2f; // Half the height of the capsule
            Vector3 newPosition = hit.point + Vector3.up * heightOffset;

            // Update the position of the capsule to stand on top
            transform.position = newPosition;
        }
    }

    private void CalculatingRadiusAndHeight()
    {
        // Get the bounds of the capsule mesh
        Bounds bounds = meshRenderer.bounds;

        // Height is the full height (y-axis dimension) of the mesh bounds
        height = bounds.size.y;

        // Radius is roughly half of the width of the mesh bounds
        radius = bounds.size.x / 2f;
    }
}

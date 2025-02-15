using UnityEngine;

public class SphereCollider : MonoBehaviour
{
    private float radius;
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        CalculatingRadiusAndHeight();
    }

    private void Update()
    {
        Collision();
    }

    /// <summary>
    /// Calculating the radius of the player using the mesh renderer
    /// </summary>
    private void CalculatingRadiusAndHeight()
    {
        // Get the bounds of the mesh
        Bounds bounds = meshRenderer.bounds;

        // Radius is of a circle is half the width of the mesh bounds
        radius = bounds.size.x / 2f;
    }

    /// <summary>
    /// Use a sphere cast to check in every direction to see if there is a collision
    /// </summary>
    private void Collision()
    {
        Vector3[] directions = new Vector3[]
        {
            Vector3.right, Vector3.left, Vector3.forward, Vector3.back, Vector3.down, Vector3.up
        };

        // Perform a SphereCast check in each direction in the directions array
        foreach (var direction in directions)
        {
            RaycastHit hit;

            // Perform a SphereCast to check if there is any obstacles
            if (Physics.SphereCast(transform.position, radius, direction, out hit, radius))
            {
                // Calculate the penetration depth (This calculation helps prevents clipping)
                float penetrationDepth = radius - hit.distance;

                // Calculate the new position
                Vector3 newPosition = transform.position + (-direction * penetrationDepth);

                // Update the position of the sphere
                transform.position = newPosition;
            }
        }
    }
}
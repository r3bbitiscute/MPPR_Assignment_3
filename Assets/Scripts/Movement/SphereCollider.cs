using UnityEngine;
using System.Collections.Generic;

public class SphereCollider : MonoBehaviour
{
    private float radius;
    private MeshRenderer meshRenderer;

    // List to store object that could be collide with
    private List<Bounds> collideObjects = new List<Bounds>();

    void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        CalculateRadius();
        FindObstacles();
    }

    private void Update()
    {
        CheckCollisions();
    }

    /// <summary>
    /// Calculate the sphere's radius based on the bounds
    /// </summary>
    private void CalculateRadius()
    {
        Bounds bounds = meshRenderer.bounds;
        radius = bounds.size.x / 2f;
    }

    /// <summary>
    /// Find all Game Object with the tags "Wall" and "Floor" and assign their bounds into collideObjects for further calculation
    /// </summary>
    private void FindObstacles()
    {
        string[] objectTags = { "Wall", "Ground" };

        foreach (string tag in objectTags)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objects)
            {
                MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    collideObjects.Add(meshRenderer.bounds);
                }
            }
        }
    }

    /// <summary>
    /// Check for collisions with obstacles
    /// </summary>
    private void CheckCollisions()
    {
        // Looping through each object and see which points on the bounds are closest to player
        foreach (var obj in collideObjects)
        {
            Vector3 closestPoint = GetClosestPointOnBounds(transform.position, obj);
            float distance = Vector3.Distance(transform.position, closestPoint);

            // If inside obstacle, push sphere out
            if (distance < radius)
            {
                Vector3 pushDirection = (transform.position - closestPoint).normalized;
                float penetrationDepth = radius - distance;
                transform.position += pushDirection * penetrationDepth;
            }
        }
    }

    /// <summary>
    /// Get the closest point on a box (wall/floor) to a sphere
    /// </summary>
    private Vector3 GetClosestPointOnBounds(Vector3 centerPoint, Bounds bounds)
    {
        // Note: Bounds.min is the bottom left corner of the object, while Bounds.max is the top right corner of the object
        float clampedX = Mathf.Clamp(centerPoint.x, bounds.min.x, bounds.max.x);
        float clampedY = Mathf.Clamp(centerPoint.y, bounds.min.y, bounds.max.y);
        float clampedZ = Mathf.Clamp(centerPoint.z, bounds.min.z, bounds.max.z);
        return new Vector3(clampedX, clampedY, clampedZ);
    }
}

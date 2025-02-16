using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SphereCollider : MonoBehaviour
{
    private float radius;
    private MeshRenderer meshRenderer;
    public GameObject lava;
    public Lava lavascript;
    public PlayerMovement playerMovement;

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
        CheckLavaCollision();
        CheckWinCollision();
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
    /// Find all GameObjects with the tags "Wall" and "Ground" and assign their bounds into collideObjects for further calculation
    /// </summary>
    private void FindObstacles()
    {
        string[] objectTags = { "Wall", "Ground", "Lava", "WinGoal"};

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
            Vector3 closestPoint = GetNearestPointOnBounds(transform.position, obj);
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
    /// Check for collision with Lava
    /// </summary>
    private void CheckLavaCollision()
    {
        if (lava != null)
        {
            MeshRenderer lavaRenderer = lava.GetComponent<MeshRenderer>();
            if (lavaRenderer != null)
            {
                Bounds lavaBounds = lavaRenderer.bounds;
                Vector3 closestPoint = GetNearestPointOnBounds(transform.position, lavaBounds);
                float distance = Vector3.Distance(transform.position, closestPoint);

                if (distance < radius) // Use radius to ensure accurate collision
                {
                    playerMovement.health -= lavascript.lavaDamage;

                    if (playerMovement.health <= 0)
                    {
                        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
                        Cursor.visible = true;
                        SceneManager.LoadScene("GameOver");
                        Debug.Log("Game Over");
                    }
                    Debug.Log("Player health" + playerMovement.health);
                }
            }
        }
    }
    /// <summary>
    /// Check for collision with WinGoal
    /// </summary>
    private void CheckWinCollision()
    {
        GameObject winGoal = GameObject.FindGameObjectWithTag("WinGoal"); // ✅ Find the WinGoal object
        if (winGoal != null)
        {
            MeshRenderer winRenderer = winGoal.GetComponent<MeshRenderer>();
            if (winRenderer != null)
            {
                Bounds winBounds = winRenderer.bounds;
                Vector3 closestPoint = GetNearestPointOnBounds(transform.position, winBounds);
                float distance = Vector3.Distance(transform.position, closestPoint);

                if (distance < radius) // If player collides with win goal
                {
                    Cursor.lockState = CursorLockMode.None; // Unlock cursor
                    Cursor.visible = true;
                    SceneManager.LoadScene("WinScene");
                    Debug.Log("You Win!");
                }
            }
        }
    }

    /// <summary>
    /// Finds the closest point of the object's bounds according to the player position.
    /// <summary>
    private Vector3 GetNearestPointOnBounds(Vector3 playerPos, Bounds bounds)
    {
        // Note: Bounds.min is the bottom left corner of the object, while Bounds.max is the top right corner of the object
        float clampedX = Mathf.Clamp(playerPos.x, bounds.min.x, bounds.max.x);
        float clampedY = Mathf.Clamp(playerPos.y, bounds.min.y, bounds.max.y);
        float clampedZ = Mathf.Clamp(playerPos.z, bounds.min.z, bounds.max.z);
        return new Vector3(clampedX, clampedY, clampedZ);
    }
}

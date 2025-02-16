using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public float speed = 1f;
    private Vector3 initialScale;
    private float elapsedTime; // Tracks lava growth time
    public float lavaDamage = 10f; // Damage dealt to player when collided

    // Start is called before the first frame update
    void Start()
    {
        initialScale = transform.localScale; // Store the initial scale
        elapsedTime = 0f;
        ResetLava();
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        // Increase y value to increase lava height
        Vector3 newScale = transform.localScale;
        newScale.y = initialScale.y * (elapsedTime * speed);

        transform.localScale = newScale;
    }

    // Function to reset lava when the player respawns
    public void ResetLava()
    {
        transform.localScale = initialScale; // Reset to original scale
        elapsedTime = 0f; // Reset time to prevent continued growth
        Debug.Log("Lava reset.");
    }
}

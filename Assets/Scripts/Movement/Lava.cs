using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public float speed = 1f;
    private Vector3 initialScale;
    // Start is called before the first frame update
    void Start()
    {
        initialScale = transform.localScale; // Store the initial scale
    }

    // Update is called once per frame
    void Update()
    {
        // Increase y value to increase lava height
        Vector3 newScale = transform.localScale;
        newScale.y = initialScale.y * Time.time * speed;

        transform.localScale = newScale;
    }
}

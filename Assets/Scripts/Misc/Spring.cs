using UnityEngine;

public class Spring
{
    private float strength; // Stiffness of the spring
    private float damper; // Controls how much resistance the spring has
    private float target; // Target value the spring is trying to reach
    private float velocity; // Velocity of the spring
    private float value; // Current value/position of the spring

   
    public void Update(float deltaTime)
    {
        // Calculate the direction to the target
        var direction = target - value >= 0 ? 1f : -1f;

        // Calculate force applied based on the spring's stiffness
        var force = Mathf.Abs(target - value) * strength;

        // Update velocity using Hooke's Law for spring force and damping
        velocity += (force * direction - velocity * damper) * deltaTime;

        // Update the position with the current velocity
        value += velocity * deltaTime;
    }

    public void Reset()
    {
        velocity = 0f;
        value = 0f;
    }

    // Setter for spring's current position
    public void SetValue(float value)
    {
        this.value = value;
    }

    // Setter for spring's target position
    public void SetTarget(float target)
    {
        this.target = target;
    }

    // Setter for the damper
    public void SetDamper(float damper)
    {
        this.damper = damper;
    }

    // Setter for spring's strength
    public void SetStrength(float strength)
    {
        this.strength = strength;
    }

    // Setter for velocity
    public void SetVelocity(float velocity)
    {
        this.velocity = velocity;
    }

    // Property to get the current value/position of the spring
    public float Value => value;
}
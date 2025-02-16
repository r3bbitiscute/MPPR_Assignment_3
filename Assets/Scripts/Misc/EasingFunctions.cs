using UnityEngine;

[System.Serializable]
public class EasingFunctions
{
    /// <summary>
    /// Applies an Ease-In easing function, where progress starts slow and accelerates.
    /// </summary>
    /// <param name="t">The input progress value (0 to 1).</param>
    /// <returns>The eased progress value.</returns>
    public float EaseIn(float t)
    {
        return t * t;  // Quadratic easing in
    }

    /// <summary>
    /// Applies an Ease-Out easing function, where progress starts fast and decelerates.
    /// </summary>
    /// <param name="t">The input progress value (0 to 1).</param>
    /// <returns>The eased progress value.</returns>
    public float EaseOut(float t)
    {
        return 1 - (1 - t) * (1 - t);  // Quadratic easing out
    }

    /// <summary>
    /// Applies an Ease-In-Out easing function, where progress starts slow, 
    /// accelerates in the middle, and decelerates at the end.
    /// </summary>
    /// <param name="t">The input progress value (0 to 1).</param>
    /// <returns>The eased progress value.</returns>
    public float EaseInOut(float t)
    {
        // Use a cubic function for smoother transitions
        return t < 0.5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
    }
}

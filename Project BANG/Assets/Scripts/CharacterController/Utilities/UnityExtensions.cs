using UnityEngine;
using System.Collections;

public static class UnityExtensions
{


    /// <summary>
    /// Resets transform values.
    /// </summary>
    /// <param name="transform"></param>
    public static void ResetTransform(this Transform transform)
    {
        transform.position = new Vector3(0, 0, 0);
        transform.rotation = new Quaternion(0, 0, 0, 1);
        transform.localScale = new Vector3(1, 1, 1);
    }

    /// <summary>
    /// Resets transform local values.
    /// </summary>
    /// <param name="transform"></param>
    public static void ResetLocalTransform(this Transform transform)
    {
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = new Quaternion(0, 0, 0, 1);
        transform.localScale = new Vector3(1, 1, 1);
    }


    /// <summary>
    /// Equivalent to transform.position + Vector3.up * value.
    /// </summary>
    /// <param name="transform">Transform to affect.</param>
    /// <param name="value">Y value.</param>
    /// <returns>Transform position plus Vector3.up * value</returns>
    public static Vector3 WithY(this Transform transform, float value)
    {
        return transform.position + new Vector3(0, value, 0);
    }

    /// <summary>
    /// Equivalent to transform.position + Vector3.forward * value.
    /// </summary>
    /// <param name="transform">Transform to affect.</param>
    /// <param name="value">Z value.</param>
    /// <returns>Transform position plus Vector3.forward * value</returns>
    public static Vector3 WithZ(this Transform transform, float value)
    {
        return transform.position + new Vector3(0, 0, value);
    }

    /// <summary>
    /// Returns Vector with X and Z values to transform position.
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns>Returns Vector with X and Z values to transform position.</returns>
    public static Vector3 WithXZ(this Transform transform, float x, float z)
    {
        return transform.position + new Vector3(x, 0, z);
    }


    /// <summary>
    /// Gets angle around Y axis from a world space direction.
    /// </summary>
    /// <param name="transform">Transform to affect</param>
    /// <param name="worldDirection">World space direction.</param>
    /// <returns>Angle between transform forward and world direction.</returns>
    public static float AngleFromForward(this Transform transform, Vector3 worldDirection)
    {
        Vector3 local = transform.InverseTransformDirection(worldDirection);
        return Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;
    }


}

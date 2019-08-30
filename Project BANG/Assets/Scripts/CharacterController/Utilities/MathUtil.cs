using System;
using UnityEngine;

public static class MathUtil
{

    public static float Round(float value, int rounding = 2)
    {
        if (Mathf.Approximately(value, 0f)) return 0;
        rounding = Mathf.Clamp(rounding, 2, int.MaxValue);
        return (float)Math.Round(value, rounding);
    }


    public static float Min(float value1, float value2, float value3)
    {
        float min = (value1 < value2) ? value1 : value2;
        return (min < value3) ? min : value3;
    }

    public static float Max(float value1, float value2, float value3)
    {
        float max = (value1 > value2) ? value1 : value2;
        return (max > value3) ? max : value3;
    }


    public static float Squared(this float value)
    {
        return value * value;
    }
    public static float Squared(this int value)
    {
        return value * value;
    }


    public static float ClampAngle(float angle)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return angle;
    }





    public static float SmoothStep(float t)
    {
        return t * t * (3f - 2f * t);
    }

}

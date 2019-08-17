using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class MinMaxRangeAttribute : PropertyAttribute
{

    public float MinValue { get; private set; }
    public float MaxValue { get; private set; }


    public MinMaxRangeAttribute(float minValue, float maxValue)
    {
        this.MinValue = minValue;
        this.MaxValue = maxValue;
    }

}

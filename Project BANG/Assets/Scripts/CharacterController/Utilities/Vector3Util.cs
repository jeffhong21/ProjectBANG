using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Util
{

    public static Vector3 Combine(this Vector3 a, Vector3 b)
    {
        a.x += b.x;
        a.y += b.y;
        a.z += b.z;
        return a;
    }

    public static Vector3 Add(Vector3 a, Vector3 b )
    {
        a.x += b.x;
        a.y += b.y;
        a.z += b.z;
        return a;
    }


    public static Vector3 AddY(this Vector3 v, float y)
    {
        return new Vector3(v.x, v.y + y, v.z);
    }


    public static Vector3 Multiply(Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }



    public static Vector3 Squared(this Vector3 v)
    {
        return new Vector3(v.x * v.x, v.y * v.y, v.z * v.z);
    }


}

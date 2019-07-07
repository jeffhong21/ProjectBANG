//using UnityEngine;
//using System.Collections;

//public class MovementUtilities : MonoBehaviour
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="target"></param>
//    /// <param name="origin"></param>
//    /// <param name="time"> time is how long per second</param>
//    /// <returns></returns>
//    Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time)
//    {
//        //  Define the distance x and y first.
//        Vector3 distance = target - origin;
//        Vector3 distanceXZ = distance;
//        distanceXZ.y = 0;


//        //  Create a float that repsents our distance
//        float Sy = distance.y;              //  vertical distance
//        float Sxz = distanceXZ.magnitude;   //  horizontal distance


//        //  Calculate the initial velocity.  This is distance / time.
//        float Vxz = Sxz / time;
//        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;



//        Vector3 result = distanceXZ.normalized;
//        result *= Vxz;
//        result.y = Vy;

//        return result;
//    }
//}

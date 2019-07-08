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




//    public class DrawLineRenderer
//    {
//        float velocity;
//        float angle = 45f;
//        int resolution = 5;

//        float gravity;
//        float radianAngle;
//        Vector3[] arcPoints = new Vector3[0];

//        protected Vector3[] CalculateArcArray()
//        {
//            resolution = 5;
//            Vector3[] arcArray = new Vector3[resolution];
//            angle = 45f;
//            velocity = m_DistanceToWall - m_CapsuleCollider.radius * 2;
//            gravity = Mathf.Abs(Physics.gravity.y);
//            radianAngle = Mathf.Deg2Rad * angle;

//            //Debug.Log(gravity);



//            float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / gravity;

//            for (int index = 0; index < resolution; index++)
//            {
//                float t = (float)index / (float)resolution;
//                arcArray[index] = CalculateArcPoint(t, maxDistance);
//            }

//            return arcArray;
//        }


//        //  Calculate platformHeight and distance of each vertex.
//        Vector3 CalculateArcPoint(float t, float maxDistance)
//        {
//            float x = t * maxDistance;
//            float y = x * Mathf.Tan(radianAngle) - ((gravity * x * x) / (2 * velocity * velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));
//            return new Vector3(x, y);
//        }
//    }

//}

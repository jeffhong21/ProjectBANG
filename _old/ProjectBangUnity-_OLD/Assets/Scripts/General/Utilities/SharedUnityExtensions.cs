namespace SharedExtensions
{
    using UnityEngine;

    public static class SharedUnityExtensions
    {

        public static Vector3[] GetDirectionsFrom(this Vector3 mainDirection, int directionAmounts, float angle)
        {
            Vector3[] returnDirections = new Vector3[Mathf.Max(directionAmounts, 1)];

            float halfFOV = angle / 2.0f;
            Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
            Vector3 leftRayDirection = leftRayRotation * mainDirection;
            Vector3 desired = leftRayDirection;

            returnDirections[0] = directionAmounts <= 1 ? mainDirection : desired;
            if (directionAmounts <= 1) return returnDirections;
            float amountEach = angle / (directionAmounts - 1);
            for (int i = 0; i < directionAmounts; i++)
            {
                if (i == 0) continue;
                Quaternion desiredRotation = Quaternion.AngleAxis(amountEach, Vector3.up);
                desired = desiredRotation * desired;
                returnDirections[i] = desired;
            }

            return returnDirections;
        }


        //public static bool isInFrontAndRange(this Transform transform, Transform Testing, float range, float angle = 60)
        //{
        //    float findAngle = Vector3.Angle(transform.forward, Testing.position - transform.position);

        //    return Mathf.Abs(findAngle) < angle * 0.5f && transform.DistanceTo(Testing) < range;
        //}

        public static bool IsInFront(this Transform transform, Vector3 Testing, float angle = 60)
        {
            float findAngle = Vector3.Angle(transform.forward, Testing - transform.position);

            return Mathf.Abs(findAngle) < angle;
        }

        public static bool IsInFront(this Transform transform, Transform Testing, float angle = 60)
        {
            return transform.IsInFront(Testing.position, angle);
        }
    }
}


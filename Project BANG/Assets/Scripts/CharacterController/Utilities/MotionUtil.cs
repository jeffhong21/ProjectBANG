﻿using UnityEngine;
using System.Collections;

public static class MotionUtil
{





    public class MotionPath
    {

        public struct MotionPoint
        {
            public Vector3 point { get; private set; }
            public Vector3 velocity { get; private set; }

            public MotionPoint( Vector3 point, Vector3 velocity )
            {
                this.point = point;
                this.velocity = velocity;
            }
        }



        //public Transform transform;
        private Vector3 startPosition;
        private Vector3 motionDirection;
        private Vector3 endPosition;


        private Vector3[] motionPath;
        private int resolution;



        public MotionPath( int resolution )
        {
            this.resolution = resolution;
            motionPath = new Vector3[resolution];
        }



        public Vector3[] CalculateTrajectory( Vector3 startPosition, Vector3 motionDirection, Vector3 endPosition )
        {
            if (motionPath == null) motionPath = new Vector3[resolution];
            Vector3 p0 = startPosition;
            Vector3 p1 = startPosition + motionDirection;
            Vector3 p2 = endPosition;

            for (int i = 0; i <= resolution; i++) {
                float t = i / (float)resolution;
                motionPath[i] = GetPoint(p0, p1, p2, t);
            }

            return motionPath;
        }


        public Vector3 GetPoint( Vector3 p0, Vector3 p1, Vector3 p2, float t )
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * p0 +
                2f * oneMinusT * t * p1 +
                t * t * p2;
        }


        public void DrawMotionPath( float pointSize = 0.01f )
        {
            if (motionPath == null || motionPath.Length == 0) return;
            Vector3 lineStart = motionPath[0];
            Gizmos.color = Color.white;
            for (int i = 0; i < resolution; i++) {
                Vector3 lineEnd = motionPath[i];
                Gizmos.DrawLine(lineStart, lineEnd);
                Gizmos.DrawSphere(lineEnd, pointSize);
                lineStart = lineEnd;
            }
        }


        //public Vector3 GetFirstDerivative( Vector3 p0, Vector3 p1, Vector3 p2, float t )
        //{
        //    return
        //        2f * (1f - t) * (p1 - p0) +
        //        2f * t * (p2 - p1);
        //}

        //public Vector3 GetVelocity(float t)
        //{
        //    return transform.TransformPoint(GetFirstDerivative(motionPoints[0], motionPoints[1], motionPoints[2], t)) -
        //        transform.position;
        //}

        //public void DisplayMotionPath()
        //{

        //    Vector3 lineStart = motionPoints[0];
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawLine(lineStart, lineStart + GetVelocity(0));
        //    for (int i = 0; i < resolution; i++) {
        //        Vector3 lineEnd = motionPoints[i];
        //        Gizmos.color = Color.white;
        //        Gizmos.DrawLine(lineStart, lineEnd);
        //        Gizmos.color = Color.green;
        //        Gizmos.DrawLine(lineEnd, lineEnd + GetVelocity(i / (float)resolution));
        //        lineStart = lineEnd;
        //    }
        //}

    }




    public static Vector3 GetDisplacement( Vector3 initialVelocity, Vector3 acceleration, float time )
    {
        Vector3 displacement = initialVelocity * time + (acceleration * (time * time)) / 2;
        return displacement;
    }

    public static Vector3 GetFinalVelocity( Vector3 initialVelocity, Vector3 acceleration, float time )
    {
        return initialVelocity + acceleration * time;
    }


    public static Vector3 GetAcceleration(Vector3 finalVelocity, Vector3 initialVelocity, float time)
    {
        return (finalVelocity - initialVelocity) / time;
    }


    /// <summary>
    /// Get the distance knowing the final velocity and acceleration.
    /// </summary>
    /// <param name="v">Final velocity.</param>
    /// <param name="u">Initial velocity.</param>
    /// <param name="a">Acceleration.</param>
    /// <returns>Returns the displacement distance.</returns>
    public static float Distance( Vector3 v, Vector3 u, Vector3 a )
    {
        return (v.sqrMagnitude - u.sqrMagnitude) / (2 * a.magnitude);
    }
}

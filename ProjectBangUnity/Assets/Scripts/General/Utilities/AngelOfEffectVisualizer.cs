using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SharedExtensions;

public class AngelOfEffectVisualizer : MonoBehaviour 
{
    public float totalFOV = 70.0f;
    public float rayRange = 10.0f;

    public Transform testTransform;

    public int amountOfBullets = 5;

    public bool ShowAngleTest = false;
    public bool ShowBulletsTest = false;

    private void OnDrawGizmosSelected()
    {
        if (!ShowBulletsTest) return;
        Vector3[] dirs = transform.forward.GetDirectionsFrom(amountOfBullets, totalFOV);
        for (int i = 0; i < dirs.Length; i++)
        {
            Gizmos.DrawRay(transform.position, dirs[i] * rayRange);
        }
    }


    void OnDrawGizmos()
    {
        if (!ShowAngleTest) return;
        float halfFOV = totalFOV / 2.0f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;

        //Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, leftRayDirection * rayRange);
        Gizmos.DrawRay(transform.position, rightRayDirection * rayRange);
        //Gizmos.DrawLine(transform.position+ ( rightRayDirection * rayRange), transform.position + (leftRayDirection * rayRange));
        if (testTransform != null)
        {
            if (transform.IsInFront(testTransform, totalFOV) && Vector3.Distance(transform.position, testTransform.position)  < rayRange)  //transform.DistanceTo(testTransform) 
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(testTransform.position, 0.5f);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(testTransform.position, 0.5f);
            }
        }
    }



}

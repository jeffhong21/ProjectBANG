namespace CharacterController
{
    using UnityEngine;
    using System;

    //[DisallowMultipleComponent]
    //[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(LayerManager))]
    public class RigidbodyController : MonoBehaviour
    {








        protected bool DetectionRaycast(float stopMovementAngle, out RaycastHit hitInfo, float checkDistance, float checkHeight = 0.4f, int layerMask = 1 << 27)
        {
            //checkHeight = Mathf.Clamp(checkHeight, 0, m_CapsuleCollider.height);
            //  TODO: check if stopMovementAngle exceedes 180.

            Vector3 hitDetectionStartRay = transform.position + Vector3.up * checkHeight;
            Quaternion rayRotation = Quaternion.AngleAxis(stopMovementAngle, transform.up) * transform.rotation;
            Vector3 hitDetectionEndRay = rayRotation * transform.InverseTransformDirection(transform.forward);

            bool hitObject = false;
            if (Physics.Raycast(hitDetectionStartRay, hitDetectionEndRay + Vector3.up * checkHeight, out hitInfo, checkDistance, layerMask))
            {
                if (hitInfo.collider != null)
                {
                    hitObject = true;
                }
            }
            Debug.DrawRay(hitDetectionStartRay, hitDetectionEndRay * checkDistance, hitObject == true ? Color.red : Color.blue);
            return hitObject;
        }
    }

}

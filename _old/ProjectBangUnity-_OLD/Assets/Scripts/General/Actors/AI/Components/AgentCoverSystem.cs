namespace Bang
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    using SharedExtensions;

    public class AgentCoverSystem : MonoBehaviour
    {
        private Vector3 aimOrigin;

        protected Transform leftHelper;
        protected Transform rightHelper;





        private void InitializeCoverMarkers()
        {
            leftHelper = new GameObject().transform;
            leftHelper.name = "Left cover Helper";
            leftHelper.parent = transform;
            leftHelper.localPosition = Vector3.zero;
            leftHelper.localEulerAngles = Vector3.zero;

            rightHelper = new GameObject().transform;
            rightHelper.name = "Right cover Helper";
            rightHelper.parent = transform;
            rightHelper.localPosition = Vector3.zero;
            rightHelper.localEulerAngles = Vector3.zero;
        }


        public void EnterCover(CoverObject cover)
        {
            Color debugColor = Color.red;
            if (cover == null) return;

            float minCoverHeight = aimOrigin.y * 0.75f;
            float maxDistance = 1f;     //  Max distance away from cover.

            Vector3 origin = aimOrigin;
            Vector3 directionToCover = -(transform.position - cover.transform.position);
            //directionToCover.y = minCoverHeight;
            RaycastHit hit;



            if (Physics.Raycast(origin, directionToCover, out hit, maxDistance, Layers.cover))
            {
                //  We hit a box collider
                if (hit.transform.GetComponent<BoxCollider>())
                {
                    Quaternion targetRot = Quaternion.FromToRotation(transform.forward, hit.normal) * transform.rotation;
                    float angel = Vector3.Angle(hit.normal, -transform.forward);
                    //Debug.Log(angel);
                    transform.rotation = targetRot;

                    //States.CanShoot = true;
                    //States.InCover = true;

                    //AnimHandler.EnterCover();
                    Debug.Log("Agent is taking cover");
                    debugColor = Color.green;
                }
            }

            Debug.DrawRay(origin, directionToCover, debugColor, 0.5f);
            //Debug.Break();
        }


        //protected IEnumerator TransitionCoverStates()
        //{
        //    yield return new WaitForSeconds(1f);
        //    States.CanShoot = true;
        //    States.InCover = true;
        //}


        protected void HandleCoverState()
        {
            //Vector3 lookRotation = (transform.position - hitNormal);
            ////  Errors out when roation method is called towards a vector zero.
            //if (lookRotation != Vector3.zero)
            //{
            //    // Create a quaternion (rotation) based on looking down the vector from the player to the target.
            //    Quaternion newRotatation = Quaternion.LookRotation(lookRotation);
            //    transform.rotation = Quaternion.Slerp(transform.rotation, newRotatation, Time.fixedDeltaTime * 8);
            //}
        }


        public bool CanEmergeFromCover(Transform helper, bool right)
        {
            float entitySize = 0.5f;
            float distOffset = entitySize * 0.5f;
            Vector3 origin = transform.position;
            Vector3 side = (right == true) ? transform.right : -transform.right;
            //side.y = origin.y;
            Vector3 direction = side - origin;
            Vector3 helpPosition = side + (direction.normalized * 0.025f);
            helpPosition.y = 1f;
            helper.localPosition = helpPosition;
            Vector3 outDir = (-helper.transform.forward) + helper.position;


            float scanDistance = (outDir - helper.position).magnitude;
            RaycastHit hit;

            if (Physics.Raycast(helper.position, outDir, out hit, scanDistance, Layers.cover))
            {
                Debug.DrawLine(helper.position, outDir, Color.red, 1f);
                Debug.Log(helper.name + " hit " + hit.transform.name);
                return false;
            }

            Debug.DrawLine(helper.position, outDir, Color.green, 1f);
            return true;
        }


        protected void OnDrawGizmosSelected()
        {
            if (leftHelper != null && rightHelper != null)
            {
                Vector3 size = new Vector3(0.2f, 0.2f, 0.2f);
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(leftHelper.position, size);
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(rightHelper.position, size);
            }
        }


        //public bool CanEmergeFromCover(bool left = false)
        //{
        //    float entitySize = 0.5f;
        //    float distOffset = entitySize * 0.5f;
        //    Vector3 origin = aimOrigin;
        //    Vector3 side = left == false ? transform.right : -transform.right;

        //    Vector3 direction = side - origin;
        //    Vector3 scanPosition = side + (direction.normalized * distOffset);

        //    //Vector3 outDirection = scanPosition - Vector3.back;
        //    //Vector3 scanPosition = origin + offset * entitySize;
        //    //Vector3 scanDirection = scanPosition + (outDirection.normalized * 1);
        //    Vector3 scanDirection = new Vector3(scanPosition.x, scanPosition.y, scanPosition.z - 1);

        //    RaycastHit hit;

        //    Debug.DrawRay(origin, scanPosition, Color.magenta, 0.25f);
        //    Debug.Log(scanPosition);
        //    //Debug.DrawRay(scanPosition, scanDirection, Color.blue, 0.25f);


        //    if (Physics.Raycast(scanPosition, scanDirection, out hit, entitySize, Layers.cover))
        //    {
        //        Debug.DrawRay(scanPosition, scanDirection, Color.red, 0.25f);
        //        return false;
        //    }
        //    //Debug.DrawRay(scanPosition, scanDirection, Color.green, 0.25f);
        //    Debug.DrawRay(scanPosition, scanDirection, Color.blue, 0.25f);
        //    return true;
        //}


        //public void ExitCover()
        //{
        //    States.InCover = false;
        //    States.CanShoot = true;
        //    AnimHandler.ExitCover();
        //    Debug.Log("Agent is leaving cover");
        //}


    }

}
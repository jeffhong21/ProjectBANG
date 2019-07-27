namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Climb : CharacterAction
    {

        [Tooltip("Where does the detection raycast height start at.")]
        [SerializeField] protected float checkHeight = 0.4f;
        [Tooltip("Max distance to start action.")]
        [SerializeField] protected float startDistance = 2f;
        [Tooltip("Minimum height to check if action can start.")]
        [SerializeField] protected float minHeight = 0.4f;
        [Tooltip("Maximum height.")]
        [SerializeField] protected float maxHeight = 2f;
        [Tooltip("Layers to check against.")]
        [SerializeField] protected LayerMask collisionLayers;


        protected float reachOffset;
        protected float platformHeight;
        protected RaycastHit objectHit, heightHit;
        protected Vector3 objectNormal;
        protected Vector3 endEdgeOffset, endEdge;


        //  Where to start the vertical raycast.
        protected Vector3 heightCheck;
        //  Height of the platform
        protected float height;

        protected float jumpForce;
        protected float timeToApex = 0.4f;
        protected float verticalVelocity;


        protected Vector3 rayOrigin;
        protected RaycastHit horizontalRayHit, verticalRayHit;
        protected Vector3 startPosition, endPosition;



        #region Character Action Methods

        protected virtual void Start()
        {
            collisionLayers = m_Layers.SolidLayers;
            rayOrigin = m_Transform.position + (Vector3.up * checkHeight) + (m_Transform.forward * (m_CapsuleCollider.radius - 0.1f));
        }


        public override bool CanStartAction()
        {
            if (!base.CanStartAction()) {
                return false;
            }

            rayOrigin = m_Transform.position + (Vector3.up * checkHeight) + (m_Transform.forward * (m_CapsuleCollider.radius - 0.1f));

            if (Physics.Raycast(rayOrigin, m_Transform.forward, out objectHit, startDistance, collisionLayers)) {
                return CheckHeightRequirement(objectHit.point);
            }

            return false;
        }


        protected override void ActionStarted()
        {
            reachOffset = m_CapsuleCollider.radius + 0.1f;
            objectNormal = objectHit.normal;
            DeterminePositions();



            //  Cache variables
            m_ColliderHeight = m_CapsuleCollider.height;
            m_ColliderCenter = m_CapsuleCollider.center;


            //jumpForce = -(2 * height) / Mathf.Pow(0.4f, 2);
            //verticalVelocity = Mathf.Abs(jumpForce) * timeToApex;
            Vector3 velocity = CalculateVelocity(endPosition, startPosition, timeToApex);
            //velocity.y = verticalVelocity;
            m_Rigidbody.velocity = velocity;
        }


        public override bool UpdateRotation()
        {

            return false;
        }


        //  Move over the vault object based off of the root motion forces.
        public override bool UpdateMovement()
        {
            Vector3 targetPosition = Vector3.Lerp(startPosition, endEdgeOffset, m_DeltaTime * 10);

            m_Rigidbody.MovePosition(targetPosition);

            return false;
        }


        public override bool Move()
        {
            return false;
        }


        public override bool CanStopAction()
        {
            int layerIndex = 0;
            if (m_Animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == 0) {
                m_ExitingAction = true;
            }
            if (m_ExitingAction && m_Animator.IsInTransition(layerIndex)) {
                Debug.LogFormat("{1} is exiting. | {0} is the next state.", m_AnimatorMonitor.GetStateName(m_Animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash), this.GetType());
                return true;
            }

            if (m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(m_StateName + "." + m_DestinationStateName)) {
                if (m_Animator.IsInTransition(0)) {
                    return true;
                }
            }

            return Time.time > m_ActionStartTime + 2;
        }

        protected override void ActionStopped()
        {

        }



        #endregion


        protected float HeightPercentage(Vector3 targetPosition, float distance)
        {
            float remainingDistance = targetPosition.y - m_Transform.position.y;
            float percent = (distance - remainingDistance) / distance;
            return percent;
        }


        protected void DeterminePositions()
        {


            startPosition = m_Transform.position;

            endEdgeOffset = endEdge + objectNormal * reachOffset;
            endPosition = endEdge - objectNormal * m_CapsuleCollider.radius;
        }





        protected Vector3 DetermineReachOffsets(Vector3 edge)
        {
            reachOffset = m_CapsuleCollider.radius + 0.1f;
            objectNormal = objectHit.normal;

             return edge + objectNormal * reachOffset;
        }


        protected Vector3 DetermineNormal(RaycastHit hit)
        {
            Vector3 normal = hit.normal;


            return normal;
        }


        /// <summary>
        /// Check if the detected object meets the height requirements.
        /// </summary>
        /// <param name="hitPoint"></param>
        /// <returns>Returns true if hit object meets the height requirement </returns>
        protected bool CheckHeightRequirement(Vector3 hitPoint)
        {
            heightCheck = hitPoint + Vector3.up * (maxHeight - checkHeight + 0.01f);
            float radius = m_CapsuleCollider.radius * 0.75f;

            if(Physics.SphereCast(heightCheck, radius, Vector3.down,out heightHit, maxHeight, collisionLayers))
            {
                //  If max height is 2m and distance is 0.4m, than the platform height is 1.6m.
                platformHeight = maxHeight - heightHit.distance;
                if (platformHeight >= minHeight) {
                    endEdge = heightHit.point;
                    return true;
                }
            }

            platformHeight = 0;
            heightCheck = Vector3.zero;
            return false;
        }




        protected Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time)
        {
            //  Define the distance x and y first.
            Vector3 distance = target - origin;
            Vector3 distanceXZ = distance;
            distanceXZ.y = 0;


            //  Create a float that repsents our distance
            float verticalDistance = distance.y;              //  vertical distance
            float horizontalDistance = distanceXZ.magnitude;   //  horizontal distance


            //  Calculate the initial velocity.  This is distance / time.
            float velocityXZ = horizontalDistance / time;
            float velocityY = (verticalDistance / time) + 0.5f * Mathf.Abs(Physics.gravity.y) * time;



            Vector3 result = distanceXZ.normalized;
            result *= velocityXZ;
            result.y = velocityY;

            return result;
        }




        protected virtual void OnDrawGizmos()
        {
            if (Application.isPlaying && m_IsActive && m_Debug) {
                //Gizmos.color = Color.green;
                //Gizmos.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance);

                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(startPosition, 0.2f);

                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(endEdgeOffset, 0.2f);

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(endPosition, 0.2f);


            }
        }

    }

}


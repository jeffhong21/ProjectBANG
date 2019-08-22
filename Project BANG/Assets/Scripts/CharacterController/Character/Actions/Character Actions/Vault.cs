﻿namespace CharacterController
{
    using UnityEngine;



    public class Vault : CharacterAction
    {

        [Tooltip("Maximum height."), Range(0, 90)]
        [SerializeField] protected float angleThreshold = 30f;
        [Tooltip("Where does the detection raycast height start at.")]
        [SerializeField] protected float checkHeight = 0.4f;
        [Tooltip("Max distance to start action.")]
        [SerializeField] protected float startDistance = 2f;
        [Tooltip("Minimum height to check if action can start.")]
        [SerializeField] protected float minHeight = 0.4f;
        [Tooltip("Maximum height.")]
        [SerializeField] protected float maxHeight = 2f;
        [Tooltip("Layers to check against.")]
        [SerializeField] protected LayerMask detectLayers;



        protected float platformHeight;
        protected RaycastHit objectHit, heightHit;

        protected bool cachedIsKinamatic;

        //  Where to start the vertical raycast.
        protected Vector3 heightCheck;
        //  Height of the platform
        protected float height; 
        protected float objectAngle;

        protected Vector3 rayOrigin;
        protected Vector3 objectNormal;
        protected Vector3 platformEdge;
        protected Vector3 startReach, endReach;
        protected Vector3 startPosition, endPosition;


        private Vector3 verticalPosition, fwdPosition;
        private bool apexReached;
        private float currentTime, totalTime;

        #region Character Action Methods

        protected virtual void Start()
        {
            detectLayers = m_Layers.SolidLayers;
        }


        public override bool CanStartAction()
        {
            if (!base.CanStartAction()) {
                return false;
            }

            //rayOrigin = m_Transform.position + (Vector3.up * checkHeight) + (m_Transform.forward * (m_CapsuleCollider.radius - 0.1f));
            rayOrigin = m_Transform.position + (Vector3.up * checkHeight);

            if (Physics.Raycast(rayOrigin, m_Transform.forward, out objectHit, startDistance, detectLayers))
            {
              
                float angle = Vector3.Angle(m_Transform.forward, -objectHit.normal);
                if (Mathf.Abs(angle) < angleThreshold)
                    return CheckHeightRequirement(objectHit.point);
            }

            return false;
        }


        /// <summary>
        /// Check if the detected object meets the height requirements.
        /// </summary>
        /// <param name="hitPoint"></param>
        /// <returns>Returns true if hit object meets the height requirement </returns>
        protected bool CheckHeightRequirement( Vector3 hitPoint )
        {
            heightCheck = hitPoint + Vector3.up * (maxHeight - checkHeight + 0.01f);
            float radius = m_CapsuleCollider.radius * 0.75f;

            if (Physics.SphereCast(heightCheck, radius, Vector3.down, out heightHit, maxHeight, detectLayers)) {
                //  If max height is 2m and distance is 0.4m, than the platform height is 1.6m.
                platformHeight = maxHeight - heightHit.distance;
                if (platformHeight >= minHeight) {
                    
                    return true;
                }
            }

            platformHeight = 0;
            heightCheck = Vector3.zero;
            return false;
        }


        protected Vector3 GetNormal(Vector3 normal)
        {
            var rhs = Vector3.Cross(normal, Vector3.up);
            var orthoNormal = Vector3.Cross(rhs, Vector3.down);

            if (m_Debug) Debug.DrawRay(objectHit.point, normal, Color.red, 2);
            if (m_Debug) Debug.DrawRay(objectHit.point, orthoNormal, Color.blue, 2);

            return orthoNormal;
        }


        protected Vector3 GetEndPosition(int depthCheck = 4)
        {
            depthCheck = Mathf.Clamp(depthCheck, 2, 5);
            heightCheck = objectHit.point + Vector3.up * (maxHeight - checkHeight + 0.01f);
            RaycastHit hit;
            for (int i = 1; i < depthCheck + 1; i++) {
                Vector3 raycastPosition = heightCheck + -objectNormal * (m_CapsuleCollider.radius * i);
                if (Physics.Raycast(raycastPosition, Vector3.down, out hit, maxHeight, detectLayers))
                {
                    if (m_Debug)
                        Debug.DrawRay(raycastPosition, Vector3.down * maxHeight, Color.green, 1);

                    if (i > 2){
                        if(Mathf.Abs(hit.point.y - endPosition.y) > m_CapsuleCollider.radius) {
                            RaycastHit depthHit;
                            if (Physics.Raycast(raycastPosition + -objectNormal * m_CapsuleCollider.radius, -objectNormal, out depthHit, maxHeight, detectLayers)) {
                                endPosition = depthHit.point;
                                continue;
                            }
                        }
                        if (hit.point == endPosition) {
                            if (Mathf.Abs(hit.point.y - endPosition.y) > m_CapsuleCollider.radius) {
                                endPosition += -objectNormal * (m_CapsuleCollider.radius * 1.0f);
                            }
                            break;
                        }
                            
                    }
                    endPosition = hit.point;
                }

            }
            return endPosition;
        }


        protected override void ActionStarted()
        {
            objectNormal = GetNormal(objectHit.normal);

            startPosition = m_Transform.position;
            platformEdge = heightHit.point;
            startReach = platformEdge + objectNormal * (m_CapsuleCollider.radius + 0);
            endPosition = GetEndPosition();
            endReach = endPosition;
            endReach.y = platformEdge.y;


            //  Cache variables
            cachedIsKinamatic = m_Rigidbody.isKinematic;
            m_Rigidbody.isKinematic = true;


            if(Mathf.Abs(endPosition.y - endReach.y) > m_CapsuleCollider.radius) {
                m_Animator.SetInteger(HashID.ActionID, ActionTypeID.Vault);
                m_Animator.SetInteger(HashID.ActionIntData, 0);
            }
            else {
                m_Animator.SetInteger(HashID.ActionID, ActionTypeID.Climb1M);
                m_Animator.SetInteger(HashID.ActionIntData, 0); 
            }


            EventHandler.ExecuteEvent(gameObject, "OnSetMatchTarget", platformEdge + -objectNormal * m_CapsuleCollider.radius, Quaternion.identity);

            currentTime = 0;
            totalTime = 0.5f;
        }


        public override bool UpdateRotation()
        {
            Quaternion rotation = Quaternion.FromToRotation(m_Transform.forward, -objectNormal) * m_Transform.rotation;
            m_Rigidbody.MoveRotation(Quaternion.Slerp(rotation, m_Transform.rotation, m_DeltaTime * m_Controller.RotationSpeed));
            return false;
        }


        public override bool UpdateMovement()
        {
            currentTime += m_DeltaTime;
            if(currentTime > totalTime) {
                currentTime = totalTime;
            }
            var perc = currentTime / totalTime;
            var heightDistance = platformEdge.y - m_Transform.position.y;

            if (apexReached == false) apexReached = heightDistance <= 0f;

            Debug.LogFormat("heightDistance: {0}, apexReached: {1}, rootMotion: {2}", heightDistance, apexReached, m_Animator.applyRootMotion);
            if (heightDistance >= 0f && !apexReached) {
                //verticalPosition = Vector3.Lerp(m_Transform.position, startReach, m_DeltaTime * 10);
                verticalPosition = Vector3.MoveTowards(m_Transform.position, startReach, m_DeltaTime * 10);
            }
            if (heightDistance <= 0.1f) {
                fwdPosition = Vector3.Lerp(m_Transform.position, endReach, perc * perc);
            }

            m_Rigidbody.MovePosition((verticalPosition + fwdPosition));


            if (m_Debug) DebugDraw.Sphere(verticalPosition + fwdPosition, 0.1f, Color.black);

            if ((endReach - m_Transform.position).sqrMagnitude < 0.1f) {
                m_Rigidbody.isKinematic = cachedIsKinamatic;
            }

            return false;
        }


        public override bool Move()
        {
            
            return true;
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



            return Time.time > m_ActionStartTime + 2;
        }

        protected override void ActionStopped()
        {
            m_Rigidbody.isKinematic = cachedIsKinamatic;
            m_Rigidbody.velocity = m_Controller.Velocity;

            startPosition = Vector3.zero;
            startReach = Vector3.zero;
            endReach = Vector3.zero;
            endPosition = Vector3.zero;
            platformEdge = Vector3.zero;
            apexReached = false;
        }



        //public override string GetDestinationState( int layer )
        //{
        //    if (layer == 0) {
        //        return m_StateName;
        //    }

        //    return "";
        //}



        #endregion













        protected virtual void OnDrawGizmos()
        {
            if (Application.isPlaying && m_IsActive && m_Debug) {
                //Gizmos.color = Color.green;
                //Gizmos.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance);

                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(startPosition, 0.08f);

                Gizmos.color = Color.cyan;
                GizmosUtils.DrawMarker(startReach, 0.08f, Color.cyan);

                Gizmos.color = Color.cyan;
                GizmosUtils.DrawMarker(endReach, 0.08f, Color.cyan);

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(endPosition,m_CapsuleCollider.radius);


            }
        }
    }

}

namespace CharacterController
{
    using UnityEngine;
    using System.Collections.Generic;


    public class JumpOnPlatform : CharacterAction
    {

        [SerializeField]
        protected float checkHeight = 0.25f;
        [SerializeField]
        protected float startDistance = 2f;
        [SerializeField, Tooltip("The highest level the character can climb.")]
        protected float maxHeight = 2f;
        [SerializeField]
        protected float minHeight = 0.4f;
        [SerializeField]
        protected LayerMask collisionLayers;
        [SerializeField]
        protected AnimatorStateMatchTarget matchTarget;
        protected MatchTargetWeightMask matchTargetWeightMask = new MatchTargetWeightMask(Vector3.one, 0);

        //  Where to start the vertical raycast.
        protected Vector3 heightCheckStart;
        //  Height of the platform
        protected float platformHeight;

        protected float jumpForce;
        protected float timeToApex = 0.4f;
        protected float verticalVelocity;

        protected Vector3 velocity;
        protected Vector3 rayOrigin;
        protected RaycastHit detectObjectHit, detectObjectHeightHit;
        protected Vector3 startPosition, endPosition;

        ////  Smooth velocity for forward movement.
        //protected float velocitySmooth;
        //protected float accelerationTime = 0.1f;





        public override bool CanStartAction()
        {
            bool canStart = false;
            if (base.CanStartAction())
            {
                //  Check if character is within range.

                rayOrigin = m_Transform.position + (Vector3.up * checkHeight) + (m_Transform.forward * (m_CapsuleCollider.radius - 0.1f));
                if (Physics.Raycast(rayOrigin, m_Transform.forward, out detectObjectHit, startDistance, collisionLayers))
                {

                    heightCheckStart = detectObjectHit.point;
                    heightCheckStart.y += (maxHeight + 0.2f) - checkHeight;

                    //  This will check if platform is too high for character.
                    if (Physics.SphereCast(heightCheckStart, m_CapsuleCollider.radius, Vector3.down, out detectObjectHeightHit, maxHeight, collisionLayers))
                    {

                        var heightCheckDist = detectObjectHeightHit.distance;
                        //  Get the objet to vault over platformHeight.
                        platformHeight = maxHeight - heightCheckDist;

                        canStart = true;
                    }

                }

                if (m_Debug) Debug.DrawRay(rayOrigin, m_Transform.forward * startDistance, Color.green);

            }


            return canStart;
        }


        protected override void ActionStarted()
        {
            var jumpTime = timeToApex * platformHeight;
            endPosition = detectObjectHeightHit.point + (m_Transform.forward * (m_CapsuleCollider.radius + 0.12f));
            startPosition = m_Transform.position;

            jumpForce = -(2 * platformHeight) / Mathf.Pow(jumpTime, 2);
            verticalVelocity = Mathf.Abs(jumpForce) * jumpTime;

            //platformHeight = jumpForce * Mathf.Pow(jumpTime, 2) / -2;

            //var adjustedStartingPos = startPosition;
            ////adjustedStartingPos.y = endPosition.y;
            velocity = CalculateVelocity(endPosition, startPosition, jumpTime);



            //  Cache variables
            m_ColliderHeight = m_CapsuleCollider.height;
            m_ColliderCenter = m_CapsuleCollider.center;

            //m_Rigidbody.isKinematic = !m_Rigidbody.isKinematic;

            velocity.y = verticalVelocity;
            velocity.x = 0;
            velocity.z = 0;
            m_Rigidbody.velocity = velocity;
        }




        public override bool UpdateRotation()
        {
            float distance = detectObjectHeightHit.point.y - m_Transform.position.y;
            float percent = (platformHeight - distance) / platformHeight;

            var rotation = Quaternion.FromToRotation(m_Transform.forward, -detectObjectHit.normal) * m_Transform.rotation;
            m_Rigidbody.MoveRotation(Quaternion.Slerp(rotation, m_Rigidbody.rotation, 2 * percent).normalized);
            return false;
        }




        //  Move over the vault object based off of the root motion forces.
        public override bool UpdateMovement()
        {

            //float targetVelocityX = Mathf.Abs(distance) * 1;
            //velocity.z = Mathf.SmoothDamp(velocity.z, targetVelocityX, ref velocitySmooth, accelerationTime);
            velocity.y += jumpForce * m_DeltaTime;
            m_Rigidbody.AddForce(velocity, ForceMode.VelocityChange);

            //m_Rigidbody.MovePosition(velocity);
            //m_Animator.MatchTarget(endPosition + matchTarget.matchTargetOffset, Quaternion.identity, matchTarget.avatarTarget, matchTargetWeightMask,
            //    matchTarget.startMatchTarget, matchTarget.endMatchTarget);

            return false;
        }


        public override bool Move()
        {
            ////m_Animator.MatchTarget(m_MatchPosition, Quaternion.Euler(0, m_Transform.eulerAngles.y, 0), AvatarTarget.LeftHand, m_MatchTargetWeightMask, m_StartMatchTarget, m_StopMatchTarget);
            //m_Animator.MatchTarget(m_MatchPosition, Quaternion.identity, AvatarTarget.LeftHand, m_MatchTargetWeightMask, m_StartMatchTarget, m_StopMatchTarget);

            return false;
        }




        protected Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time)
        {
            //  Define the distance x and y first.
            Vector3 distance = target - origin;
            Vector3 distanceXZ = distance;
            distanceXZ.y = 0;


            //  Create a float that repsents our distance
            float Sy = distance.y;              //  vertical distance
            float Sxz = distanceXZ.magnitude;   //  horizontal distance


            //  Calculate the initial velocity.  This is distance / time.
            float Vxz = Sxz / time;
            float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;



            Vector3 result = distanceXZ.normalized;
            result *= Vxz;
            result.y = Vy;

            return result;
        }



        public override string GetDestinationState(int layer)
        {
            if (layer == 0)
                return m_DestinationStateName = "Climb1M";
            return "";
                
        }




        public override bool CanStopAction()
        {
            int layerIndex = 0;
            if (m_Animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == 0)
            {
                m_ExitingAction = true;
            }
            if (m_ExitingAction && m_Animator.IsInTransition(layerIndex))
            {
                Debug.LogFormat("{1} is exiting. | {0} is the next state.", m_AnimatorMonitor.GetStateName(m_Animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash), this.GetType());
                return true;
            }

            if (m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(m_StateName + "." + m_DestinationStateName)){
                if (m_Animator.IsInTransition(0)){
                    return true;
                }
            }

            return Time.time > m_ActionStartTime + 2;
        }


        protected override void ActionStopped()
        {
            m_Rigidbody.isKinematic = false;
            startPosition = endPosition = Vector3.zero;

        }




        private void OnDrawGizmos()
        {
            if (Application.isPlaying && m_IsActive)
            {
                if (startPosition != Vector3.zero)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(startPosition, 0.2f);
                    //GizmosUtils.DrawString("Start Position", m_StartPosition, Color.white);
                }
                if (endPosition != Vector3.zero)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(endPosition, 0.2f);
                    UnityEditor.Handles.color = Color.yellow;
                    UnityEditor.Handles.DrawSolidDisc(endPosition, Vector3.up, 0.2f);
                    //GizmosUtils.DrawString("Match Position", m_MatchPosition, Color.white);
                }

            }
        }














    }
}


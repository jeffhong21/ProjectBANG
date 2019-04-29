namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class JumpOnPlatform : CharacterAction
    {
        protected float m_CheckHeight = 0.4f;
        //protected float m_CheckDistance = 4f;

        [SerializeField, Tooltip("Max distance character can be to start the action.")]
        protected float m_MoveToJumpDistance = 1f;
        [SerializeField]
        protected float m_MaxHeight = 1.25f;
        //[SerializeField]
        protected float m_JumpForce = 3f;
        [SerializeField]
        protected LayerMask m_CheckLayers;
        [SerializeField]
        protected float m_HorizontalMatchTargetOffset = 0.1f;
        [SerializeField]
        protected float m_VerticalMatchTargetOffset;
        [SerializeField]
        protected float m_StartMatchTarget;
        [SerializeField]
        protected float m_StopMatchTarget;
        [SerializeField]
        protected AvatarTarget m_AvatarTarget = AvatarTarget.LeftHand;


        private Vector3 m_Velocity, m_VerticalVelocity;
        private float m_PlatformHeight;
        private Vector3 m_StartPosition, m_EndPosition, m_MatchPosition;
        private Vector3 m_HeightCheckStart;
        private RaycastHit DetectObjectHit, ObjectHeightHit;

        private MatchTargetWeightMask m_MatchTargetWeightMask = new MatchTargetWeightMask(Vector3.one, 0);
        private float m_StartTime;
        private float m_ColliderHeight;
        private Vector3 m_ColliderCenter;





        public override bool CanStartAction()
        {
            if (base.CanStartAction())
            {
                if (Physics.Raycast(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward, out DetectObjectHit, m_MoveToJumpDistance, m_CheckLayers))
                {
                    m_HeightCheckStart = DetectObjectHit.point;
                    m_HeightCheckStart.y += m_MaxHeight - m_CheckHeight;

                    //if (m_Debug) Debug.DrawRay(heightCheckStart, Vector3.down * (m_MaxClimbHeight - m_MinClimbHeight), Color.cyan, 1f);
                    if (Physics.Raycast(m_HeightCheckStart, Vector3.down, out ObjectHeightHit, m_MaxHeight, m_CheckLayers))
                    {
                        //  cache HeightCheckHit distance.
                        var heightCheckDist = ObjectHeightHit.distance;
                        if (heightCheckDist < m_MaxHeight)
                        {
                            //  Get the plaform height.
                            m_PlatformHeight = m_MaxHeight - heightCheckDist;
                            //  Get the position of when the characters hand is placed on the object.
                            m_MatchPosition = ObjectHeightHit.point + (Vector3.up * m_VerticalMatchTargetOffset) + (m_Transform.right * m_HorizontalMatchTargetOffset);
                            //m_MatchPosition = ObjectHeightHit.point + (Vector3.up * m_VerticalMatchTargetOffset);
                            m_EndPosition = ObjectHeightHit.point + (Vector3.up * m_VerticalMatchTargetOffset) + (m_Transform.forward * m_HorizontalMatchTargetOffset);
                            return true;
                        }
                    }
                }
            }
            return false;
        }




        protected override void ActionStarted()
        {
            //m_CapsuleCollider.isTrigger = true;
            //m_Rigidbody.useGravity = false;
            m_ColliderCenter = m_CapsuleCollider.center;

            m_StartTime = Time.time;
        }


        //  Move over the vault object based off of the root motion forces.
        public override bool UpdateMovement()
        {
            //m_CapsuleCollider.center = m_ColliderCenter + (-Vector3.forward * m_CapsuleCollider.radius / 2);
            //m_Rigidbody.drag = 0;
            //m_CapsuleCollider.center = m_ColliderCenter + (-Vector3.forward * m_CapsuleCollider.radius / 2);
            var m_HeightDifference = (float)System.Math.Round(m_MatchPosition.y - m_Transform.position.y, 2);

            m_VerticalVelocity = Vector3.up * (m_PlatformHeight + m_VerticalMatchTargetOffset) * m_DeltaTime;
            if (m_HeightDifference >= 0.1f){
                m_Rigidbody.AddForce(m_VerticalVelocity * m_JumpForce, ForceMode.VelocityChange);
            }


            return false;
        }


        public override bool Move()
        {
            m_Animator.ApplyBuiltinRootMotion();

            m_Animator.MatchTarget(m_MatchPosition, Quaternion.LookRotation(m_Transform.forward, Vector3.up), m_AvatarTarget, m_MatchTargetWeightMask, m_StartMatchTarget, m_StopMatchTarget);

            m_Velocity = m_Animator.deltaPosition / m_DeltaTime;

            m_Velocity += m_VerticalVelocity;

            m_Rigidbody.velocity = m_Velocity;

            return false;
        }



        public override bool CanStopAction()
        {
            if ((m_EndPosition - m_Transform.position).sqrMagnitude < 0.2f){
                //Debug.LogFormat("{0} Action has stopped {1}", GetType().Name, Time.time);
                return true;
            }

            if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                return false;
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(m_StateName))
            {
                if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 - m_TransitionDuration)
                    return true;
                return false;
            }

            return m_StartTime + 1f < Time.time;
        }


        protected override void ActionStopped()
        {
            if(m_Animator.isMatchingTarget)
                m_Animator.InterruptMatchTarget();
            m_CapsuleCollider.center = m_ColliderCenter;
            m_CapsuleCollider.isTrigger = false;
            m_Rigidbody.useGravity = true;

            m_MatchPosition = Vector3.zero;
            //Debug.LogFormat("{0} Action has stopped", GetType().Name);
        }



        public override string GetDestinationState(int layer)
        {
            if (layer == 0)
                return m_StateName;
            return "";
        }






        private void OnDrawGizmos()
        {
            if (Application.isPlaying && m_IsActive)
            {
                Gizmos.color = Color.green;
                //  First raycast that checks if there's something in front.
                Gizmos.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToJumpDistance);

                Gizmos.color = Color.cyan;
                //  Second raycast for height checking.
                Gizmos.DrawRay(m_HeightCheckStart, Vector3.down * m_MaxHeight);



                if (m_MatchPosition != Vector3.zero)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(m_MatchPosition, 0.1f);
                    GizmosUtils.DrawString("Match Position", m_MatchPosition, Color.white);
                }
            }
        }


    }

}


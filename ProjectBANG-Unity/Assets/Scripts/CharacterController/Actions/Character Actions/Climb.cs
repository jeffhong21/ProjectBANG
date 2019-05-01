namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Climb : CharacterAction
    {
        public const int ACTION_ID = 16;

        protected float m_CheckHeight = 0.4f;
        [Header("-- Vault Settings --")]
        [SerializeField]
        protected float m_MoveToVaultDistance = 4f;
        [SerializeField, Tooltip("The highest level the character can climb.")]
        protected float m_MaxHeight = 3f;
        [SerializeField]
        protected float m_MinHeight = 1.5f;
        //[SerializeField]
        protected float m_JumpForce = 10f;
        [SerializeField]
        protected LayerMask m_CheckLayers;
        [SerializeField, Tooltip("The offset between the vault point and the point that the character should start to vault at")]
        protected float m_StartVaultOffset = 0.2f;
        [SerializeField, Tooltip("The offset between the vault point and the point that the character places their hands")]
        protected float m_MatchTargetOffset = 0.1f;
        [SerializeField]
        protected float m_StartMatchTarget = 0.01f;
        [SerializeField]
        protected float m_StopMatchTarget = 0.1f;
        [SerializeField]
        protected AvatarTarget m_AvatarTarget = AvatarTarget.RightHand;






        //[SerializeField]
        private Vector3 m_Velocity, m_VerticalVelocity;
        private float m_PlatformHeight;
        private Vector3 m_StartPosition, m_MatchPosition;
        private Vector3 m_HeightCheckStart;
        private RaycastHit DetectObjectHit, ObjectHeightHit;

        private MatchTargetWeightMask m_MatchTargetWeightMask = new MatchTargetWeightMask(Vector3.one, 1);
        private float m_StartTime;
        private float m_ColliderHeight;
        private Vector3 m_ColliderCenter;





        [SerializeField] bool m_Debug;


        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if (base.CanStartAction())
            {
                if (Physics.Raycast(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward, out DetectObjectHit, m_MoveToVaultDistance, m_CheckLayers))
                {
                    //if (m_Debug) Debug.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance, Color.green);
                    m_HeightCheckStart = DetectObjectHit.point;
                    m_HeightCheckStart.y += (m_MaxHeight + m_StartVaultOffset) - m_CheckHeight;

                    //if (m_Debug) Debug.DrawRay(heightCheckStart, Vector3.down * (m_MaxHeight - m_MinHeight), Color.cyan, 1f);
                    if (Physics.Raycast(m_HeightCheckStart, Vector3.down, out ObjectHeightHit, (m_MaxHeight - m_MinHeight), m_CheckLayers))
                    {
                        //  cache HeightCheckHit distance.
                        var heightCheckDist = ObjectHeightHit.distance;
                        if (heightCheckDist < m_MaxHeight)
                        {
                            //  Get the objet to vault over height.
                            m_PlatformHeight = m_MaxHeight - heightCheckDist;
                            //  Get the position of when the characters hand is placed on the object.
                            m_MatchPosition = ObjectHeightHit.point + (Vector3.up * m_MatchTargetOffset) + (m_Transform.forward * m_MatchTargetOffset);

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

            //m_Animator.SetInteger(HashID.ActionIntData, 2);
            //m_StateName = "Vault.Head";
            m_ColliderCenter = m_CapsuleCollider.center;

            m_StartTime = Time.time;
            //Debug.LogFormat("Playing:  {0}.  ColliderHeight is : {1}", stateNames[currentAnimIndex], m_PlatformHeight);
        }



        //  Move over the vault object based off of the root motion forces.
        public override bool UpdateMovement()
        {
            m_CapsuleCollider.center = m_ColliderCenter + (m_Transform.forward * m_CapsuleCollider.radius / 1);

            var m_HeightDifference = (float)System.Math.Round(m_MatchPosition.y - m_Transform.position.y, 2);


            //m_VerticalVelocity = m_Transform.up * (m_PlatformHeight + m_MatchTargetOffset) * m_DeltaTime;
            m_VerticalVelocity = m_Transform.up + (m_Transform.up * m_CapsuleCollider.height);
            m_VerticalVelocity = m_VerticalVelocity * m_DeltaTime;
            if (m_HeightDifference >= 0.1f)
                m_Rigidbody.AddForce(m_VerticalVelocity, ForceMode.VelocityChange);
            
            return true;
        }



        public override bool Move()
        {
            m_Animator.ApplyBuiltinRootMotion();

            m_Animator.MatchTarget(m_MatchPosition, Quaternion.LookRotation(m_Transform.forward, Vector3.up), m_AvatarTarget, m_MatchTargetWeightMask, m_StartMatchTarget, m_StopMatchTarget);

            m_Velocity = m_Animator.deltaPosition / m_DeltaTime;

            //m_Velocity += m_VerticalVelocity * m_JumpForce;
            m_Velocity += m_VerticalVelocity * 1;

            //m_Velocity.x = 0;
            //m_Velocity.z = 0;
            m_Rigidbody.velocity = m_Velocity;

            //Debug.LogFormat("Target Matching: {0}", m_Animator.isMatchingTarget);
            return false;
        }




        public override bool CanStopAction()
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                return false;
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(m_StateName))
            {
                if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 - m_TransitionDuration)
                    return true;
                return false;
            }

            return m_StartTime + 3f < Time.time;
        }


        protected override void ActionStopped()
        {
            m_CapsuleCollider.center = m_ColliderCenter;
            //m_CapsuleCollider.height = m_ColliderHeight;
            m_CapsuleCollider.isTrigger = false;
            m_Rigidbody.useGravity = true;

            m_StartPosition = m_MatchPosition = Vector3.zero;
            //Debug.LogFormat("{0} Action has stopped {1}", GetType().Name, Time.time);
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
                Gizmos.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance);

                Gizmos.color = Color.cyan;
                //  Second raycast for height checking.
                Gizmos.DrawRay(m_HeightCheckStart, Vector3.down * (m_MaxHeight - m_MinHeight));



                Gizmos.color = Color.green;
                Gizmos.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance);

                if (m_StartPosition != Vector3.zero)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(m_StartPosition, 0.2f);
                    GizmosUtils.DrawString("Start Position", m_StartPosition, Color.white);
                }
                if (m_MatchPosition != Vector3.zero)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(m_MatchPosition, 0.1f);
                    GizmosUtils.DrawString("End Position", m_MatchPosition, Color.white);
                }
            }
        }




    }

}


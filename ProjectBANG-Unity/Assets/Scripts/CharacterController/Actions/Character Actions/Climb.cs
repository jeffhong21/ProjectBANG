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
        [SerializeField]
        protected LayerMask m_ClimbableLayers;
        [SerializeField, Tooltip("The highest level the character can climb.")]
        protected float m_MaxClimbHeight = 3f;
        [SerializeField]
        protected float m_MinClimbHeight = 1.5f;
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

        float m_ClimbRate = 10f;

        //[SerializeField]
        Vector3 m_VerticalVelocity;


        //[SerializeField]
        private Vector3 m_Velocity;
        private float m_ClimbHeight;
        private Vector3 m_StartPosition;
        private Vector3 m_EndPosition;



        private Quaternion m_MatchRotation;
        private RaycastHit m_MoveToVaultDistanceHit, m_MatchPositionHit;
        private float m_StartTime;

        private float m_ColliderHeight;
        private Vector3 m_ColliderCenter;
        private Vector3 m_HeightCheckStart;

        private MatchTargetWeightMask m_MatchTargetWeightMask = new MatchTargetWeightMask(Vector3.one, 1);

        [SerializeField] bool m_Debug;


        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if (base.CanStartAction())
            {
                if (Physics.Raycast(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward, out m_MoveToVaultDistanceHit, m_MoveToVaultDistance, m_ClimbableLayers))
                {
                    //if (m_Debug) Debug.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance, Color.green);
                    return CachePositions();
                }
            }
            return false;
        }


        private bool CachePositions()
        {
            m_HeightCheckStart = m_MoveToVaultDistanceHit.point;
            m_HeightCheckStart.y += (m_MaxClimbHeight + m_StartVaultOffset) - m_CheckHeight;

            //if (m_Debug) Debug.DrawRay(heightCheckStart, Vector3.down * (m_MaxClimbHeight - m_MinClimbHeight), Color.cyan, 1f);
            if (Physics.Raycast(m_HeightCheckStart, Vector3.down, out m_MatchPositionHit, (m_MaxClimbHeight - m_MinClimbHeight), m_ClimbableLayers))
            {
                //  cache HeightCheckHit distance.
                var heightCheckDist = m_MatchPositionHit.distance;
                if (heightCheckDist < m_MaxClimbHeight)
                {
                    //  Get the objet to vault over height.
                    m_ClimbHeight = m_MaxClimbHeight - heightCheckDist;
                    //  Get the position of when the characters hand is placed on the object.
                    m_EndPosition = m_MatchPositionHit.point + (Vector3.up * m_MatchTargetOffset) + (m_Transform.forward * m_MatchTargetOffset);

                    return true;
                }

            }
            return false;
        }

        Quaternion targetRotation;
        protected override void ActionStarted()
        {
            //m_CapsuleCollider.isTrigger = true;
            //m_Rigidbody.useGravity = false;

            //m_Animator.SetInteger(HashID.ActionIntData, 2);
            //m_StateName = "Vault.Head";
            m_ColliderCenter = m_CapsuleCollider.center;
            targetRotation = m_Transform.rotation;
            m_StartTime = Time.time;
            //Debug.LogFormat("Playing:  {0}.  ColliderHeight is : {1}", stateNames[currentAnimIndex], m_ClimbHeight);
        }



        //  Move over the vault object based off of the root motion forces.
        public override bool UpdateMovement()
        {
            //m_CapsuleCollider.center = m_ColliderCenter + (-Vector3.forward * m_CapsuleCollider.radius / 2);

            var m_HeightDifference = (float)System.Math.Round(m_EndPosition.y - m_Transform.position.y, 2);


            m_VerticalVelocity = Vector3.up * (m_ClimbHeight + m_MatchTargetOffset) * m_DeltaTime;
            if (m_HeightDifference >= 0.1f)
                m_Rigidbody.AddForce(m_VerticalVelocity, ForceMode.VelocityChange);
            
            return true;
        }



        public override bool Move()
        {
            m_Animator.ApplyBuiltinRootMotion();
            m_Animator.MatchTarget(m_EndPosition, Quaternion.LookRotation(m_Transform.forward, Vector3.up), m_AvatarTarget, m_MatchTargetWeightMask, m_StartMatchTarget, m_StopMatchTarget);
            m_Velocity = m_Animator.deltaPosition / m_DeltaTime;

            m_Velocity += m_VerticalVelocity * m_ClimbRate;
            //m_Velocity.x = 0;
            //m_Velocity.z = 0;
            m_Rigidbody.velocity = m_Velocity;

            //Debug.LogFormat("Target Matching: {0}", m_Animator.isMatchingTarget);

            return false;
        }




        protected override void ActionStopped()
        {
            m_CapsuleCollider.center = m_ColliderCenter;
            //m_CapsuleCollider.height = m_ColliderHeight;
            m_CapsuleCollider.isTrigger = false;
            m_Rigidbody.useGravity = true;

            m_StartPosition = m_EndPosition = Vector3.zero;
            //Debug.LogFormat("{0} Action has stopped {1}", GetType().Name, Time.time);
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
                Gizmos.DrawRay(m_HeightCheckStart, Vector3.down * (m_MaxClimbHeight - m_MinClimbHeight));



                Gizmos.color = Color.green;
                Gizmos.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance);

                if (m_StartPosition != Vector3.zero)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(m_StartPosition, 0.2f);
                    GizmosUtils.DrawString("Start Position", m_StartPosition, Color.white);
                }
                if (m_EndPosition != Vector3.zero)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(m_EndPosition, 0.1f);
                    GizmosUtils.DrawString("End Position", m_EndPosition, Color.white);
                }
            }
        }




    }

}


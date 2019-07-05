namespace CharacterController
{
    using UnityEngine;



    public class Vault : CharacterAction
    {
        protected float m_CheckHeight = 0.4f;




        [SerializeField]
        protected float m_MoveToVaultDistance = 1f;
        [SerializeField]
        protected LayerMask m_VaultLayers;
        [SerializeField, Tooltip("The highest level the character can vault over.")]
        protected float m_MaxHeight = 2f;
        [SerializeField, Tooltip("How deep the character can vault over.")]
        protected float m_MaxVaultDepth = 1f;
        [SerializeField]
        public float m_MinMoveSpeed = 0.5f;
        [SerializeField]
        public float m_HorizontalOffset = 0.1f;
        [SerializeField]
        public float m_VerticalOffset;
        [SerializeField, Range(0.01f, 1)]
        public float m_StartMatchTarget = 0.01f;
        [SerializeField, Range(0.01f, 1)]
        public float m_StopMatchTarget = 0.1f;

        //[SerializeField]
        protected float m_JumpForce = 4f;
        protected float m_TimeToApex = 0.4f;
        protected float m_JumpVelocity;
        protected float m_MoveSpeed;
        protected float m_VelocitySmooth;
        protected float m_AccelerationTime = 0.1f;
        protected float m_Distance;




        float m_ColliderAnimHeight;
        //[SerializeField]
        private Vector3 m_Velocity;
        private float m_VaultObjectHeight;
        private Vector3 m_StartPosition, m_EndPosition, m_MatchPosition;

        private Quaternion m_MatchRotation;
        private RaycastHit m_MoveToVaultDistanceHit, m_MatchPositionHit, m_EndPositionHit;



        private MatchTargetWeightMask m_MatchTargetWeightMask = new MatchTargetWeightMask(Vector3.one, 0);

        private Vector3 m_HeightCheckStart;




        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if(base.CanStartAction() )
            {
                if (m_Controller.DetectObject(m_Transform.forward, out m_MoveToVaultDistanceHit, m_MoveToVaultDistance, m_VaultLayers))
                {
                    if (m_Debug) Debug.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance, Color.green);
                    return CachePositions();
                }

                //if (Physics.Raycast(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward, out m_MoveToVaultDistanceHit, m_MoveToVaultDistance, m_VaultLayers)){
                //    if (m_Debug) Debug.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance, Color.green);
                //    return CachePositions();
                //}
            }
            else if (m_Controller.GetAction<Sprint>().IsActive)
            {
                if (m_Controller.DetectObject(m_Transform.forward, out m_MoveToVaultDistanceHit, 2, m_VaultLayers))
                {
                    if (m_Debug) Debug.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance, Color.green);
                    return CachePositions();
                }
            }
            //if(m_MatchTargetStates.Length == 0)
                //Debug.LogFormat("No AnimatorStateMatchTarget");
            return false;
        }


        private bool CachePositions()
        {
            m_HeightCheckStart = m_MoveToVaultDistanceHit.point;
            m_HeightCheckStart.y += (m_MaxHeight + 0.2f) - m_CheckHeight;

            if (Physics.Raycast(m_HeightCheckStart, Vector3.down, out m_MatchPositionHit, m_MaxHeight, m_VaultLayers)){
                //  cache HeightCheckHit distance.
                var heightCheckDist = m_MatchPositionHit.distance;
                if (heightCheckDist < m_MaxHeight)
                {
                    //  Get the objet to vault over height.
                    m_VaultObjectHeight = m_MaxHeight - heightCheckDist;

                    Vector3 depthCheck = (m_MoveToVaultDistanceHit.point - (m_MoveToVaultDistanceHit.normal * m_MaxVaultDepth)) ;
                    depthCheck.y = m_HeightCheckStart.y;
                    //Debug.DrawRay(m_MoveToVaultDistanceHit.point, -m_MoveToVaultDistanceHit.normal * (m_MaxVaultDepth - m_CapsuleCollider.radius), Color.green, 3f);
                    if (Physics.Raycast(depthCheck, Vector3.down, out m_EndPositionHit, (m_MaxHeight + 0.2f), m_Layers.SolidLayers))
                    {
                        if (m_Debug) Debug.DrawRay(m_HeightCheckStart, Vector3.down * m_MaxHeight, Color.magenta, 1f); //  First depthCheck
                        if (m_Debug) Debug.DrawRay(m_MoveToVaultDistanceHit.point, -m_MoveToVaultDistanceHit.normal, Color.cyan, 3f);  //  Above depth
                        //if (m_Debug) Debug.DrawRay(m_MoveToVaultDistanceHit.point, -m_MoveToVaultDistanceHit.normal * (m_MaxVaultDepth - m_CapsuleCollider.radius), Color.green, 3f);
                        if (m_Debug) Debug.DrawRay(depthCheck, Vector3.down * (m_MaxHeight + 0.2f), Color.yellow, 3f);      //  Last raycast

                        //Debug.LogFormat("Distance {0}", Vector3.Distance(m_MoveToVaultDistanceHit.point, m_EndPositionHit.point));
                        return true;
                    }
                }

            }
            return false;
        }



        protected override void ActionStarted()
        {
            m_Animator.SetInteger(HashID.ActionID, (int)ActionTypeDefinition.Vault);

            m_EndPosition = m_EndPositionHit.point;
            m_StartPosition = m_MoveToVaultDistanceHit.point + (m_Transform.position - m_MoveToVaultDistanceHit.point);
            m_StartPosition.y = m_Transform.position.y;
            //  Get the position of when the characters hand is placed on the object.
            m_MatchPosition = m_MatchPositionHit.point + (Vector3.up * m_VerticalOffset + m_Transform.right * m_HorizontalOffset);
            //m_MatchPosition = m_MatchPosition + (m_Transform.forward * m_CapsuleCollider.radius);
            m_MatchPosition = m_MatchPosition + (m_Transform.forward * m_AccelerationTime);


            m_VaultObjectHeight = Mathf.Clamp(m_VaultObjectHeight, 0.4f, m_MaxHeight) + m_VerticalOffset;
            m_JumpForce = -(2 * m_VaultObjectHeight) / Mathf.Pow(0.4f, 2);
            m_JumpVelocity = Mathf.Abs(m_JumpForce) * m_TimeToApex;

            m_Velocity = m_Controller.Velocity;
            m_MoveSpeed = Mathf.Clamp(m_MoveSpeed, m_MinMoveSpeed, 8);
            m_Distance = Vector3.Distance(m_StartPosition, m_MoveToVaultDistanceHit.point);


            //  Cache variables
            m_ColliderHeight = m_CapsuleCollider.height;
            m_ColliderCenter = m_CapsuleCollider.center;

            m_Rigidbody.isKinematic = !m_Rigidbody.isKinematic;


            m_Velocity.y = m_JumpVelocity;
            m_Rigidbody.velocity = m_Velocity;
        }


        public override bool UpdateRotation()
        {
            var rotation = Quaternion.FromToRotation(-m_Transform.forward, m_MoveToVaultDistanceHit.normal) * m_Transform.rotation;
            m_Rigidbody.MoveRotation(Quaternion.Slerp(rotation, m_Rigidbody.rotation, 2 * m_DeltaTime).normalized);
            return false;
        }




        //  Move over the vault object based off of the root motion forces.
        public override bool UpdateMovement()
        {
            float targetVelocityX = Mathf.Abs(m_Distance) * m_MoveSpeed;
            m_Velocity.z = Mathf.SmoothDamp(m_Velocity.z, targetVelocityX, ref m_VelocitySmooth, m_AccelerationTime);
            m_Velocity.y += m_JumpForce * m_DeltaTime;
            m_Rigidbody.AddForce(m_Velocity * m_DeltaTime, ForceMode.VelocityChange);

            return false;
        }


		public override bool Move()
		{
            ////m_Animator.MatchTarget(m_MatchPosition, Quaternion.Euler(0, m_Transform.eulerAngles.y, 0), AvatarTarget.LeftHand, m_MatchTargetWeightMask, m_StartMatchTarget, m_StopMatchTarget);
            m_Animator.MatchTarget(m_MatchPosition, Quaternion.identity, AvatarTarget.LeftHand, m_MatchTargetWeightMask, m_StartMatchTarget, m_StopMatchTarget);

            return false;
		}




		protected override void ActionStopped()
        {
            m_Rigidbody.isKinematic = false;


            //m_CharacterIK.disableIK = false;
            m_CapsuleCollider.height = m_ColliderHeight;
            m_CapsuleCollider.center = m_ColliderCenter;
            m_StartPosition = m_MatchPosition = m_EndPosition = Vector3.zero;

            //Debug.LogFormat("{0} Action has stopped {1}", GetType().Name, Time.time);
        }


        public override bool CanStopAction()
        {
            //Debug.LogFormat("Distance Remaining: {0}",Vector3.Distance(m_Transform.position, m_EndPosition));

            if(m_Animator.GetNextAnimatorStateInfo(0).shortNameHash == 0){
                if(m_Animator.IsInTransition(0)){
                    Debug.LogFormat("{0} has stopped because it is entering Exit State", m_StateName);
                    return true;
                }
            }
            else if (m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(m_StateName)){
                if (m_Animator.IsInTransition(0)){
                    //Debug.LogFormat("{0} is exiting.", m_StateName);
                    return true;
                }
            }

            //bool safetyCheck = m_ActionStartTime + 3 < Time.time;
            //if(safetyCheck) Debug.LogFormat("{0} has stopped by safet check. | {1} : {2}", m_StateName, m_ActionStartTime, Time.time);
            return Time.time > m_ActionStartTime + 1;;
        }


        public override string GetDestinationState(int layer)
        {
            if (layer == 0)
            {
                if (m_Controller.GetAction<Sprint>().IsActive)
                    return "JumpOverObstacle-1H";
                else
                    return m_StateName;
            }
                
            return "";
        }







		private void OnDrawGizmos()
		{
            if(Application.isPlaying && m_IsActive){
                //Gizmos.color = Color.green;
                //Gizmos.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance);

                if(m_StartPosition != Vector3.zero){
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(m_StartPosition, 0.2f);
                    //GizmosUtils.DrawString("Start Position", m_StartPosition, Color.white);
                }
                if (m_MatchPosition != Vector3.zero)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(m_MatchPosition, 0.1f);
                    UnityEditor.Handles.color = Color.cyan;
                    UnityEditor.Handles.DrawSolidDisc(m_MatchPosition, Vector3.up, 0.1f);
                    //GizmosUtils.DrawString("Match Position", m_MatchPosition, Color.white);
                }
                if (m_EndPosition != Vector3.zero)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(m_EndPosition, 0.2f);
                }

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(m_Transform.position, m_EndPosition);
            }
		}






	}

}


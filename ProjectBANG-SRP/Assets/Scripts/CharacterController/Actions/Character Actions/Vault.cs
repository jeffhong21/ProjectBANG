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
        public float m_HorizontalOffset = 0.1f;
        [SerializeField]
        public float m_VerticalOffset;
        [SerializeField, Range(0.01f, 1)]
        public float m_StartMatchTarget = 0.01f;
        [SerializeField, Range(0.01f, 1)]
        public float m_StopMatchTarget = 0.1f;
        //[SerializeField]
        protected float m_JumpForce = 4f;

        //[SerializeField]
        Vector3 m_VerticalVelocity;
        //[SerializeField]
        float m_VerticalHeight;


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

                if (Physics.Raycast(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward, out m_MoveToVaultDistanceHit, m_MoveToVaultDistance, m_VaultLayers)){
                    if (m_Debug) Debug.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance, Color.green);
                    return CachePositions();
                }
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

            if(m_Debug) Debug.DrawRay(m_HeightCheckStart, Vector3.down * m_MaxHeight, Color.magenta, 1f);
            if (Physics.Raycast(m_HeightCheckStart, Vector3.down, out m_MatchPositionHit, m_MaxHeight, m_VaultLayers)){
                //  cache HeightCheckHit distance.
                var heightCheckDist = m_MatchPositionHit.distance;
                if (heightCheckDist < m_MaxHeight)
                {
                    //  Get the objet to vault over height.
                    m_VaultObjectHeight = m_MaxHeight - heightCheckDist;
                    if(m_Debug) Debug.DrawRay(m_MoveToVaultDistanceHit.point, -m_MoveToVaultDistanceHit.normal , Color.cyan, 3f);
                    Vector3 depthCheck = (m_MoveToVaultDistanceHit.point - (m_MoveToVaultDistanceHit.normal * m_MaxVaultDepth)) ;
                    //Debug.DrawRay(m_MoveToVaultDistanceHit.point, -m_MoveToVaultDistanceHit.normal * (m_MaxVaultDepth - m_CapsuleCollider.radius), Color.green, 3f);
                    depthCheck.y = m_HeightCheckStart.y;
                    //Debug.DrawRay(m_MoveToVaultDistanceHit.point, -m_MoveToVaultDistanceHit.normal * (m_MaxVaultDepth - m_CapsuleCollider.radius), Color.green, 3f);
                    if (m_Debug) Debug.DrawRay(depthCheck, Vector3.down * (m_MaxHeight + 0.2f), Color.yellow, 3f);
                    if (Physics.Raycast(depthCheck, Vector3.down, out m_EndPositionHit, (m_MaxHeight + 0.2f), m_Layers.GroundLayer))
                    {
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
            m_MatchPosition = m_MatchPosition + (m_Transform.forward * 0.1f);



            m_CharacterIK.disableIK = true;
            //  Cache variables
            m_ColliderHeight = m_CapsuleCollider.height;
            m_ColliderCenter = m_CapsuleCollider.center;

            m_Rigidbody.isKinematic = !m_Rigidbody.isKinematic;


            Vector3 verticalVelocity = Vector3.up * (Mathf.Sqrt(m_VaultObjectHeight * -2 * Physics.gravity.y));
            m_Rigidbody.velocity = verticalVelocity;

        }







        //  Move over the vault object based off of the root motion forces.
        public override bool UpdateMovement()
        {
            //  -----
            //  Rigidbody IsKinamatic is currently on, so physics movement will do nothing.
            //  -----
            //  
            float heightDifference = (float)System.Math.Round(m_MatchPosition.y - m_Transform.position.y, 2);

            float colliderScale = 1 / (m_VaultObjectHeight - heightDifference);
            colliderScale = Mathf.Clamp(colliderScale - 0.5f, 0.5f, 1);
            m_CapsuleCollider.height = Mathf.MoveTowards(m_CapsuleCollider.height, m_ColliderHeight * colliderScale, Time.deltaTime * 4);
            m_CapsuleCollider.center = Vector3.MoveTowards(m_CapsuleCollider.center, m_ColliderCenter * colliderScale, Time.deltaTime * 2);

            //var velocity = Vector3.Project(m_Rigidbody.velocity, Physics.gravity * 2);
            //var velocityY = Mathf.Sin()
            //if (Vector3.Dot(velocity, Physics.gravity) > 0)
                //velocityY = -velocityY;

            m_VerticalVelocity = Vector3.up * (m_VaultObjectHeight + m_VerticalOffset) ;
            m_VerticalVelocity = m_VerticalVelocity * m_DeltaTime;
            if (heightDifference >= 0.0f)
            {
                m_Rigidbody.AddForce(m_VerticalVelocity * 10, ForceMode.VelocityChange);
                //m_Rigidbody.AddForce(m_VerticalVelocity, ForceMode.VelocityChange);
                //m_Rigidbody.velocity = m_VerticalVelocity;
            }
            else{

                m_Rigidbody.AddForce(m_Transform.forward * m_CapsuleCollider.radius, ForceMode.VelocityChange);
            }


            return true;
        }


		public override bool Move()
		{
            //m_Animator.MatchTarget(m_MatchPosition, Quaternion.Euler(0, m_Transform.eulerAngles.y, 0), AvatarTarget.LeftHand, m_MatchTargetWeightMask, m_StartMatchTarget, m_StopMatchTarget);
            m_Animator.MatchTarget(m_MatchPosition, Quaternion.identity, AvatarTarget.LeftHand, m_MatchTargetWeightMask, m_StartMatchTarget, m_StopMatchTarget);

            m_Velocity = m_Animator.deltaPosition / m_DeltaTime;



            //m_Rigidbody.velocity = m_Velocity;
            m_Rigidbody.velocity = m_Velocity + m_VerticalVelocity;
            //Debug.LogFormat("Target Matching: {0}", m_Animator.isMatchingTarget);
            return true;
		}




		protected override void ActionStopped()
        {
            m_Rigidbody.isKinematic = !m_Rigidbody.isKinematic;


            m_CharacterIK.disableIK = false;
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






		//protected override void DrawOnGUI()
		//{
  //          //content.text = string.Format("State Name: {0}\n", Animator.StringToHash(m_StateName));
  //          //content.text += string.Format("ShortNameHash: {0}\n", m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash);

  //          //content.text = string.Format("Vertical Height: {0}\n", m_VerticalHeight.ToString());
  //          //content.text += string.Format("Height Difference: {0}\n", m_HeightDifference);
  //          //content.text += string.Format("Distance Remaing: {0}\n", (m_EndPosition - m_Transform.position).sqrMagnitude);
  //          //content.text += string.Format("Vertical Object Height: {0}\n", m_VaultObjectHeight);
  //          //content.text += string.Format("Vertical Velocity: {0}\n", m_VerticalVelocity);
  //          //content.text += string.Format("Rigidbody Velocity: {0}\n", m_Rigidbody.velocity);
  //          ////content.text += string.Format("Velocity: {0}\n", m_Velocity);
  //          //content.text += string.Format("Normalize Time: {0}\n", GetNormalizedTime());
  //          //content.text += string.Format("Matching Target: {0}\n", m_Animator.isMatchingTarget);

  //          GUILayout.Label(content);
		//}




	}

}


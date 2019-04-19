namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Vault : CharacterAction
    {
        public const int ACTION_ID = 15;

        protected float m_CheckHeight = 0.5f;

        [SerializeField]
        protected float m_MoveToVaultDistance = 2f;
        [SerializeField]
        protected LayerMask m_VaultLayers;
        [SerializeField]
        protected float m_MaxVaultHeight = 2f;
        [SerializeField]
        protected float m_MaxVaultDepth = 1f;
        [SerializeField]
        protected float m_StartVaultOffset = 0.2f;
        [SerializeField]
        protected float m_HorizontalMatchTargetOffset = -0.2f;
        [SerializeField]
        protected float m_VerticalMatchTargetOffset;
        [SerializeField]
        protected float m_StartMatchTarget;
        [SerializeField]
        protected float m_StopMatchTarget;



        [SerializeField]
        Vector3 m_VerticalVelocity;
        [SerializeField]
        float m_VerticalHeight;
        float m_HeightMultiplier = 2f;



        private Vector3 m_Velocity;
        private float m_VaultObjectHeight;
        private Vector3 m_StartPosition;
        private Vector3 m_EndPosition;
        private Vector3 m_MatchPosition;


        private Quaternion m_MatchRotation;
        private RaycastHit m_MoveToVaultDistanceHit, m_HeightCheckHit, m_EndPositionHit;
        private float m_StartTime;

        private float m_ColliderHeight;
        private Vector3 m_ColliderCenter;

        [Header("-----  Debug -----")]
        [SerializeField]
        private string[] stateNames = { "Base Layer.Vault.Waist", "Base Layer.Vault.Chest", "Base Layer.Vault.Head", "Base Layer.Vault.Box" };
        [SerializeField]
        private int currentAnimIndex = 2;


        //
        // Methods
        //

        public override bool CanStartAction()
        {
            if(base.CanStartAction())
            {
                ////Debug.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance, Color.green, 1f);
                //if(Physics.Raycast(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward, out m_MoveToVaultDistanceHit, m_MoveToVaultDistance, m_VaultLayers)){
                //    CachePositions();
                //    return true;
                //}
                return true;
            }
            return false;
        }


        private void CachePositions()
        {
            var heightCheckStart = m_MoveToVaultDistanceHit.point;
            heightCheckStart.x += (-m_StartVaultOffset);
            heightCheckStart.y = m_Transform.position.y + m_MaxVaultHeight + m_StartVaultOffset;
            heightCheckStart.z += m_StartVaultOffset;

            Debug.DrawRay(heightCheckStart, Vector3.down * heightCheckStart.y, Color.green, 3f);
            if (Physics.Raycast(heightCheckStart, Vector3.down, out m_HeightCheckHit, heightCheckStart.y, m_VaultLayers)){
                //  cache HeightCheckHit distance.
                var heightCheckDist = m_HeightCheckHit.distance;
                if (heightCheckDist < m_MaxVaultHeight){
                    //  Get the objet to vault over height.
                    m_VaultObjectHeight = m_MaxVaultHeight - heightCheckDist;
                    //  Get the position of when the characters hand is placed on the object.
                    m_MatchPosition = m_HeightCheckHit.point + Vector3.up * m_StartVaultOffset;

                    //Debug.DrawRay(m_MoveToVaultDistanceHit.point, -m_MoveToVaultDistanceHit.normal, Color.yellow, 3f);
                    var depthCheck = (-m_MoveToVaultDistanceHit.normal + m_MoveToVaultDistanceHit.point) * m_MaxVaultDepth;
                    depthCheck.y = heightCheckStart.y;

                    Debug.DrawRay(depthCheck, Vector3.down * heightCheckStart.y, Color.green, 3f);
                    if (Physics.Raycast(depthCheck, Vector3.down, out m_EndPositionHit, heightCheckStart.y, m_Layers.GroundLayer))
                    {
                        m_EndPosition = m_EndPositionHit.point;
                        m_StartPosition = m_Transform.position;
                    }
                }

            }
        }


        protected override void ActionStarted()
        {
            //  Cache variables
            m_ColliderHeight = m_CapsuleCollider.height;


            //currentAnimIndex = 0;
            m_StartTime = Time.time;
            Debug.LogFormat("Playing:  {0}.  ColliderHeight is : {1}", stateNames[currentAnimIndex], m_VaultObjectHeight);
        }




  //      //  Move over the vault object based off of the root motion forces.
  //      public override bool UpdateMovement()
  //      {
  //          m_VerticalHeight = Mathf.Clamp01(1 - Mathf.Abs(m_Animator.GetFloat(HashID.ColliderHeight)));

  //          m_CapsuleCollider.height = m_ColliderHeight * Mathf.Abs(m_Animator.GetFloat(HashID.ColliderHeight));


  //          m_VerticalHeight *= m_HeightMultiplier;
  //          m_VerticalHeight = (float)System.Math.Round(m_VerticalHeight, 2);

  //          m_VerticalVelocity = Vector3.up * (m_VerticalHeight * m_VaultObjectHeight) + Vector3.up * (m_VerticalHeight * m_StartVaultOffset);
  //          m_VerticalVelocity.x = m_Transform.position.x;
  //          m_VerticalVelocity.z = m_Transform.position.z;
  //          m_Transform.position = m_VerticalVelocity + (m_Rigidbody.velocity * Time.deltaTime) * m_Animator.GetFloat(HashID.ColliderHeight);


  //          return true;
  //      }


		//public override bool Move()
		//{
  //          m_Animator.MatchTarget(m_MatchPosition, m_Transform.rotation, AvatarTarget.RightHand, new MatchTargetWeightMask(Vector3.one, 1), m_StartMatchTarget, m_StopMatchTarget);
  //          //Debug.LogFormat("Target Matching: {0}", m_Animator.isMatchingTarget);
  //          return true;
		//}







		protected override void ActionStopped()
        {
            m_CapsuleCollider.height = m_ColliderHeight;
            m_StartPosition = m_MatchPosition = m_EndPosition = Vector3.zero;

            currentAnimIndex++;
            if (currentAnimIndex > stateNames.Length - 1) currentAnimIndex = 0;
            //Debug.LogFormat("{0} Action has stopped {1}", GetType().Name, Time.time);
        }


        public override bool CanStopAction()
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                return false;
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(stateNames[currentAnimIndex])){
                if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 - m_TransitionDuration)
                    return true;
                return false;
            }

            return m_StartTime + 3f < Time.time;
        }


        public override string GetDestinationState(int layer)
        {
            if(layer == 0)
                return stateNames[currentAnimIndex];
            return "";
        }













		private void OnDrawGizmos()
		{
            if(Application.isPlaying){
                Gizmos.color = Color.green;
                Gizmos.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance);

                if(m_StartPosition != Vector3.zero){
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(m_StartPosition, 0.2f);
                }
                if (m_MatchPosition != Vector3.zero)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(m_MatchPosition, 0.2f);
                }
                if (m_EndPosition != Vector3.zero)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(m_EndPosition, 0.2f);
                }

            }
		}











		//private float normalizedTime;
        //GUIStyle style = new GUIStyle();
        //GUIContent content = new GUIContent();
        //Vector2 size;
        //private void OnGUI()
        //{
        //    content.text = string.Format("( {0} )", normalizedTime.ToString());
        //    size = new GUIStyle(GUI.skin.label).CalcSize(content);
        //    GUILayout.BeginArea(new Rect(10, 15, size.x * 2, size.y * 2), GUI.skin.box);
        //    GUILayout.Label(content);
        //    //GUILayout.Label(string.Format("Normalized Time: {0}", normalizedTime.ToString()));
        //    GUILayout.EndArea();
        //}
    }

}


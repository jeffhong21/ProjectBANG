namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Vault : CharacterAction
    {
        public const int ACTION_ID = 15;

        protected float m_CheckHeight = 0.45f;

        [SerializeField]
        protected float m_MoveToVaultDistance = 2f;
        [SerializeField]
        protected LayerMask m_VaultLayers;
        [SerializeField, Tooltip("The highest level the character can vault over.")]
        protected float m_MaxVaultHeight = 1.5f;
        [SerializeField, Tooltip("How deep the character can vault over.")]
        protected float m_MaxVaultDepth = 1f;
        [SerializeField, Tooltip("The offset between the vault point and the point that the character should start to vault at")]
        protected float m_StartVaultOffset = 0.2f;
        [SerializeField, Tooltip("The offset between the vault point and the point that the character places their hands")]
        protected float m_MatchTargetOffset = 0.1f;
        [SerializeField]
        protected float m_StartMatchTarget;
        [SerializeField]
        protected float m_StopMatchTarget;



        //[SerializeField]
        Vector3 m_VerticalVelocity;
        //[SerializeField]
        float m_VerticalHeight;
        float m_HeightMultiplier = 2f;

        float m_ColliderAnimHeight;
        //[SerializeField]
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


        [SerializeField] bool m_Debug;


        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if(base.CanStartAction())
            {
                if (m_Debug) Debug.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance, Color.green, 3f);
                if(Physics.Raycast(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward, out m_MoveToVaultDistanceHit, m_MoveToVaultDistance, m_VaultLayers)){
                    CachePositions();
                    return true;
                }
                //return true;
            }
            return false;
        }


        private bool CachePositions()
        {
            var heightCheckStart = m_MoveToVaultDistanceHit.point;
            heightCheckStart.y += m_MaxVaultHeight - m_CheckHeight;


            if(m_Debug) Debug.DrawRay(heightCheckStart, Vector3.down * m_MaxVaultHeight, Color.cyan, 3f);

            if (Physics.Raycast(heightCheckStart, Vector3.down, out m_HeightCheckHit, m_MaxVaultHeight, m_VaultLayers)){
                //  cache HeightCheckHit distance.
                var heightCheckDist = m_HeightCheckHit.distance;
                if (heightCheckDist < m_MaxVaultHeight){
                    //  Get the objet to vault over height.
                    m_VaultObjectHeight = m_MaxVaultHeight - heightCheckDist;
                    //  Get the position of when the characters hand is placed on the object.
                    m_MatchPosition = m_HeightCheckHit.point + Vector3.up * m_StartVaultOffset;

                    //if(m_Debug) Debug.DrawRay(m_MoveToVaultDistanceHit.point, -m_MoveToVaultDistanceHit.normal, Color.yellow, 3f);

                    var depthCheck = (-m_MoveToVaultDistanceHit.normal + m_MoveToVaultDistanceHit.point) * m_MaxVaultDepth;
                    depthCheck.y = heightCheckStart.y;

                    if (m_Debug) Debug.DrawRay(depthCheck, Vector3.down * heightCheckStart.y, Color.green, 3f);
                    if (Physics.Raycast(depthCheck, Vector3.down, out m_EndPositionHit, m_MaxVaultHeight + m_StartVaultOffset, m_Layers.GroundLayer))
                    {
                        m_EndPosition = m_EndPositionHit.point;
                        m_StartPosition = (m_Transform.position - m_MoveToVaultDistanceHit.point) * m_MoveToVaultDistance;
                        //m_StartPosition = m_Transform.position;
                        return true;
                    }
                }

            }
            return false;
        }


        protected override void ActionStarted()
        {
            //  Cache variables
            m_Animator.applyRootMotion = true;
            m_ColliderHeight = m_CapsuleCollider.height;
            m_ColliderCenter = m_CapsuleCollider.center;

            if(m_VaultObjectHeight <= 0.7f){
                m_Animator.SetInteger(HashID.ActionIntData, 1);
                m_StateName = "Vault.Chest";
            }
            else if (m_VaultObjectHeight <= 0.8f)
            {
                m_Animator.SetInteger(HashID.ActionIntData, 2);
                m_StateName = "Vault.Head";
            }
            //else if (m_VaultObjectHeight <= 1.75f && m_VaultObjectHeight >= 1.15f)
            //{
            //    m_Animator.SetInteger(HashID.ActionIntData, 3);
            //    m_StateName = "Vault.Climb";
            //}
            else{
                m_StateName = "Vault";
            }



            m_StartTime = Time.time;
            //Debug.LogFormat("Playing:  {0}.  ColliderHeight is : {1}", stateNames[currentAnimIndex], m_VaultObjectHeight);
        }



        //  Move over the vault object based off of the root motion forces.
        public override bool UpdateMovement()
        {
            m_ColliderAnimHeight = m_Animator.GetFloat(HashID.ColliderHeight);
            m_VerticalHeight = Mathf.Clamp01(1 - Mathf.Abs(m_ColliderAnimHeight));

            m_CapsuleCollider.height = m_ColliderHeight * Mathf.Abs(m_ColliderAnimHeight);
            //m_CapsuleCollider.center.y 

            m_VerticalHeight *= m_HeightMultiplier;
            m_VerticalHeight = (float)System.Math.Round(m_VerticalHeight, 2);


            //m_VerticalVelocity = Vector3.up * (m_VerticalHeight * (m_VaultObjectHeight + m_MatchTargetOffset));
            m_VerticalVelocity = Vector3.up * (m_VerticalHeight * m_VaultObjectHeight);

            return true;
        }


		public override bool Move()
		{
            m_Animator.MatchTarget(m_MatchPosition, m_Transform.rotation, AvatarTarget.RightHand, new MatchTargetWeightMask(Vector3.one, 1), m_StartMatchTarget, m_StopMatchTarget);
            m_Velocity = m_Animator.deltaPosition / m_DeltaTime;
            //if (m_ColliderAnimHeight > 0) m_Velocity = m_Velocity * 0.5f;

            m_Velocity += m_VerticalVelocity;
            //m_Rigidbody.velocity = Vector3.SmoothDamp(m_Rigidbody.velocity, m_Velocity, ref m_SmoothVelocityDamp, 0.5f);
            m_Rigidbody.velocity = m_Velocity;

            //Debug.LogFormat("Target Matching: {0}", m_Animator.isMatchingTarget);
            return true;
		}




		protected override void ActionStopped()
        {
            m_Animator.applyRootMotion = false;
            m_CapsuleCollider.height = m_ColliderHeight;
            m_StartPosition = m_MatchPosition = m_EndPosition = Vector3.zero;

            //Debug.LogFormat("{0} Action has stopped {1}", GetType().Name, Time.time);
        }


        public override bool CanStopAction()
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                return false;
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(m_StateName)){
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
            if(Application.isPlaying && m_IsActive){
                Gizmos.color = Color.green;
                Gizmos.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance);

                if(m_StartPosition != Vector3.zero){
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(m_StartPosition, 0.2f);
                }
                if (m_MatchPosition != Vector3.zero)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(m_MatchPosition, 0.1f);
                    GizmosUtils.DrawString("Match Position", m_MatchPosition, Color.white);
                }
                if (m_EndPosition != Vector3.zero)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(m_EndPosition, 0.2f);
                }

            }
		}




        GUIStyle style = new GUIStyle();
        GUIContent content = new GUIContent();
        Vector2 size;
        //Color debugTextColor = new Color(0, 0.6f, 1f, 1);
        GUIStyle textStyle = new GUIStyle();
        Rect location = new Rect();
        private void OnGUI()
        {
            if (Application.isPlaying && m_IsActive)
            {
                GUI.color = Color.black;
                textStyle.fontStyle = FontStyle.Bold;

                content.text = string.Format("Vertical Height: {0}\n", m_VerticalHeight.ToString());
                content.text += string.Format("Vertical Object Height: {0}\n", m_VaultObjectHeight);
                content.text += string.Format("Vertical Velocity: {0}\n", m_VerticalVelocity);
                //content.text += string.Format("Velocity: {0}\n", m_Velocity);
                content.text += string.Format("Normalize Time: {0}\n", GetNormalizedTime());
                content.text += string.Format("Matching Target: {0}\n", m_Animator.isMatchingTarget);
                size = new GUIStyle(GUI.skin.label).CalcSize(content);
                location.Set(5, 15, size.x * 2, size.y * 2);
                GUILayout.BeginArea(location, GUI.skin.box);
                GUILayout.Label(content);
                //GUILayout.Label(string.Format("Normalized Time: {0}", normalizedTime.ToString()));
                GUILayout.EndArea();
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


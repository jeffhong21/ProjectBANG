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
        protected float m_MatchTargetOffset;
        [SerializeField]
        protected float m_StartMatchTarget;
        [SerializeField]
        protected float m_StopMatchTarget;



        private Vector3 m_StartPosition;
        private Vector3 m_EndPosition;
        private RaycastHit m_MoveToVaultDistanceHit;
        private RaycastHit m_HeightCheckHit;
        private RaycastHit m_EndPositionHit;
        private float m_StartTime;


        [Header("-----  Debug -----")]
        [SerializeField]
        private string[] stateNames = { "Base Layer.Vault.Waist", "Base Layer.Vault.Chest", "Base Layer.Vault.Head" };
        [SerializeField]
        private int currentAnimIndex;


        //
        // Methods
        //

        public override bool CanStartAction()
        {
            if(base.CanStartAction())
            {
                //Debug.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance, Color.green, 1f);
                if(Physics.Raycast(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward, out m_MoveToVaultDistanceHit, m_MoveToVaultDistance, m_VaultLayers)){
                    return true;
                }

            }
            return false;
        }


        protected override void ActionStarted()
        {
            var heightCheckStart = m_MoveToVaultDistanceHit.point;
            heightCheckStart.x += -m_StartVaultOffset;
            heightCheckStart.y = m_Transform.position.y + m_MaxVaultHeight + m_StartVaultOffset;
            heightCheckStart.z += m_StartVaultOffset;

            Debug.DrawRay(heightCheckStart, Vector3.down * heightCheckStart.y, Color.green, 3f);
            if (Physics.Raycast(heightCheckStart, Vector3.down, out m_HeightCheckHit, heightCheckStart.y, m_VaultLayers))
            {
                if(m_HeightCheckHit.distance < m_MaxVaultHeight)
                {
                    m_StartPosition = m_HeightCheckHit.point + Vector3.up * m_StartVaultOffset;

                    //Debug.DrawRay(m_MoveToVaultDistanceHit.point, -m_MoveToVaultDistanceHit.normal, Color.yellow, 3f);
                    var depthCheck = (-m_MoveToVaultDistanceHit.normal + m_MoveToVaultDistanceHit.point) * m_MaxVaultDepth;
                    depthCheck.y = heightCheckStart.y;

                    Debug.DrawRay(depthCheck, Vector3.down * heightCheckStart.y, Color.green, 3f);
                    if (Physics.Raycast(depthCheck, Vector3.down, out m_EndPositionHit, heightCheckStart.y, m_Layers.SolidLayer))
                    {
                        m_EndPosition = m_EndPositionHit.point;
                    }
                }
            }


            m_StartTime = Time.time;
            Debug.LogFormat("Playing:  {0}.", stateNames[currentAnimIndex]);
        }


        protected override void ActionStopped()
        {
            m_StartPosition = m_EndPosition = Vector3.zero;


            currentAnimIndex++;
            if (currentAnimIndex > stateNames.Length - 1) currentAnimIndex = 0;
            //Debug.LogFormat("{0} Action has stopped {1}", GetType().Name, Time.time);
        }


        public override bool UpdateMovement()
        {

            return false;
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

            return m_StartTime + 1f < Time.time;
        }


        //public override string GetDestinationState(int layer)
        //{
        //    if(layer == 0)
        //        return stateNames[currentAnimIndex];
        //    return "";
        //}













		private void OnDrawGizmos()
		{
            if(Application.isPlaying){
                Gizmos.color = Color.green;
                Gizmos.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance);

                if(m_StartPosition != Vector3.zero){
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(m_StartPosition, 0.2f);
                }
                if (m_EndPosition != Vector3.zero)
                {
                    Gizmos.color = Color.cyan;
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


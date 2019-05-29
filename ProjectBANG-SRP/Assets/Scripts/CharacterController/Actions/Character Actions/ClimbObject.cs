namespace CharacterController
{
    using UnityEngine;
    using System.Collections.Generic;


    public class ClimbObject : CharacterAction
    {
        protected float m_CheckHeight = 0.4f;

        //[Header("-- Climb Object Settings --")]
        [SerializeField]
        protected float m_MoveToClimbDistance = 0.5f;
        [SerializeField, Tooltip("The highest level the character can climb.")]
        protected float m_MaxHeight = 3f;
        [SerializeField]
        protected float m_MinHeight = 0.5f;
        [SerializeField]
        protected LayerMask m_CheckLayers;
        [SerializeField]
        protected AnimatorStateMatchTarget[] m_MatchTargetStates = new AnimatorStateMatchTarget[0];
        protected Dictionary<int, AnimatorStateMatchTarget> m_MatchTargetStatesLookup = new Dictionary<int, AnimatorStateMatchTarget>();


        //[SerializeField]
        private Vector3 m_Velocity, m_VerticalVelocity;
        private float m_PlatformHeight, m_HeightDifference;
        private Vector3 m_StartPosition, m_EndPosition;

        private RaycastHit ObjectHit, ObjectHeightHit;

        private MatchTargetWeightMask m_MatchTargetWeightMask = new MatchTargetWeightMask(Vector3.one, 0);
        private float m_StartTime;

        [SerializeField, DisplayOnly]
        private bool isMatchingTarget;
        [SerializeField, DisplayOnly]
        private float normalizeTime;



		//
		// Methods
		//

		protected override void Awake()
		{
            base.Awake();

            for (int i = 0; i < m_MatchTargetStates.Length; i++){
                int shortHash = Animator.StringToHash(m_MatchTargetStates[i].stateName);
                if(m_MatchTargetStatesLookup.ContainsKey(shortHash) == false ){
                    m_MatchTargetStatesLookup.Add(shortHash, m_MatchTargetStates[i]);
                }
            }
        }



		public override bool CanStartAction()
        {
            if (base.CanStartAction())
            {
                if (Physics.Raycast(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward, out ObjectHit, m_MoveToClimbDistance, m_CheckLayers))
                {
                    //if (m_Debug) Debug.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance, Color.green);
                    Vector3 heightCheckStart = ObjectHit.point;
                    //  Add a little buffer to max height.
                    heightCheckStart.y += (m_MaxHeight + 0.1f) - m_CheckHeight;
                    heightCheckStart += m_Transform.forward * m_CapsuleCollider.radius;

                    Debug.DrawRay(heightCheckStart, Vector3.down * (m_MaxHeight - m_MinHeight), Color.cyan, 1f);
                    if (Physics.Raycast(heightCheckStart, Vector3.down, out ObjectHeightHit, (m_MaxHeight - m_MinHeight), m_CheckLayers)){
                        if (ObjectHeightHit.distance < m_MaxHeight){
                            return true;
                        }
                    }
                }
            }


            return false;
        }





        protected override void ActionStarted()
        {
            m_StartPosition = m_Transform.position;
            m_EndPosition = ObjectHeightHit.point;
            //  Get the objet to vault over height.
            m_PlatformHeight = (m_MaxHeight + 0.1f) - ObjectHeightHit.distance;

            //  Cache variables
            m_ColliderHeight = m_CapsuleCollider.height;
            m_ColliderCenter = m_CapsuleCollider.center;

            //m_Rigidbody.isKinematic = !m_Rigidbody.isKinematic;




            Vector3 verticalVelocity = Vector3.up * (Mathf.Sqrt((m_PlatformHeight) * -2 * Physics.gravity.y));
            //m_Rigidbody.velocity = verticalVelocity;
            //m_Rigidbody.AddForce(verticalVelocity, ForceMode.VelocityChange);
            //Debug.LogFormat("m_PlatformHeight is:  {0}.  m_HeightDifference is : {1}", m_PlatformHeight, m_HeightDifference);
        }


		public override bool UpdateRotation()
		{
            float angle = Vector3.Angle(m_Transform.forward, -ObjectHit.normal);
            //if(angle != 0){
            //    //Quaternion rotation = Quaternion.AngleAxis(angle, m_Transform.up) * m_Transform.rotation;
            //    //m_Rigidbody.MoveRotation(Quaternion.AngleAxis(angle * m_DeltaTime * 20, m_Transform.up) * m_Rigidbody.rotation);
            //    m_Rigidbody.MoveRotation(Quaternion.AngleAxis(angle, m_Transform.up) * m_Rigidbody.rotation);
            //}

            Quaternion rotation = Quaternion.FromToRotation(m_Transform.forward, -ObjectHit.normal) * m_Rigidbody.rotation;
            m_Rigidbody.MoveRotation(Quaternion.Slerp(rotation, m_Rigidbody.rotation, m_DeltaTime * 10));

            return false;
		}


		//  Move over the vault object based off of the root motion forces.
		public override bool UpdateMovement()
        {
            Vector3 verticalVelocity = Vector3.up * (Mathf.Sqrt((m_PlatformHeight) * -2 * Physics.gravity.y));

            float distance = ObjectHeightHit.point.y - m_Rigidbody.position.y;
            float percent = (m_PlatformHeight - distance) / m_PlatformHeight;

            if(m_Animator.isMatchingTarget == false)
            {
                if (distance <= 0.12f)
                {
                    m_Rigidbody.isKinematic = false;
                    Vector3 position = Vector3.Lerp(m_Rigidbody.position, m_EndPosition, percent);
                    m_Rigidbody.MovePosition(Vector3.Lerp(m_Rigidbody.position, m_EndPosition, percent * 0.25f));
                }
                else
                {
                    //m_Controller.InputVector = m_Transform.forward;
                    //m_Transform.position = new Vector3(m_Transform.position.x, m_Transform.position.y, m_StartPosition.z);
                    //m_Rigidbody.position = new Vector3(m_Rigidbody.position.x, m_Rigidbody.position.y + m_CapsuleCollider.radius, m_StartPosition.z);
                    m_Rigidbody.MovePosition(Vector3.Lerp(m_Rigidbody.position, new Vector3(m_Rigidbody.position.x, ObjectHeightHit.point.y, m_StartPosition.z), percent * 0.25f));

                }
            }


            return false;
        }


        Vector3 matchTarget;
        public override bool Move()
        {
            if (m_MatchTargetStatesLookup.ContainsKey(m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash))
            {
                AnimatorStateMatchTarget matchState = m_MatchTargetStatesLookup[m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash];
                m_Animator.MatchTarget(ObjectHeightHit.point + matchState.matchTargetOffset, Quaternion.identity, matchState.avatarTarget, m_MatchTargetWeightMask, matchState.startMatchTarget, matchState.stopMatchTarget);
                //Debug.Log(matchState.stateName + " | normalized time: " + m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            }

            //isMatchingTarget = m_Animator.isMatchingTarget;
            //normalizeTime = Mathf.Repeat(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f);
            //if(m_Animator.isMatchingTarget == false){
            //    if(m_MatchTargetStatesLookup.ContainsKey(m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash)){
            //        AnimatorStateMatchTarget matchState = m_MatchTargetStatesLookup[m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash];
            //        m_Animator.MatchTarget(ObjectHeightHit.point + matchState.matchTargetOffset, Quaternion.identity, matchState.avatarTarget, m_MatchTargetWeightMask, matchState.startMatchTarget, matchState.stopMatchTarget);
            //    }
            //}
            //else{
            //    m_Velocity = m_Animator.deltaPosition / m_DeltaTime;

            //    m_Velocity += m_VerticalVelocity;

            //    m_Rigidbody.velocity = m_Velocity;
            //}
            m_Velocity = m_Animator.deltaPosition / m_DeltaTime;
            m_Rigidbody.velocity = m_Velocity;
            //Debug.LogFormat("Target Matching: {0}", m_Animator.isMatchingTarget);
            return false;
        }




        public override bool CanStopAction()
        {
            if (m_MatchTargetStatesLookup.ContainsKey(m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash))
            {
                if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f - m_TransitionDuration)
                {
                    //Debug.LogFormat("{0} has stopped by comparing nameHASH", m_MatchTargetState.stateName);
                    return true;
                }
            }


            //if (m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(m_MatchTargetState.stateName))
            //{
            //    if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f - m_TransitionDuration){
            //        //Debug.LogFormat("{0} has stopped by comparing nameHASH", m_MatchTargetState.stateName);
            //        return true;
            //    }
            //    return false;
            //}
            if (Vector3.Distance(m_Rigidbody.position, m_EndPosition) <= 0.1f)
                return true;
            
            return Time.time > m_ActionStartTime + 2.5;
            //return m_ActionStartTime + 2 < Time.time;
        }


        protected override void ActionStopped()
        {
            if (m_Animator.isMatchingTarget)
                m_Animator.InterruptMatchTarget();

            m_Rigidbody.isKinematic = false;

            m_CapsuleCollider.center = m_ColliderCenter;
            m_CapsuleCollider.height = m_ColliderHeight;

            m_EndPosition = Vector3.zero;
            //m_StartPosition = m_MatchPosition = m_EndPosition = Vector3.zero;
            //Debug.LogFormat("{0} Action has stopped {1}", GetType().Name, Time.time);
        }


        public override string GetDestinationState(int layer)
        {
            if (layer == 0){
                if (m_PlatformHeight < 1.5f)
                    return "Climb-1M";
                else
                    return "Climb-2M";
            }
            return "";
        }















        private void OnDrawGizmos()
        {
            if (Application.isPlaying && m_IsActive)
            {

                if (m_EndPosition != Vector3.zero)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(m_EndPosition, 0.1f);
                }
            }
        }


        //protected override void DrawOnGUI()
        //{
        //    content.text = "";
        //    content.text += string.Format("Matching Target: {0}\n", m_Animator.isMatchingTarget);
        //    content.text += string.Format("ShortNameHash: {0}\n", m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
        //    content.text += string.Format("StateName({0}) Hash: {1}\n",m_MatchTargetState.stateName, Animator.StringToHash(m_MatchTargetState.stateName));
        //    content.text += string.Format("Platform Height: {0}\n", m_PlatformHeight);
        //    content.text += string.Format("Height Difference: {0}\n", m_HeightDifference);
        //    //content.text += string.Format("Clip: {0}\n", (m_StartTime + m_Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length) - Time.time);

        //    GUILayout.Label(content);
        //}

    }

}


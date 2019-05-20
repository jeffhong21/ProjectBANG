//namespace CharacterController
//{
//    using UnityEngine;
//    using System.Collections;


//    public class ClimbObject : CharacterAction
//    {
//        protected float m_CheckHeight = 0.4f;

//        [Header("-- Climb Object Settings --")]
//        [SerializeField]
//        protected float m_MoveToClimbDistance = 0.5f;
//        [SerializeField, Tooltip("The highest level the character can climb.")]
//        protected float m_MaxHeight = 3f;
//        [SerializeField]
//        protected float m_MinHeight = 0.5f;
//        [SerializeField]
//        protected LayerMask m_CheckLayers;
//        [SerializeField]
//        protected AnimatorStateMatchTarget[] m_MatchTargetStates = new AnimatorStateMatchTarget[0];
//        private AnimatorStateMatchTarget m_MatchTargetState;



//        //[SerializeField]
//        private Vector3 m_Velocity, m_VerticalVelocity;
//        private float m_PlatformHeight, m_HeightDifference;
//        private Vector3 m_StartPosition, m_MatchPosition, m_EndPosition;
//        private Vector3 m_HeightCheckStart;
//        private RaycastHit DetectObjectHit, ObjectHeightHit;

//        private MatchTargetWeightMask m_MatchTargetWeightMask = new MatchTargetWeightMask(Vector3.one, 0);
//        private float m_StartTime;






//        [SerializeField] bool m_Debug;


//        //
//        // Methods
//        //
//        public override bool CanStartAction()
//        {
//            if (base.CanStartAction() && m_MatchTargetStates.Length > 0)
//            {
//                if (Physics.Raycast(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward, out DetectObjectHit, m_MoveToClimbDistance, m_CheckLayers))
//                {
//                    //if (m_Debug) Debug.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToVaultDistance, Color.green);
//                    m_HeightCheckStart = DetectObjectHit.point;
//                    m_HeightCheckStart.y += (m_MaxHeight + 0.2f) - m_CheckHeight;

//                    //if (m_Debug) Debug.DrawRay(heightCheckStart, Vector3.down * (m_MaxHeight - m_MinHeight), Color.cyan, 1f);
//                    if (Physics.Raycast(m_HeightCheckStart, Vector3.down, out ObjectHeightHit, (m_MaxHeight - m_MinHeight), m_CheckLayers))
//                    {
//                        if (ObjectHeightHit.distance < m_MaxHeight)
//                        {
//                            return true;
//                        }
//                    }
//                }
//            }
//            if (m_MatchTargetStates.Length == 0) Debug.LogFormat("No AnimatorStateMatchTarget");

//            return false;
//        }





//        protected override void ActionStarted()
//        {
//            //  Get the objet to vault over height.
//            m_PlatformHeight = m_MaxHeight - ObjectHeightHit.distance;
//            var stateIndex = 0;
//            for (int i = 0; i < m_MatchTargetStates.Length; i++)
//            {
//                if (m_PlatformHeight > m_MatchTargetStates[i].threshold)
//                {
//                    m_MatchTargetState = m_MatchTargetStates[i];
//                    stateIndex = i;
//                }
//            }
//            if (m_MatchTargetState == null) m_MatchTargetState = m_MatchTargetStates[0];

//            //  Get the position of when the characters hand is placed on the object.
//            m_MatchPosition = ObjectHeightHit.point + (Vector3.up * m_MatchTargetState.matchTargetOffset.y) + (m_Transform.right * m_MatchTargetState.matchTargetOffset.x);
//            m_EndPosition = ObjectHeightHit.point + (Vector3.up * m_MatchTargetState.matchTargetOffset.y) + (m_Transform.forward * m_CapsuleCollider.radius);

//            //  Cache variables
//            m_ColliderHeight = m_CapsuleCollider.height;
//            m_ColliderCenter = m_CapsuleCollider.center;

//            m_Rigidbody.isKinematic = !m_Rigidbody.isKinematic;

//            m_Animator.SetInteger(HashID.ActionIntData, stateIndex);
//            m_StartTime = Time.time;
//            m_HeightDifference = (float)System.Math.Round(m_MatchPosition.y - m_Transform.position.y, 2);

//            Vector3 verticalVelocity = Vector3.up * (Mathf.Sqrt(m_PlatformHeight * -2 * Physics.gravity.y));
//            m_Rigidbody.velocity = verticalVelocity;
//            //m_Rigidbody.AddForce(verticalVelocity, ForceMode.VelocityChange);
//            Debug.LogFormat("m_PlatformHeight is:  {0}.  m_HeightDifference is : {1}", m_PlatformHeight, m_HeightDifference);

//        }



//        //  Move over the vault object based off of the root motion forces.
//        public override bool UpdateMovement()
//        {
//            //m_CapsuleCollider.center = m_ColliderCenter + (Vector3.forward * m_CapsuleCollider.radius / 2);

//            m_HeightDifference = (float)System.Math.Round(m_MatchPosition.y - m_Transform.position.y, 2);


//            //m_VerticalVelocity = m_Transform.up * (m_PlatformHeight + m_MatchTargetOffset) * m_DeltaTime;
//            //m_VerticalVelocity =(m_Transform.up * (m_PlatformHeight + m_MatchTargetState.verticalMatchOffset)) + (m_Transform.up * m_CapsuleCollider.height);

//            //m_VerticalVelocity = m_Transform.up + (m_Transform.up * m_CapsuleCollider.height);

//            m_VerticalVelocity = Vector3.up * (m_PlatformHeight + m_CapsuleCollider.height);

//            //m_VerticalVelocity = m_VerticalVelocity * (m_PlatformHeight * 4);
//            m_VerticalVelocity = m_VerticalVelocity * m_DeltaTime;
//            if (m_HeightDifference >= 0.0f)
//                m_Rigidbody.AddForce(m_VerticalVelocity * (10), ForceMode.VelocityChange);
//            //else
//            ////m_Rigidbody.AddForce((m_Transform.forward * m_CapsuleCollider.radius) * m_DeltaTime, ForceMode.VelocityChange);
//            //m_Rigidbody.AddForce(m_Transform.forward * m_CapsuleCollider.radius * 10, ForceMode.VelocityChange);

//            return false;
//        }



//        public override bool Move()
//        {
//            //if(m_Animator.isMatchingTarget == false){
//            //    for (int i = 0; i < m_MatchTargetStates.Length; i++)
//            //    {
//            //        if (m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(m_MatchTargetStates[i].stateName))
//            //        {
//            //            Debug.DrawLine(m_Animator.bodyPosition, m_MatchPosition, Color.blue);
//            //            //m_Animator.MatchTarget(m_MatchPosition, Quaternion.LookRotation(m_Transform.forward, Vector3.up), m_MatchTargetState.avatarTarget, m_MatchTargetWeightMask, m_MatchTargetState.startMatchTarget, m_MatchTargetState.stopMatchTarget);
//            //        }
//            //    }
//            //}
//            //if(m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(m_MatchTargetState.stateName)){
//            //    Debug.DrawLine(m_Animator.bodyPosition, m_MatchPosition, Color.blue);
//            //    //m_Animator.MatchTarget(m_MatchPosition, Quaternion.LookRotation(m_Transform.forward, Vector3.up), m_MatchTargetState.avatarTarget, m_MatchTargetWeightMask, m_MatchTargetState.startMatchTarget, m_MatchTargetState.stopMatchTarget);
//            //}
//            //m_Animator.MatchTarget(m_MatchPosition, Quaternion.LookRotation(m_Transform.forward, Vector3.up) , m_MatchTargetState.avatarTarget, m_MatchTargetWeightMask, m_MatchTargetState.startMatchTarget, m_MatchTargetState.stopMatchTarget);
//            m_Animator.MatchTarget(m_MatchPosition, Quaternion.identity, m_MatchTargetState.avatarTarget, m_MatchTargetWeightMask, m_MatchTargetState.startMatchTarget, m_MatchTargetState.stopMatchTarget);


//            //m_Velocity = m_Animator.deltaPosition / m_DeltaTime;

//            //m_Velocity += m_VerticalVelocity;

//            //m_Rigidbody.velocity = m_Velocity;

//            //Debug.LogFormat("Target Matching: {0}", m_Animator.isMatchingTarget);
//            return false;
//        }




//        public override bool CanStopAction()
//        {
//            if (m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(m_MatchTargetState.stateName))
//            {
//                if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f - m_TransitionDuration)
//                {
//                    //Debug.LogFormat("{0} has stopped by comparing nameHASH", m_MatchTargetState.stateName);
//                    return true;
//                }
//                return false;
//            }
//            //float clipTime = m_Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
//            return m_StartTime + 3 < Time.time;
//            //return m_StartTime + clipTime < Time.time;
//        }


//        protected override void ActionStopped()
//        {
//            if (m_Animator.isMatchingTarget)
//                m_Animator.InterruptMatchTarget();

//            m_Rigidbody.isKinematic = !m_Rigidbody.isKinematic;

//            m_CapsuleCollider.center = m_ColliderCenter;
//            m_CapsuleCollider.height = m_ColliderHeight;


//            m_StartPosition = m_MatchPosition = m_EndPosition = Vector3.zero;
//            //Debug.LogFormat("{0} Action has stopped {1}", GetType().Name, Time.time);
//        }


//        public override string GetDestinationState(int layer)
//        {
//            if (layer == 0)
//                //return m_StateName + "." + m_MatchTargetState.stateName;
//                return m_MatchTargetState.stateName;

//            return "";
//        }















//        private void OnDrawGizmos()
//        {
//            if (Application.isPlaying && m_IsActive)
//            {
//                Gizmos.color = Color.green;
//                //  First raycast that checks if there's something in front.
//                Gizmos.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToClimbDistance);

//                Gizmos.color = Color.cyan;
//                //  Second raycast for height checking.
//                Gizmos.DrawRay(m_HeightCheckStart, Vector3.down * (m_MaxHeight - m_MinHeight));


//                Gizmos.color = Color.green;
//                Gizmos.DrawRay(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward * m_MoveToClimbDistance);

//                if (m_StartPosition != Vector3.zero)
//                {
//                    Gizmos.color = Color.magenta;
//                    Gizmos.DrawWireSphere(m_StartPosition, 0.2f);
//                    GizmosUtils.DrawString("Start Position", m_StartPosition, Color.white);
//                }
//                if (m_MatchPosition != Vector3.zero)
//                {
//                    Gizmos.color = Color.cyan;
//                    Gizmos.DrawWireSphere(m_MatchPosition, 0.1f);
//                    GizmosUtils.DrawString("End Position", m_MatchPosition, Color.white);
//                }
//            }
//        }


//        protected override void DrawOnGUI()
//        {
//            content.text = "";
//            content.text += string.Format("Matching Target: {0}\n", m_Animator.isMatchingTarget);
//            content.text += string.Format("ShortNameHash: {0}\n", m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
//            content.text += string.Format("StateName({0}) Hash: {1}\n", m_MatchTargetState.stateName, Animator.StringToHash(m_MatchTargetState.stateName));
//            content.text += string.Format("Platform Height: {0}\n", m_PlatformHeight);
//            content.text += string.Format("Height Difference: {0}\n", m_HeightDifference);
//            //content.text += string.Format("Clip: {0}\n", (m_StartTime + m_Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length) - Time.time);

//            GUILayout.Label(content);
//        }

//    }

//}


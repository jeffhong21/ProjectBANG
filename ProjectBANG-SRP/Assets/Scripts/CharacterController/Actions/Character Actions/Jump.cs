namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Jump : CharacterAction
    {
        //public const int ACTION_ID = 15;
        protected string m_DestinationState = "JumpRunStart_RU";

        string clipName;
        AnimatorClipInfo[] currentClipInfo;
        float currentClipLength;

        Vector3 m_StartPosition;
        Vector3 m_CenterPosition;
        Vector3 m_EndPosition;

		//
		// Methods
		//
        public override bool CanStartAction()
        {
            if (base.CanStartAction())
            {
                return false;
            }
            return false;
		}

		protected override void ActionStarted()
        {
            currentClipInfo = m_Animator.GetCurrentAnimatorClipInfo(0);
            currentClipLength = currentClipInfo[0].clip.length;
            clipName = currentClipInfo[0].clip.name;

            m_TransitionDuration = 0.1f;


            m_StartPosition = m_Transform.position;
            m_EndPosition = m_Transform.forward * 3;
            m_CenterPosition = (m_StartPosition + m_EndPosition) * 0.5f;
            //m_CenterPosition = m_Transform.forward * 2;

            m_Rigidbody.isKinematic = !m_Rigidbody.isKinematic;
            m_Rigidbody.detectCollisions = !m_Rigidbody.detectCollisions;
            m_CapsuleCollider.isTrigger = !m_CapsuleCollider.isTrigger;
            m_Animator.applyRootMotion = !m_Animator.applyRootMotion;
        }


        public override bool CanStopAction()
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(m_DestinationState))
            {
                if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f - m_TransitionDuration)
                {
                    return true;
                }
            }


            return false;
        }


        protected override void ActionStopped()
        {
            m_StartPosition = m_EndPosition = m_CenterPosition = Vector3.zero;

            m_Rigidbody.isKinematic = !m_Rigidbody.isKinematic;
            m_Rigidbody.detectCollisions = !m_Rigidbody.detectCollisions;
            m_CapsuleCollider.isTrigger = !m_CapsuleCollider.isTrigger;
            m_Animator.applyRootMotion = !m_Animator.applyRootMotion;;
        }

        float startTime;
        float fracComplete;
        float journeyTime;
        float speed = 1;
        Vector3 startRelCenter, endRelCenter;
		public override bool UpdateMovement()
		{
            currentClipInfo = m_Animator.GetCurrentAnimatorClipInfo(0);
            currentClipLength = currentClipInfo[0].clip.length;
            clipName = currentClipInfo[0].clip.name;

            if(m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(m_DestinationState))
            {
                if(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.2f){
                    startTime = Time.time;
                };
                m_CenterPosition = (m_StartPosition + m_EndPosition) * 0.5f;
                m_CenterPosition -= Vector3.up;

                startRelCenter = m_StartPosition - m_CenterPosition;
                endRelCenter = m_EndPosition - m_CenterPosition;

                Debug.DrawRay(m_StartPosition, startRelCenter, Color.yellow);
                Debug.DrawRay(m_EndPosition, endRelCenter, Color.cyan);

                journeyTime = currentClipLength;

                fracComplete = (Time.time - startTime) / journeyTime * speed;
                m_Transform.position = Vector3.Slerp(startRelCenter, endRelCenter, fracComplete * speed);
                m_Transform.position += m_CenterPosition / 2;
            }




            return false;
		}



		//  Returns the state the given layer should be on.
		public override string GetDestinationState(int layer)
        {
            if (layer == 0){
                return m_DestinationState;
            }
            return "";
        }



        private void OnDrawGizmos()
        {
            if (Application.isPlaying && m_IsActive)
            {

                if (m_StartPosition != Vector3.zero)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(startRelCenter, 0.2f);
                    Gizmos.DrawWireSphere(m_StartPosition, 0.2f);

                }
                if (m_CenterPosition != Vector3.zero)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(m_CenterPosition, 0.1f);
                }
                if (m_EndPosition != Vector3.zero)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(endRelCenter, 0.2f);
                    Gizmos.DrawWireSphere(m_EndPosition, 0.2f);
                }

            }
        }



        protected override void DrawOnGUI()
        {
            content.text = string.Format("Clip Info Length: {0}\n", currentClipInfo.Length);
            content.text += string.Format("Clip Name: {0}\n", clipName);
            content.text += string.Format("Clip Length: {0}\n", currentClipLength);
            content.text += string.Format("Next State: {0}\n", m_Animator.GetNextAnimatorStateInfo(0).shortNameHash);
            content.text += string.Format("State Name: {0}\n", Animator.StringToHash(m_DestinationState));
            content.text += string.Format("ShortNameHash: {0}\n", m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
            content.text += string.Format("In Transition?: {0}\n", m_Animator.IsInTransition(0));
            content.text += string.Format("Transition Norm Time: {0}\n", m_Animator.GetAnimatorTransitionInfo(0).normalizedTime);
            content.text += string.Format("Start Time: {0} | Frac {1}\n", startTime, fracComplete);
            content.text += string.Format("Start Time: {0} | Frac {1}\n", journeyTime, fracComplete);
            GUILayout.Label(content);
        }
    }

}


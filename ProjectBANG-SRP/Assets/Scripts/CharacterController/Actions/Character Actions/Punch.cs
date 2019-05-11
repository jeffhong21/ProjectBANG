namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Punch : CharacterAction
    {
        [SerializeField]
        protected Collider[] m_Hitboxes = new Collider[0];
        [SerializeField]
        protected string m_ExitStateName;


        protected int ActionIntData;

        //
        // Methods
        //



        public override bool CanStartAction()
        {
            return base.CanStartAction();
        }


        protected override void ActionStarted()
        {
            ActionIntData = 3;
            m_Animator.SetInteger(HashID.ActionIntData, ActionIntData);
        }


		public override bool Move()
		{
            m_Animator.ApplyBuiltinRootMotion();
            return false;
		}


		public override bool UpdateAnimator()
		{
            if (Input.GetKeyDown(m_KeyCodes[m_InputIndex]) && m_Animator.IsInTransition(0) == false)
            {
                if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f){
                    m_Animator.SetTrigger(Animator.StringToHash("MeleeAttack"));
                }
            }
            else if (m_Animator.IsInTransition(0) == true){
                m_Animator.ResetTrigger(Animator.StringToHash("MeleeAttack"));
            }

            return base.UpdateAnimator();
		}


		public override bool CanStopAction()
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(m_ExitStateName))
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
            //Debug.Log("Shooting action done");
        }


        public override string GetDestinationState(int layer)
        {
            if (layer == 0)
                return m_StateName;
            return "";
        }






        //protected override void DrawOnGUI()
        //{
        //    content.text = string.Format("Current State Length: {0}\n", m_Animator.GetCurrentAnimatorStateInfo(0).length);
        //    content.text += string.Format("Current State Length: {0}\n", m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        //    content.text += string.Format("ShortNameHash: {0}\n", m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
        //    content.text += string.Format("Next State: {0}\n", m_Animator.GetNextAnimatorStateInfo(0).shortNameHash);
        //    content.text += string.Format("In Transition?: {0}\n", m_Animator.IsInTransition(0));
        //    content.text += string.Format("Transition Norm Time: {0}\n", m_Animator.GetAnimatorTransitionInfo(0).normalizedTime);

        //    GUILayout.Label(content);
        //}
    }

}


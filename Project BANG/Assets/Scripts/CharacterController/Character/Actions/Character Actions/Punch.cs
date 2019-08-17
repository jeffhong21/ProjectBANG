namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Punch : CharacterAction
    {
        [SerializeField]
        protected Collider[] m_Hitboxes = new Collider[0];



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


        }


		public override bool Move()
		{



            return true;
		}


		public override bool UpdateAnimator()
		{



            return base.UpdateAnimator();
		}


		public override bool CanStopAction()
        {



            return false;
        }


        protected override void ActionStopped()
        {
            //Debug.Log("Shooting action done");
        }


        //public override string GetDestinationState(int layer)
        //{

        //    return "";
        //}






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


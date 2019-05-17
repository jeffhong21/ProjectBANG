namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Slide : CharacterAction
    {




        //
        // Methods
        //

        public override bool CanStartAction()
        {
            if (base.CanStartAction())
            {
                if(m_Controller.Moving)
                    return true;
            }
            return false;
        }

        protected override void ActionStarted()
        {

        }


        public override bool CanStopAction()
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(m_StateName))
            {
                if (m_Animator.IsInTransition(0))
                {
                    Debug.LogFormat("{0} is exiting.", m_StateName);
                    return true;
                }

            }
            return false;
        }


        protected override void ActionStopped()
        {

        }





        //  Returns the state the given layer should be on.
        public override string GetDestinationState(int layer)
        {
            if (layer == 0)
            {
                return m_StateName;
            }
            return "";
        }

    }

}


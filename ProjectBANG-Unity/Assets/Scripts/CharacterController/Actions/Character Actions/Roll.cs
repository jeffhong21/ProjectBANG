namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Roll : CharacterAction
    {
        //public const int ACTION_ID = 15;
        protected readonly float m_MaxRollDistance = 4f;
        protected readonly float m_CheckHeight = 0.35f;

        protected string m_DestinationState;
        //
        // Methods
        //

        public override bool CanStartAction()
        {
            if (base.CanStartAction())
            {
                return true;
            }
            return false;
        }


        protected override void ActionStarted()
        {
            m_DestinationState = "Roll";
        }


        public override bool CanStopAction()
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(m_DestinationState)){
                if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f - m_TransitionDuration){
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
            if (layer == 0){
                return m_DestinationState;
            }
            return "";
        }

    }

}


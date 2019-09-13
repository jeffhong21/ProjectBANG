namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Crouch : CharacterAction
    {
        public override int ActionID
        {
            get { return m_ActionID = ActionTypeID.Crouch; }
            set { m_ActionID = value; }
        }


        protected float m_Height = 0.4f;
        protected float m_DefaultHeight = 1f;

        //
        // Methods
        //


        protected override void ActionStarted()
        {
            m_animator.SetFloat(HashID.Height, m_Height);
            m_animatorMonitor.SetMovementSetID(2);
        }



        protected override void ActionStopped()
        {
            m_animator.SetFloat(HashID.Height, m_DefaultHeight);
            m_animatorMonitor.SetMovementSetID(0);
        }




        //////  Returns the state the given layer should be on.
        //public override string GetDestinationState( int layer )
        //{
        //    if (layer == 0) {
        //        return m_StateName;
        //    }

        //    return "";
        //}

        public override bool IsConcurrentAction()
        {
            return true;
        }





    }

}


namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Crouch : CharacterAction
    {
        protected float m_Height = 0.5f;
        protected float m_DefaultHeight = 1f;

        //
        // Methods
        //


        protected override void ActionStarted()
        {
            m_Animator.SetFloat(HashID.Height, m_Height);
        }



        protected override void ActionStopped()
        {
            m_Animator.SetFloat(HashID.Height, m_DefaultHeight);
        }




        ////  Returns the state the given layer should be on.
        //public override string GetDestinationState(int layer)
        //{
        //    return "Crouch.Crouch";
        //    //return "Crouch.Crouch";
        //}

        public override bool IsConcurrentAction()
        {
            return true;
        }





    }

}


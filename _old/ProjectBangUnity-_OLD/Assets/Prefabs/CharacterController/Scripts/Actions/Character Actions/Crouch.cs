namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Crouch : CharacterAction
    {

        [Header("-- Crouch Settings --")]
        [SerializeField]
        protected bool m_IsCrouching;
        [SerializeField]
        protected bool m_IsConcurrent = true;
        [SerializeField]
        protected float m_Height = 0.5f;

        protected float m_DefaultHeight = 1f;



        //
        // Methods
        //
        protected virtual void Reset()
        {
            m_ActionID = 3;
            m_InputName = "Crouch";
            m_StartType = ActionStartType.ButtonDown;
            m_StopType = ActionStopType.ButtonToggle;
        }



        protected override void ActionStarted()
        {
            //m_IsCrouching = true;
            m_AnimatorMonitor.SetHeightValue(m_Height);
        }



        protected override void ActionStopped()
        {
            //m_IsCrouching = false;
            m_AnimatorMonitor.SetHeightValue(m_DefaultHeight);
        }




        //public override bool UpdateAnimator()
        //{
        //    m_Animator.SetBool("Crouching", m_IsCrouching);

        //    m_AnimatorMonitor.SetHeightValue(m_Height);

        //    return true;
        //}


        //  Returns the state the given layer should be on.
        public override string GetDestinationState(int layer)
        {
            return "Crouch";
            //return "Crouch.Crouch";
        }

        public override bool IsConcurrentAction()
        {
            return m_IsConcurrent;
        }





    }

}


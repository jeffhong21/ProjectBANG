namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Jump : CharacterAction
    {


        protected Rigidbody m_RigidBody;



        //
        // Methods
        //
        protected virtual void Reset()
        {
            m_ActionID = 1;
            m_InputName = "Jump";
            m_StartType = ActionStartType.ButtonDown;
            m_StopType = ActionStopType.Automatic;
        }


        //public override bool CanStopAction()
        //{
        //    return CheckGround();
        //}

        protected override void ActionStarted()
        {

        }


        //  Returns the state the given layer should be on.
        public override string GetDestinationState(int layer)
        {
            return "Jump.Jump";
        }



        protected override void ActionStopped()
        {
            //Debug.LogFormat("{0} Action has stopped", GetType().Name);
        }
    }

}


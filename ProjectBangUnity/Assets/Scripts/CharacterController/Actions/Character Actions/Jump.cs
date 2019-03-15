namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Jump : CharacterAction
    {




        //
        // Methods
        //


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


        //public override bool CanStopAction()
        //{
        //    return CheckGround();
        //}
    }

}


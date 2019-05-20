namespace CharacterController
{
    using UnityEngine;
    using System;


    public class StartMovement : CharacterAction
    {


        private float startAngle;

		//
		// Methods
		//
        public override bool CanStartAction()
        {
            if(m_Controller.Moving == false){
                if(m_Controller.InputVector.sqrMagnitude > 0.2f){
                    return true;
                } 
            }
            return false;
		}

		protected override void ActionStarted()
        {
            //  Start walk angle
            Vector3 axisSign = Vector3.Cross(m_Controller.LookDirection, m_Transform.forward);
            startAngle = Vector3.Angle(m_Transform.forward, m_Controller.LookDirection) * (axisSign.y >= 0 ? -1f : 1f);
            startAngle = (float)Math.Round(startAngle, 2);
            startAngle = Mathf.Approximately(startAngle, 0) ? 0 : (float)Math.Round(startAngle, 2);

            //Debug.LogFormat("Starting to move.  Start angle is {0}", startAngle);
        }




		public override bool UpdateMovement()
		{

		    return true;
		}


		public override bool UpdateAnimator()
		{
            //m_Animator.SetInteger(HashID.ForwardInput, 1);
            return false;
		}


		public override bool CanStopAction()
        {

            return true;
        }


        protected override void ActionStopped(){

        }


        //  Returns the state the given layer should be on.
        public override string GetDestinationState(int layer){
            return "";
        }



		protected void OnDrawGizmos(){
            if(m_IsActive){

            }
		}

	}

}


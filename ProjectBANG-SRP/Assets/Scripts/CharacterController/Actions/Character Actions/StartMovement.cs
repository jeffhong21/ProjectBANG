namespace CharacterController
{
    using UnityEngine;
    using System;


    public class StartMovement : CharacterAction
    {


        protected float m_InputAngle;
        protected float m_StartAngle;

        private float m_StartAngleSmooth;


        float startTime;
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
            var inputMag = m_Controller.InputVector.magnitude;
            //  Start walk angle
            Vector3 axisSign = Vector3.Cross(m_Controller.LookDirection, m_Transform.forward);
            m_InputAngle = Vector3.Angle(m_Transform.forward, m_Controller.LookDirection) * (axisSign.y >= 0 ? -1f : 1f);
            m_InputAngle = (float)Math.Round(m_InputAngle, 2);
            //m_StartAngle = m_InputAngle;
            if (inputMag <= 0.2f)
                m_StartAngle = m_InputAngle;
            else if (inputMag > 0.2f && Mathf.Abs(m_InputAngle) < 10)
                m_StartAngle = Mathf.SmoothDamp(m_StartAngle, 0, ref m_StartAngleSmooth, 0.25f);
            m_StartAngle = Mathf.Approximately(m_StartAngle, 0) ? 0 : (float)Math.Round(m_StartAngle, 2);

            Debug.LogFormat("Starting to move.  Start angle is {0}", m_StartAngle);
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


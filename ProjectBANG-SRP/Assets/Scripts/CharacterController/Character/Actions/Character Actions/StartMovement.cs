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
                if(m_Controller.InputVector.sqrMagnitude > 0f){
                    return true;
                } 
            }
            return false;
		}

		protected override void ActionStarted()
        {
            ////  Start walk angle
            //Vector3 axisSign = Vector3.Cross(m_Controller.LookDirection, m_Transform.forward);
            //startAngle = Vector3.Angle(m_Transform.forward, m_Controller.LookDirection) * (axisSign.y >= 0 ? -1f : 1f);
            //startAngle = (float)Math.Round(startAngle, 2);
            //startAngle = Mathf.Approximately(startAngle, 0) ? 0 : (float)Math.Round(startAngle, 2);

            ////Debug.LogFormat("Starting to move.  Start angle is {0}", startAngle);
            //m_Animator.SetFloat(HashID.StartAngle, startAngle);
        }




        //public override bool UpdateMovement()
        //{

        //    return true;
        //}

        float angleInDegrees;
        Vector3 rotationAxis;
        public override bool Move()
		{

            m_Animator.deltaRotation.ToAngleAxis(out angleInDegrees, out rotationAxis);
            Vector3 angularDisplacement = rotationAxis * angleInDegrees * Mathf.Deg2Rad;// * m_Controller.RotationSpeed;
            m_Rigidbody.angularVelocity = angularDisplacement;

            Vector3 velocity = (m_Animator.deltaPosition / m_DeltaTime);
            velocity.y = m_Controller.Grounded ? 0 : m_Rigidbody.velocity.y;
            m_Rigidbody.velocity = velocity;

            return false;
		}


		public override bool CanStopAction()
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(m_StateName))
            {
                if (m_Animator.GetNextAnimatorStateInfo(0).shortNameHash != 0 && m_Animator.IsInTransition(0))
                {
                    Debug.LogFormat("{0} has stopped because it is entering Exit State", m_StateName);
                    return true;
                }

                if (m_Animator.IsInTransition(0))
                {
                    return true;
                }
            }
            if (Time.time > m_ActionStartTime + 0.5f)
                return true;
            return false;
        }


        protected override void ActionStopped(){
            m_Animator.CrossFade("LocomotionFwd", 0.2f, 0);
        }


        //  Returns the state the given layer should be on.
        public override string GetDestinationState(int layer){
            if (layer == 0)
                return m_StateName;
            return "";
        }



		protected void OnDrawGizmos(){
            if(m_IsActive){

            }
		}

	}

}


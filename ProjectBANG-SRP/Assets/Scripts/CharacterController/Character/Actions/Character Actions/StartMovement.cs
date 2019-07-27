namespace CharacterController
{
    using UnityEngine;
    using System;


    public class StartMovement : CharacterAction
    {
        protected int maxInputCount = 4;


        protected int detectionCount;

        private float startAngle;

        //protected bool currentFrameMoving;
        //protected bool previousFrameMoving;

        protected float lastMoveAmount;
        protected float currentMoveAmount;
        protected bool isMoving;
        protected float moveAmount;
		//
		// Methods
		//
        public override bool CanStartAction()
        {
            lastMoveAmount = currentMoveAmount;
            currentMoveAmount = Mathf.Clamp01(Mathf.Abs(m_Controller.InputVector.x) + Mathf.Abs(m_Controller.InputVector.z));

            if (lastMoveAmount < currentMoveAmount) 
                isMoving = true;
            

            if (isMoving){
                if (detectionCount >= maxInputCount) {
                    return true;
                }
                detectionCount++;
            }

            if(currentMoveAmount >= 1) {
                detectionCount = 0;
                isMoving = false;
            }
            

            //CharacterDebug.Log("--- StartMovement --- ", isMoving);
            //CharacterDebug.Log("detectionCount", detectionCount);
            //CharacterDebug.Log("lastMoveAmount", lastMoveAmount);
            //CharacterDebug.Log("currentMoveAmount", currentMoveAmount);

            return false;
		}

		protected override void ActionStarted()
        {
            Vector3 lookDirection = m_Controller.LookRotation * m_Transform.forward;
            ////  Start walk angle
            Vector3 axisSign = Vector3.Cross(lookDirection, m_Transform.forward);
            startAngle = Vector3.Angle(m_Transform.forward, lookDirection) * (axisSign.y >= 0 ? -1f : 1f);
            startAngle = (float)Math.Round(startAngle, 2);
            startAngle = Mathf.Approximately(startAngle, 0) ? 0 : (float)Math.Round(startAngle, 2);

            Debug.LogFormat("Starting to move.  Start angle is {0}", startAngle);
            m_Animator.SetFloat(HashID.ActionFloatData, startAngle);
        }



        //float angleInDegrees;
        //Vector3 rotationAxis;
        //public override bool UpdateMovement()
        //{

        //    m_Animator.deltaRotation.ToAngleAxis(out angleInDegrees, out rotationAxis);
        //    Vector3 angularDisplacement = rotationAxis * angleInDegrees * Mathf.Deg2Rad;// * m_Controller.RotationSpeed;
        //    m_Rigidbody.angularVelocity = angularDisplacement;

        //    Vector3 velocity = (m_Animator.deltaPosition / m_DeltaTime);
        //    velocity.y = m_Controller.Grounded ? 0 : m_Rigidbody.velocity.y;
        //    m_Rigidbody.velocity = velocity;
        //    return true;
        //}





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
            //m_Animator.CrossFade("LocomotionFwd", 0.2f, 0);
            detectionCount = 0;
            isMoving = false;
            startAngle = 0;
        }


        //  Returns the state the given layer should be on.
        public override string GetDestinationState( int layer )
        {
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


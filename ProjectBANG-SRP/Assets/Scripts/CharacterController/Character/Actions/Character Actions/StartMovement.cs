namespace CharacterController
{
    using UnityEngine;
    using System;


    public class StartMovement : CharacterAction
    {
        public override int ActionID {
            get { return m_ActionID = ActionTypeID.StartMovement; }
            set { m_ActionID = value; }
        }


        [SerializeField] protected int maxInputCount = 4;


        protected int detectionCount;
        protected float startAngle;
        protected float lastMoveAmount;
        protected float currentMoveAmount;
        protected bool isStartingToMove;
        protected float moveAmount;
		//
		// Methods
		//
        public override bool CanStartAction()
        {
            lastMoveAmount = currentMoveAmount;
            currentMoveAmount = Mathf.Clamp01(Mathf.Abs(m_Controller.InputVector.x) + Mathf.Abs(m_Controller.InputVector.z));

            if (lastMoveAmount < currentMoveAmount) 
                isStartingToMove = true;
            

            if (isStartingToMove){
                if (detectionCount >= maxInputCount) {
                    detectionCount = 0;
                    isStartingToMove = false;
                    return true;
                }
                detectionCount++;
            }

            if(currentMoveAmount >= 1) {
                detectionCount = 0;
                isStartingToMove = false;
            }


            //CharacterDebug.Log("detectionCount", detectionCount);
            //CharacterDebug.Log("lastMoveAmount", lastMoveAmount);
            //CharacterDebug.Log("currentMoveAmount", currentMoveAmount);

            return false;
		}

		protected override void ActionStarted()
        {
            Vector3 lookDirection = m_Controller.LookRotation * m_Transform.forward;
            Vector3 moveDirection = m_Transform.InverseTransformDirection(m_Controller.InputVector);
            ////  Start walk angle
            Vector3 axisSign = Vector3.Cross(moveDirection, m_Transform.forward);
            startAngle = Vector3.Angle(m_Transform.forward, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);
            startAngle = (float)Math.Round(startAngle, 2);
            startAngle = Mathf.Approximately(startAngle, 0) ? 0 : (float)Math.Round(startAngle, 2);



            if (m_StateName.Length == 0) m_Animator.SetInteger(HashID.ActionID, m_ActionID);
            m_Animator.SetFloat(HashID.ActionFloatData, startAngle);


            //float angle = Mathf.Abs(startAngle);
            //int actionIntData = 0;
            //if (angle > 0) {
            //    if (angle > 0 && angle <= 45) actionIntData = 2;
            //    else if (angle > 45 && angle <= 90) actionIntData = 2;
            //    else if (angle > 90 && angle <= 135) actionIntData = 3;
            //    else if (angle > 135 && angle <= 180) actionIntData = 4;
            //    else actionIntData = 0;
            //}
            //actionIntData *= -(int)Mathf.Sign(startAngle);
            //m_Animator.SetInteger(HashID.ActionIntData, actionIntData);
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

        public override bool UpdateRotation()
        {
            if (m_ApplyBuiltinRootMotion) {
                m_Controller.RootMotionRotation.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);

                //  Update angular velocity.
                m_Rigidbody.angularVelocity = Vector3.Lerp(m_Rigidbody.angularVelocity, rotationAxis.normalized * angleInDegrees, m_DeltaTime * m_Controller.RotationSpeed);

                //  Update the rotations.
                var targetRotation = Quaternion.Slerp(m_Transform.rotation, Quaternion.AngleAxis(angleInDegrees, rotationAxis.normalized), m_DeltaTime * m_Controller.RotationSpeed);
                m_Rigidbody.MoveRotation(targetRotation * m_Transform.rotation);
                return false;
            }

            return true;




            
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
            if (Time.time > m_ActionStartTime +0.75f)
                return true;
            return false;
        }


        protected override void ActionStopped(){
            //m_Animator.CrossFade("LocomotionFwd", 0.2f, 0);
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


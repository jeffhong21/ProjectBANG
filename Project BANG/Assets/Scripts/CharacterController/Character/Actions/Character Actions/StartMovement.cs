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


            if (Math.Abs(lastMoveAmount) < float.Epsilon && currentMoveAmount > 0)
                isStartingToMove = true;

            //if (lastMoveAmount < currentMoveAmount) 
            //    isStartingToMove = true;
            

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

            ////  Start walk angle
            switch (m_Controller.Movement) {
                case (RigidbodyCharacterController.MovementTypes.Adventure):
                    Vector3 moveDirection = m_Transform.InverseTransformDirection(m_Controller.InputVector);
                    Vector3 axisSign = Vector3.Cross(moveDirection, m_Transform.forward);
                    startAngle = Vector3.Angle(m_Transform.forward, moveDirection) * (axisSign.y >= 0 ? 1f : -1f);
                    break;
                case (RigidbodyCharacterController.MovementTypes.Combat):
                    //moveDirection = m_Transform.rotation * m_Controller.InputVector;
                    //startAngle = Vector3.Angle(m_Transform.forward, moveDirection) * m_Controller.InputVector.x;
                    startAngle = Mathf.Atan2(m_Controller.InputVector.x, m_Controller.InputVector.z) * Mathf.Rad2Deg;
                    break;
            }

            startAngle = (float)Math.Round(startAngle, 2);
            startAngle = Mathf.Approximately(startAngle, 0) ? 0 : (float)Math.Round(startAngle, 2);


            if (m_StateName.Length == 0) m_Animator.SetInteger(HashID.ActionID, m_ActionID);
            m_Animator.SetFloat(HashID.ActionFloatData, startAngle);

            actionStarted = true;
        }



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


        bool actionStarted;
        public override bool CanStopAction()
        {
            //if (m_Animator.GetAnimatorTransitionInfo(0).anyState) {
            //    var state = m_AnimatorMonitor.GetStateName(m_Animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
            //    var clip = m_Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            //    Debug.LogFormat("Current State is <color=blue>{0}</color>.  Current clip is <color=blue>{1}</color>", state, clip);
            //}

            //if (actionStarted) {
            //    var state = m_AnimatorMonitor.GetStateName(m_Animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
            //    var clip = m_Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            //    Debug.LogFormat("Current State is <color=red>{0}</color>.  Current clip is <color=red>{1}</color>", state, clip);
            //    actionStarted = false;
            //}

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
            if (Time.time > m_ActionStartTime + 1)
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


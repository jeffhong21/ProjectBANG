namespace CharacterController
{
    using UnityEngine;
    using System;


    public class StopMovement : CharacterAction
    {
        public override int ActionID {
            get { return m_ActionID = ActionTypeID.StopMovement; }
            set { m_ActionID = value; }
        }

        [SerializeField] protected int maxInputCount = 4;
        [SerializeField, Range(0, 1)]
        protected float stopThreshold = 0.8f;
        
        protected float pivotWeightThreshold = 0.18f;

        protected int detectionCount;
        protected float lastMoveAmount;
        protected float currentMoveAmount;
        protected bool isStopMoving;
        protected float moveAmount;
        //  The direction the character is moving.
        protected float moveDirection;

        protected int pivotFoot;


        //
        // Methods
        //
        public override bool CanStartAction()
        {
            lastMoveAmount = currentMoveAmount;
            currentMoveAmount = Mathf.Clamp01(Mathf.Abs(m_Controller.InputVector.x) + Mathf.Abs(m_Controller.InputVector.z));

            if (lastMoveAmount > currentMoveAmount)
                isStopMoving = true;


            if (isStopMoving) {
                if (detectionCount >= maxInputCount && currentMoveAmount < stopThreshold) {
                    detectionCount = 0;
                    isStopMoving = false;

                    return true;
                }
                detectionCount++;
            }

            if (currentMoveAmount <= 0) {
                detectionCount = 0;
                isStopMoving = false;
            }


            CharacterDebug.Log("--- IsStopMoving", m_Animator.pivotWeight);
            return false;
        }


        public override void UpdateAction()
        {
            if (IsActive && pivotFoot == 0)
            {
                if (m_Animator.pivotWeight >= 1 - pivotWeightThreshold) {
                    pivotFoot = 1;
                } else if (m_Animator.pivotWeight <= 0 + pivotWeightThreshold) {
                    pivotFoot = 0;
                } else
                    pivotFoot = -1;

                if (m_IsActive && Time.time > m_ActionStartTime + 1.5f) {
                    Debug.LogWarningFormat("{0} could not find a planted pivot foot.  Ending Action.", GetType().Name);
                    CanStopAction();
                }
            }


        }


        protected override void ActionStarted()
        {
            if (pivotFoot == -1) return;

            //  Set ActionID parameter.
            if (m_StateName.Length == 0)
                m_Animator.SetInteger(HashID.ActionID, m_ActionID);



            //  Get the stop move direction.
            var axisSign = Vector3.Cross(m_Controller.MoveDirection, m_Transform.forward);
            moveDirection = Vector3.Angle(m_Transform.forward, m_Controller.MoveDirection) * (axisSign.y >= 0 ? -1f : 1f);

            if (moveDirection > 360 || moveDirection < -360)
                Debug.LogError(string.Format("{0} moveDirection value of {1} is beyond 360", GetType().Name, moveDirection ));

            m_Animator.SetFloat(HashID.ActionFloatData, moveDirection);



            //  Determine if we should play walk or run stop and which leg.  0 = left, 1 = right.
            int actionIntData = 0;
            if (m_Controller.Speed >= 1)
                actionIntData = 2;
            actionIntData += pivotFoot;

            m_Animator.SetInteger(HashID.ActionIntData, actionIntData);


            //float runCycle = Mathf.Repeat(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1);
            //Debug.Log("* Pivot weight: <b>" + m_Animator.pivotWeight + "</b> | RunCycle: " + runCycle + " | MoveDirection: " + moveDirection);

            //Debug.Log("* Pivot Foot: " + pivotFoot + " | RunCycle: " + runCycle + " | MoveDirection: " + moveDirection);
            //reset pivot foot index.

            pivotFoot = 0;
        }



        public override bool CanStopAction()
        {
            //if (m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(m_DestinationStateName))
            //{
            //    if (m_Animator.GetNextAnimatorStateInfo(0).shortNameHash != 0 && m_Animator.IsInTransition(0))
            //    {
            //        Debug.LogFormat("{0} has stopped because it is entering Exit State", m_StateName);
            //        return true;
            //    }

            //    if (m_Animator.IsInTransition(0))
            //    {
            //        return true;
            //    }
            //}

            //if (Mathf.Abs(m_Rigidbody.velocity.x) + Mathf.Abs(m_Rigidbody.velocity.z) <= 0.05f) return true;

            //if (m_Animator.pivotWeight == 0.5f) return true;

            if (Time.time > m_ActionStartTime + 1.5f)
                return true;
            return false;
        }


        protected override void ActionStopped()
        {
            pivotFoot = 0;
        }





    }

}


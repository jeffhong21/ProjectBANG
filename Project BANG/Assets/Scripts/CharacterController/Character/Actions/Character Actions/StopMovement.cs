namespace CharacterController
{
    using UnityEngine;
    using System;


    public class StopMovement : CharacterAction
    {
        public override int ActionID { get { return m_ActionID = ActionTypeID.StopMovement; } set { m_ActionID = value; } }

        [SerializeField] protected int maxInputCount = 4;
        [SerializeField, Range(0, 1)]
        protected float stopThreshold = 0.8f;
        //[SerializeField, Range(0.01f, 0.49f)]
        //protected float pivotWeightThreshold = 0.18f;

        protected int detectionCount;
        protected float lastMoveAmount;
        protected float currentMoveAmount;
        protected bool isStopMoving;
        protected float moveAmount;
        //  The direction the character is moving.
        protected float moveDirection;



        protected int successfulStarts;


        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if (!m_Controller.Grounded) return false;



            lastMoveAmount = currentMoveAmount;
            currentMoveAmount = Mathf.Clamp01(Mathf.Abs(m_Controller.InputVector.x) + Mathf.Abs(m_Controller.InputVector.z));

            if (lastMoveAmount > currentMoveAmount)
                isStopMoving = true;


            if (isStopMoving)
            {
                if (detectionCount >= maxInputCount && currentMoveAmount < stopThreshold)
                {
                    successfulStarts++;
                    //return true;
                }
                //if (detectionCount >= maxInputCount && currentMoveAmount < stopThreshold) {
                //    detectionCount = 0;
                //    isStopMoving = false;
                //    successfulStarts++;
                //    return true;
                //}
                detectionCount++;

                if (successfulStarts >= 3)
                {
                    successfulStarts = 0;
                    detectionCount = 0;
                    isStopMoving = false;
                    return true;
                }
            }


            if (currentMoveAmount <= 0 || lastMoveAmount <= currentMoveAmount) {
                detectionCount = 0;
                isStopMoving = false;
            }


            //CharacterDebug.Log("--- IsStopMoving", m_animator.pivotWeight);
            return false;
        }




        protected override void ActionStarted()
        {
            //  Set ActionID parameter.
            if (m_StateName.Length == 0)
                m_animator.SetInteger(HashID.ActionID, m_ActionID);

            //  Determine if we should play walk or run stop.
            int actionIntData = 0;
            if (m_Controller.Speed >= 1) actionIntData = 1;

            m_animator.SetInteger(HashID.ActionIntData, actionIntData);
        }







        public override bool CanStopAction()
        {
            //if (m_animator.pivotWeight == 0.5f) {
            //    Debug.LogFormat("Stopping {0} by pivotWeight");
            //    return true;
            //}
                

            int layerIndex = 0;
            if (m_animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == 0 && m_animator.IsInTransition(layerIndex)) {
                Debug.LogFormat("{1} is exiting. | {0} is the next state.", m_animatorMonitor.GetStateName(m_animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash), this.GetType());
                Debug.Log(Mathf.Abs(m_rigidbody.velocity.x) + Mathf.Abs(m_rigidbody.velocity.z));
                return true;
            }
            //if (m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(m_DestinationStateName))
            //{
            //    if (m_animator.GetNextAnimatorStateInfo(0).shortNameHash != 0 && m_animator.IsInTransition(0))
            //    {
            //        Debug.LogFormat("{0} has stopped because it is entering Exit State", m_StateName);
            //        return true;
            //    }

            //    if (m_animator.IsInTransition(0))
            //    {
            //        return true;
            //    }
            //}

            //if (Mathf.Abs(m_rigidbody.velocity.x) + Mathf.Abs(m_rigidbody.velocity.z) <= 0.05f) return true;

            //if (m_animator.pivotWeight == 0.5f) return true;

            if (Time.time > m_ActionStartTime + .5f)
                return true;
            return false;
        }






    }

}


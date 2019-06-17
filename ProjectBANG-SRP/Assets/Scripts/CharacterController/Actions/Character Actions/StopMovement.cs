namespace CharacterController
{
    using UnityEngine;
    using System;


    public class StopMovement : CharacterAction
    {

        protected const string walkBwdStopRightUp = "WalkBwdStop-RightUp";
        protected const string walkBwdStopLeftUp = "WalkBwdStop-LeftUp";
        protected const string runFwdtopRightUp = "RunBwdStop-RightUp";
        protected const string runFwdStopLeftUp = "RunBwdStop-LeftUp";

        [SerializeField]
        protected int m_MaxInputCount = 10;
        [SerializeField, Range(0, 1)]
        protected float m_StopThreshold = 0.2f;
        [SerializeField]
        protected int m_InputStartCount = 10;






        [SerializeField, DisplayOnly]
        protected int inputCount;




        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if(base.CanStartAction())
            {
                if (m_Controller.Moving && m_Controller.Grounded && m_Controller.InputVector.z < m_StopThreshold)
                {
                    //inputCount++;
                    //if(inputCount >= m_MaxInputCount || inputCount >= m_InputStartCount)
                    //{
                    //    Debug.Log(inputCount);
                    //    inputCount = 0;
                    //    return true;
                    //}
                    return true;
                }
            }

            return false;
        }

        protected override void ActionStarted()
        {

        }


        //  Returns the state the given layer should be on.
        public override string GetDestinationState(int layer)
        {

            if (layer == 0)
            {
                if (m_Animator.pivotWeight > 0.5f)
                {
                    if(m_Controller.InputVector.z > 0)
                        return runFwdStopLeftUp;
                    else if (m_Controller.InputVector.z < 0)
                        return walkBwdStopRightUp;
                }
                else if (m_Animator.pivotWeight < 0.5f)
                {
                    if (m_Controller.InputVector.z > 0)
                        return runFwdtopRightUp;
                    else if (m_Controller.InputVector.z < 0)
                        return walkBwdStopRightUp;
                }
                else
                {
                    return "";
                }

                return m_StateName;
            }
            return "";
        }


        public override bool CanStopAction()
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(m_DestinationStateName))
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


            return false;
        }


        protected override void ActionStopped()
        {
            m_Animator.CrossFade("LocomotionFwd", 0.2f, 0);
        }





    }

}


namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Roll : CharacterAction
    {


        //
        // Methods
        //
        protected override void ActionStarted()
        {
            //Debug.LogFormat("{0} Action has started {1}", GetType().Name, Time.time);
        }


        //  Returns the state the given layer should be on.
        public override string GetDestinationState(int layer)
        {
            if(layer == 0){
                if (m_Controller.InputVector.z > 0 && m_Controller.InputVector.x == 0)
                {
                    m_StateName = "Roll.Roll_Forward";
                }
                else if (m_Controller.InputVector.x < 0 && m_Controller.InputVector.z == 0)
                {
                    m_StateName = "Roll.Roll_Left";
                }
                else if (m_Controller.InputVector.x > 0 && m_Controller.InputVector.z == 0)
                {
                    m_StateName = "Roll.Roll_Right";
                }
                return m_StateName;
            }
            return "";
        }



        protected override void ActionStopped()
        {
            m_Animator.SetInteger(HashID.ActionID, 0);
            //Debug.LogFormat("{0} Action has stopped {1}", GetType().Name, Time.time);
        }




        public override bool CanStopAction()
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1){
                return false;
            }

            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(m_StateName)){
                if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 - m_TransitionDuration){
                    return true;
                }
                //Debug.LogFormat("Current state: {0} .  Normalized time.  {1} ",m_StateName, m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                return false;
            }


            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(m_StateName) == false){
                return true;
            }

            return false;
        
        }
    }

}


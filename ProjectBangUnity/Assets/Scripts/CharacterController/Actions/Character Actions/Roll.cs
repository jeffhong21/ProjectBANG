namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Roll : CharacterAction
    {
        protected Vector3 m_RollDirection;
        protected Vector3 m_StartDirection;

        public Vector3 RollDirection{
            get { return m_RollDirection; }
            set { m_RollDirection = value; }
        }

        //
        // Methods
        //
        protected override void ActionStarted()
        {
            m_StartDirection = m_Transform.forward;
            if(m_RollDirection != Vector3.zero)
                m_Controller.SetRotation(Quaternion.LookRotation(m_RollDirection, m_Transform.up));
        }



        protected override void ActionStopped()
        {
            m_Controller.SetRotation(Quaternion.LookRotation(m_StartDirection, m_Transform.up));
            m_RollDirection = Vector3.zero;
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


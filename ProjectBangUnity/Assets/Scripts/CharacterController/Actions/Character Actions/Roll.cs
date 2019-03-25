namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Roll : CharacterAction
    {
        protected const int ACTION_ID = 15;

        protected Vector3 m_RollDirection;
        protected Vector3 m_StartDirection;

        protected float m_ActionIntData;

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

            //m_ActionIntData = Vector3.Dot(m_Transform.forward, m_RollDirection);
            //if(m_ActionIntData < 1){
            //    m_ActionIntData = Vector3.Dot(m_Transform.right, m_RollDirection);
            //} else {
            //    m_ActionIntData = 0;
            //}
            //m_Animator.SetInteger(HashID.ActionIntData, (int)m_ActionIntData);
            //Debug.Log(m_ActionIntData);
            m_Animator.SetInteger(HashID.ActionIntData, 0);
        }



        protected override void ActionStopped()
        {
            m_Controller.SetRotation(Quaternion.LookRotation(m_StartDirection, m_Transform.up));
            m_RollDirection = Vector3.zero;

            //Debug.LogFormat("{0} Action has stopped {1}", GetType().Name, Time.time);

        }

        //public override string GetDestinationState(int layer)
        //{
        //    //if (m_ActionIntData == 0) return "Roll_Forward";
        //    //if (m_ActionIntData == 1) return "Roll_Right_Start";
        //    //if (m_ActionIntData == -1) return "Roll_Left_Start";
        //    m_StateName = "Roll_Forward";
        //    return "Roll_Forward";
        //}


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


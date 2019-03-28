namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Dodge : CharacterAction
    {
        public const int ACTION_ID = 16;


        protected int m_ActionIntData;

        private float m_ActionTime;
        private float m_ActionStopTime = 0.5f;

        //
        // Methods
        //
        protected override void ActionStarted()
        {
            m_Animator.SetInteger(HashID.ActionIntData, m_ActionIntData);
        }


        protected override void ActionStopped(){
            m_Animator.SetInteger(HashID.ActionIntData, 0);
        }


        public void SetDodgeDirection(int direction){
            m_ActionIntData = direction;
        }


        public override bool CanStopAction()
        {
            m_ActionTime += Time.deltaTime;
            if (m_ActionTime > m_ActionStopTime)
            {
                m_ActionTime = 0;
                return true;
            }
            return false;
        }
    }

}


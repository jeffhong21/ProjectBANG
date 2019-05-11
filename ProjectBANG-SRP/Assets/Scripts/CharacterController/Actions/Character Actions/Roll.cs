namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Roll : CharacterAction
    {
        //public const int ACTION_ID = 15;
        //protected readonly float m_MaxRollDistance = 4f;
        //protected readonly float m_CheckHeight = 0.35f;

        protected float m_RollRecurrenceDelay = 0.2f;
        protected float m_NextRollAllowed;
        protected float m_ColliderHeight;
        protected Vector3 m_ColliderCenter;
        //
        // Methods
        //

        public override bool CanStartAction()
        {
            if (base.CanStartAction() && Time.time > m_NextRollAllowed)
            {
                return true;
            }
            return false;
		}

		protected override void ActionStarted()
        {
            //  Cache variables
            m_ColliderHeight = m_CapsuleCollider.height;
            m_ColliderCenter = m_CapsuleCollider.center;
        }



		public override bool UpdateMovement()
		{
            m_CapsuleCollider.height = m_ColliderHeight * m_Animator.GetFloat(HashID.ColliderHeight);
            m_CapsuleCollider.center = (m_CapsuleCollider.height / 2) * Vector3.up;
            return base.UpdateMovement();
		}



		public override bool CanStopAction()
        {
            if (m_Animator.GetNextAnimatorStateInfo(0).shortNameHash == 0){
                if (m_Animator.IsInTransition(0)){
                    //Debug.LogFormat("{0} has stopped because it is entering Exit State", m_StateName);
                    return true;
                }
            }
            else if (m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(m_StateName))
            {
                if (m_Animator.IsInTransition(0)){
                    //Debug.LogFormat("{0} is exiting.", m_StateName);
                    return true;
                }
            }
            return false;
        }


        protected override void ActionStopped()
        {
            m_CapsuleCollider.height = m_ColliderHeight;
            m_CapsuleCollider.center = m_ColliderCenter;

            m_NextRollAllowed = Time.time + m_RollRecurrenceDelay;
        }





        //  Returns the state the given layer should be on.
        public override string GetDestinationState(int layer)
        {
            if (layer == 0){
                return m_StateName;
            }
            return "";
        }






    }

}


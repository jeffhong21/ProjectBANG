namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Roll : CharacterAction
    {
        protected float m_CheckHeight = 0.4f;
        //public const int ACTION_ID = 15;
        protected readonly float m_MaxDistance = 3f;
        //protected readonly float m_CheckHeight = 0.35f;

        protected float m_RollRecurrenceDelay = 0.2f;
        protected float m_NextRollAllowed;

        protected RaycastHit m_CheckDistanceHit;
        //
        // Methods
        //

        public override bool CanStartAction()
        {
            if (base.CanStartAction() && Time.time > m_NextRollAllowed)
            {
                if(Physics.Raycast(m_Transform.position + (Vector3.up * m_CheckHeight), m_Transform.forward, out m_CheckDistanceHit, m_MaxDistance, m_VaultLayers)){

                }
                return true;
            }
            return false;
		}

		protected override void ActionStarted()
        {
            //  Cache variables
            m_ColliderHeight = m_CapsuleCollider.height;
            m_ColliderCenter = m_CapsuleCollider.center;

            Vector3 velocity = Vector3.Scale(transform.forward, m_MaxDistance * new Vector3((Mathf.Log(1f / (m_DeltaTime * m_Rigidbody.drag + 1)) / -m_DeltaTime), 0, (Mathf.Log(1f / (m_DeltaTime * m_Rigidbody.drag + 1)) / -m_DeltaTime)));
            m_Rigidbody.velocity = velocity;
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


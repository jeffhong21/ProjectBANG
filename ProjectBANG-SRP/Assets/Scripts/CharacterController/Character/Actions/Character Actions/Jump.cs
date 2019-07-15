namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Jump : CharacterAction
    {
        [SerializeField]
        protected float m_Force = 5;
        [SerializeField]
        protected float m_RecurrenceDelay = 0.2f;

        private float m_NextJump;

		//
		// Methods
		//
        public override bool CanStartAction()
        {
            if (base.CanStartAction())
            {
                if(m_NextJump < Time.time)
                {
                    if (m_Controller.Grounded && m_Rigidbody.velocity.y > -0.001f)
                    {
                        return true;
                    }
                }

            }
            return false;
		}


		protected override void ActionStarted()
        {
            m_Animator.SetInteger(HashID.ActionID, (int)ActionTypeDefinition.Jump);

            float timeToJumpApex = 0.4f;
            var gravity = -(2 * m_Force) / Mathf.Pow(timeToJumpApex, 2);
            Vector3 verticalVelocity = Vector3.up * Mathf.Abs(gravity) * timeToJumpApex;
            //Vector3 verticalVelocity = Vector3.up * (Mathf.Sqrt(m_Force * -2 * Physics.gravity.y));


            //verticalVelocity = Vector3.up * (Mathf.Pow(m_Force, 2) / -2 * Physics.gravity.y);
            //m_Rigidbody.AddForce(verticalVelocity, ForceMode.VelocityChange);
            //m_Rigidbody.velocity += verticalVelocity;
            m_Rigidbody.AddForce(Vector3.up * m_Force, ForceMode.VelocityChange);
            Debug.Log(verticalVelocity);
        }


        public override bool CanStopAction()
        {
            return m_Rigidbody.velocity.y < 0;
        }


        protected override void ActionStopped()
        {
            m_NextJump = Time.time + m_RecurrenceDelay;

        }


		public override bool CheckGround()
		{
            if (m_Rigidbody.velocity.y > -0.01f || m_Rigidbody.velocity.y <= 0f)
            {
                m_Controller.Grounded = false;
            }

            return false;
		}




		public override bool UpdateAnimator()
		{
            return base.UpdateAnimator();
		}


		//  Returns the state the given layer should be on.
		public override string GetDestinationState(int layer)
        {
            if (layer == 0){
                if(m_Controller.Moving){
                    if (m_Animator.pivotWeight >= 0.5f)
                        m_DestinationStateName = "RunStart_LeftUp";
                    else if (m_Animator.pivotWeight < 0.5f)
                        m_DestinationStateName = "RunStart_RightUp";
                }
                else{
                    m_DestinationStateName = "IdleStart";
                }

                return m_DestinationStateName;
            }
            return "";
        }



    }

}


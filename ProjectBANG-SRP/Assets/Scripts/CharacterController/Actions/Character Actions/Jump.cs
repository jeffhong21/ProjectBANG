namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Jump : CharacterAction
    {
        [SerializeField]
        protected float m_Force = 12;
        [SerializeField]
        protected float m_Distance = 4;
        [SerializeField]
        protected float m_RecurrenceDelay = 0.1f;

        private Vector3 velocity;
        private Vector3 verticalVelocity;
        private Vector3 forwardVelocity;
        private float gravity;
        private float velocityY;
        private float startTime;





		//
		// Methods
		//
        public override bool CanStartAction()
        {
            if (base.CanStartAction())
            {
                return true;
            }
            return false;
		}

		protected override void ActionStarted()
        {
            m_Animator.SetInteger(HashID.ActionID, (int)ActionTypeDefinition.Jump);

            gravity = Physics.gravity.y;
            velocityY = Mathf.Sqrt(-2 * gravity * m_Force);
            forwardVelocity = m_Controller.Moving ? transform.forward : Vector3.zero;
            verticalVelocity = Vector3.zero;
            m_Rigidbody.AddForce(Vector3.up * m_Force + transform.forward * m_Distance, ForceMode.VelocityChange);
            //m_Rigidbody.velocity = forwardVelocity + verticalVelocity;
            startTime = Time.time;
        }


        public override bool CanStopAction()
        {
            return m_Controller.Grounded;
        }


        protected override void ActionStopped()
        {



        }


		public override bool UpdateMovement()
		{
            velocityY += gravity * m_DeltaTime;
            velocity = forwardVelocity * m_Distance + Vector3.up * velocityY;

            //Vector3 verticalVelocity = Vector3.Project(m_Rigidbody.velocity * m_Force, Physics.gravity);
            //Vector3 velocity = Vector3.Project(m_Transform.forward * m_Distance, Physics.gravity);

            velocity = (velocity + verticalVelocity) * m_DeltaTime;
            m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, velocity, (startTime - Time.time) / 1);  

            return false;
		}



		//  Returns the state the given layer should be on.
		public override string GetDestinationState(int layer)
        {
            if (layer == 0){
                return "JumpRunStart_LU";
            }
            return "";
        }



        private void OnDrawGizmos()
        {
            if (Application.isPlaying && m_IsActive)
            {

            }
        }



        protected override void DrawOnGUI()
        {

        }
    }

}


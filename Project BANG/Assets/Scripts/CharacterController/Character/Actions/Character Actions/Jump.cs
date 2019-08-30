namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Jump : CharacterAction
    {
        public override int ActionID {
            get { return m_ActionID = ActionTypeID.Jump; }
            set { m_ActionID = value; }
        }


        [SerializeField]
        protected float m_jumpHeight = 2;
        [SerializeField]
        protected float m_recurrenceDelay = 0.2f;

        private float m_nextJump;


        protected Vector3 m_verticalVelocity;
        protected Vector3 m_velocity;



        protected bool hasReachedApex;
        protected bool hasLanded;
        //
        // Methods
        //




        public override bool CanStartAction()
        {
            if (!base.CanStartAction()) return false;

            return m_nextJump < Time.time && m_Controller.Grounded && m_Rigidbody.velocity.y > -0.01f;

		}


		protected override void ActionStarted()
        {
            m_nextJump = Time.time + m_recurrenceDelay;
            //  Set ActionID parameter.
            if (m_StateName.Length == 0)
                m_Animator.SetInteger(HashID.ActionID, m_ActionID);


            float heightAdjusted = m_jumpHeight - m_Controller.ColliderHeight * m_Controller.ColliderRadius;
            m_verticalVelocity = Vector3.up * Mathf.Sqrt(2 * heightAdjusted * Mathf.Abs(m_Controller.Gravity.y) );
            m_Rigidbody.velocity += m_verticalVelocity;

            Debug.Log(heightAdjusted);
            //m_Rigidbody.AddForce(m_verticalVelocity, ForceMode.VelocityChange);
        }



        public override bool CanStopAction()
        {
            return m_Rigidbody.velocity.y <= 0;
        }




		public override bool CheckGround()
		{
            if (Time.time < m_ActionStartTime + 0.2f) {
                m_Controller.Grounded = false;
            }


            if (m_Rigidbody.velocity.y > 0) {
                RaycastHit hit = m_Controller.GetSphereCastGroundHit();

                float groundDistance = Vector3.Project(m_Transform.position - hit.point, m_Transform.up).magnitude;
                if (groundDistance < 0.1f && m_Rigidbody.velocity.y <= 0){
                    m_Controller.Grounded = true;
                }
                m_Controller.Grounded = false;

            }

            
            return false;
		}


        public override bool UpdateMovement()
        {
            var verticalVelocity = -(2 * m_jumpHeight) / (m_verticalVelocity.y * m_verticalVelocity.y);
            m_verticalVelocity = m_Controller.Gravity * m_DeltaTime + (Vector3.zero * verticalVelocity);

            //m_verticalVelocity = m_Controller.Gravity * m_DeltaTime;
            m_Controller.Velocity += m_verticalVelocity;
            //m_Rigidbody.velocity += m_verticalVelocity;

            return false;
        }









    }

}


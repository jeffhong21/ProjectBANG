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

        [SerializeField]
        protected bool m_startJump;
        [SerializeField]
        protected bool m_hasReachedApex;
        [SerializeField]
        protected bool m_hasLanded;
        //
        // Methods
        //




        public override bool CanStartAction()
        {
            if (!base.CanStartAction()) return false;

            return m_nextJump < Time.time && m_Controller.Grounded && m_rigidbody.velocity.y > -0.01f;

		}


		protected override void ActionStarted()
        {
            m_nextJump = Time.time + m_recurrenceDelay;
            //  Set ActionID parameter.
            if (m_StateName.Length == 0)
                m_animator.SetInteger(HashID.ActionID, m_ActionID);


            float heightAdjusted = m_jumpHeight - m_Controller.ColliderHeight * m_Controller.ColliderRadius;
            heightAdjusted = m_jumpHeight;
            m_verticalVelocity = Vector3.up * Mathf.Sqrt(2 * heightAdjusted * Mathf.Abs(m_Controller.Gravity.y) );
            //m_rigidbody.velocity += m_verticalVelocity;

            //m_rigidbody.ResetCenterOfMass();
            m_rigidbody.velocity += m_verticalVelocity;
            //m_rigidbody.AddRelativeForce(m_verticalVelocity, ForceMode.VelocityChange);
        }



        public override bool CanStopAction()
        {
            return m_hasLanded;
        }

        protected override void ActionStopped()
        {
            base.ActionStopped();

            m_Controller.Velocity = Vector3.zero;
            m_startJump = m_hasReachedApex = m_hasLanded = default;
            m_verticalVelocity = Vector3.zero;
        }


        public override bool CheckGround()
		{
            if (Time.time < m_ActionStartTime + 0.2f) {
                m_Controller.Grounded = false;
                return false;
            }


            //if (m_rigidbody.velocity.y > 0) {
            //    RaycastHit hit = m_Controller.GetSphereCastGroundHit();

            //    float groundDistance = Vector3.Project(m_transform.position - hit.point, m_transform.up).magnitude;
            //    if (groundDistance < 0.1f && m_rigidbody.velocity.y <= 0){
            //        m_Controller.Grounded = true;
            //    }
            //    m_Controller.Grounded = false;

            //}
            float radius = 0.1f;
            Vector3 origin = m_transform.position + Vector3.up * (0.1f);
            origin += Vector3.up * radius;

            if (Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit groundHit, 0.3f * 2, m_layers.SolidLayers))
            {
                m_Controller.Grounded = groundHit.distance < 0.3f;
            }
            else
            {
                m_Controller.Grounded = false;
            }





            return m_Controller.Grounded;
		}


        public override bool UpdateMovement()
        {
            if (!m_startJump)
            {
                m_startJump = m_rigidbody.velocity.y > 0;
            }

            if(m_startJump)
            {
                if (!m_hasReachedApex) m_hasReachedApex = m_rigidbody.velocity.y <= 0;
            }

            if(m_hasReachedApex)
            {
                m_hasLanded = m_Controller.Grounded;
            }

            //var verticalVelocity = -(2 * m_jumpHeight) / (m_verticalVelocity.y * m_verticalVelocity.y);
            //m_verticalVelocity = m_Controller.Gravity * m_deltaTime + (Vector3.zero * verticalVelocity);

            //m_verticalVelocity = m_Controller.Gravity * m_deltaTime;
            //m_Controller.Velocity += m_verticalVelocity;
            //m_rigidbody.velocity += m_verticalVelocity;

            return false;
        }



        public override bool UpdateAnimator()
        {
            if (m_hasLanded)
            {
                m_animatorMonitor.SetActionID(0);
            }
            return true;
        }






    }

}


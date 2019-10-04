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


        protected Vector3 m_verticalVelocity, m_velocity;
        protected Vector3 m_startPosition, m_verticalDirection;
        protected float m_verticalDistance;
        protected float m_elapsedTime;

        [SerializeField]
        protected bool m_startJump;
        [SerializeField]
        protected bool m_hasReachedApex;
        [SerializeField]
        protected bool m_hasLanded;


        private bool useGravityCache;


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

            useGravityCache = m_rigidbody.useGravity;
            m_rigidbody.useGravity = false;
            m_verticalVelocity = new Vector3(0, Mathf.Sqrt(-2 * m_jumpHeight * -9.8f), 0);


            m_rigidbody.AddForce(m_verticalVelocity * m_jumpHeight, ForceMode.VelocityChange);
            m_startPosition = m_transform.position;
            m_elapsedTime = 0;
            m_verticalDistance = 0;
            
        }



        public override bool CanStopAction()
        {
            return m_hasLanded;
        }

        protected override void ActionStopped()
        {
            base.ActionStopped();

            m_rigidbody.useGravity = useGravityCache;
            //m_Controller.Velocity = Vector3.zero;
            m_startJump = m_hasReachedApex = m_hasLanded = false;
            m_verticalVelocity = m_startPosition = Vector3.zero;
        }


        public override bool CheckGround()
		{
            //float velocityY = (verticalDistance / time) + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

            //if (Time.time < m_ActionStartTime + 0.2f) {
            //    m_Controller.Grounded = false;
            //    return false;
            //}


            //float radius = 0.1f;
            //Vector3 origin = m_transform.position.WithY(0.1f);
            //origin += Vector3.up * radius;

            //if (Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit groundHit, 0.3f * 2, m_layers.SolidLayers))
            //{
            //    m_Controller.Grounded = groundHit.distance < 0.3f;
            //}
            //else
            //{
            //    m_Controller.Grounded = false;
            //}





            return false;
		}


        public override bool UpdateMovement()
        {
            if (!m_startJump)
            {
                m_startJump = m_rigidbody.velocity.y > 0;

                if(m_Debug && m_startJump){
                    Debug.Log("<color=magenta>[m_startJump ]</color>");
                    Debug.Break();
                }

            }

            if(m_startJump)
            {
                m_hasReachedApex = m_rigidbody.velocity.y <= 0;

                if (m_Debug && m_hasReachedApex) {
                    Debug.Log("<color=magenta>[m_hasReachedApex]</color>");
                    Debug.Break();
                }
            }

            if(m_hasReachedApex)
            {
                m_hasLanded = m_Controller.Grounded || m_rigidbody.velocity.y > -0.2f;

                if (m_Debug && m_hasLanded) {
                    Debug.Log("<color=magenta>[ m_hasLanded]</color>");
                    Debug.Break();
                }
            }



            //if (m_rigidbody.velocity.y < 0) {
            //    m_rigidbody.useGravity = useGravityCache;
            //    m_Controller.MoveDirection += m_Controller.Gravity * m_deltaTime;
            //}

            


            //m_elapsedTime += m_deltaTime;
            //m_verticalDirection = m_transform.position - m_startPosition;
            ////m_verticalDirection = Vector3.Project(m_verticalDirection, Vector3.up);
            //m_verticalDistance = (m_verticalDirection.y / m_elapsedTime) + Mathf.Abs(Physics.gravity.y) * m_elapsedTime;
            //m_verticalDistance *= m_deltaTime;

            //Vector3 velocity = m_Controller.Velocity;
            //float percentage = 0;
            //if (m_verticalDistance < m_jumpHeight) {
            //    percentage = Mathf.Clamp01(m_verticalDistance / m_jumpHeight);
            //    percentage *= percentage * percentage;
            //    velocity.y = Mathf.Lerp(m_rigidbody.velocity.y, 0, percentage);
            //}
            //else {
            //    percentage = 1;
            //}


            ////var verticalVelocity = -(2 * m_jumpHeight) / (m_verticalVelocity.y * m_verticalVelocity.y);
            //////m_verticalVelocity = m_Controller.Gravity * m_deltaTime + (Vector3.zero * verticalVelocity);
            ////var velocity = m_Controller.Velocity;
            //velocity += m_Controller.Gravity * m_deltaTime;
            //m_Controller.Velocity = velocity;
            //////m_rigidbody.velocity += m_verticalVelocity;
            //Debug.LogFormat("time: {2} | percentage: {0} | distance: {1}", percentage, m_verticalDistance, m_elapsedTime);


            //if (m_verticalDistance >= m_jumpHeight) {
            //    if (reachedJumpHeight == false) {
            //        reachedJumpHeight = true;
            //        Debug.LogFormat("<b>percentage: {0} | distance: {1}\n vv: {2} | rb: {3}  | tie: {4}</b>",
            //            percentage, m_verticalDistance, m_verticalVelocity, m_rigidbody.velocity, m_elapsedTime);
            //    }
            //}
            //else {
            //    reachedJumpHeight = false;
            //}


            return false;
        }





    }

}


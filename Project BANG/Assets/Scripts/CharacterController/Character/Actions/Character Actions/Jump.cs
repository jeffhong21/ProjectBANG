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
        protected float m_Force = 5;
        [SerializeField]
        protected float m_RecurrenceDelay = 0.2f;
        [SerializeField]
        protected SurfaceEffect m_LandSurfaceImpact;


        private float m_NextJump;



        [SerializeField]
        protected float jumpHeight = 4;

        protected float acceleration;
        protected float speed;
        protected float verticalVelocity;
        protected Vector3 velocity;



        protected bool hasReachedApex;
        protected bool hasLanded;
        //
        // Methods
        //
        // Calculate the initial velocity of a jump based off gravity and desired maximum height attained
        protected float CalculateJumpSpeed(float jumpHeight, float gravity)
        {
            return Mathf.Sqrt(2 * jumpHeight * gravity);
        }



        public override bool CanStartAction()
        {
            if (base.CanStartAction())
            {
                if(m_NextJump < Time.time && m_Controller.Grounded && m_Rigidbody.velocity.y > -0.01f){
                    return true;
                }

            }
            return false;
		}


		protected override void ActionStarted()
        {
            //  Set ActionID parameter.
            if (m_StateName.Length == 0)
                m_Animator.SetInteger(HashID.ActionID, m_ActionID);

            hasReachedApex = hasLanded = false;



            //float timeToJumpApex = 0.4f;
            //var gravity = -(2 * m_Force) / Mathf.Pow(timeToJumpApex, 2);
            ////Vector3 verticalVelocity = Vector3.up * Mathf.Abs(gravity) * timeToJumpApex;
            //Vector3 verticalVelocity = Vector3.up * (Mathf.Sqrt(m_Force * -2 * Physics.gravity.y));


            ////verticalVelocity = Vector3.up * (Mathf.Pow(m_Force, 2) / -2 * Physics.gravity.y);
            ////m_Rigidbody.AddForce(verticalVelocity, ForceMode.VelocityChange);
            //m_Rigidbody.velocity += verticalVelocity;
            //m_Rigidbody.AddForce(verticalVelocity, ForceMode.VelocityChange);
            ////Debug.Log(verticalVelocity);
            ///

            //CalculateJumpVelocity();
            //m_Rigidbody.velocity = CalculateJumpVelocity() + m_Controller.MoveDirection;
            //m_Rigidbody.AddForce(CalculateJumpVelocity(), ForceMode.VelocityChange);

            float jumpForce = CalculateJumpSpeed(jumpHeight, -9.8f);
            Vector3 vv = Vector3.up * jumpForce + m_Controller.MoveDirection;
            m_Rigidbody.velocity = vv;
            m_Rigidbody.AddForce(vv, ForceMode.VelocityChange);
        }



        public override bool CanStopAction()
        {
            if (hasReachedApex && hasLanded)
                return true;

            if (Time.time > m_ActionStartTime + 1.1f)
                return true;
            return false;
        }


        protected override void ActionStopped()
        {
            hasReachedApex = hasLanded = false;
            m_NextJump = Time.time + m_RecurrenceDelay;
        }


		public override bool CheckGround()
		{
            if (Time.time < m_ActionStartTime + 0.2f) {
                m_Controller.Grounded = false;
            }


            if (m_Rigidbody.velocity.y > 0) {
                RaycastHit hit;

                if (Physics.Raycast(m_Transform.position, Vector3.down, out hit, 0.1f, m_Layers.SolidLayers))
                {
                    var groundDistance = Vector3.Project(m_Rigidbody.position - hit.point, transform.up).magnitude;

                    if (groundDistance < 0.1f && m_Rigidbody.velocity.y <= 0) {
                        m_Controller.Grounded = true;
                    }
                    m_Controller.Grounded = false;

                }
            }

            
            return false;
		}


        public override bool UpdateMovement()
        {
            //velocity.y -= verticalVelocity * m_DeltaTime;
            //m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, velocity, m_DeltaTime * jumpHeight);
            return false;
        }


        public override bool UpdateAnimator()
		{
            return base.UpdateAnimator();
		}





        protected Vector3 CalculateJumpVelocity()
        {
            //speed = Mathf.Sqrt(-2.0f * m_Controller.Gravity.y  * jumpHeight);
            speed = .8f;
            acceleration = -(2 * jumpHeight) / Mathf.Pow(speed, 2);
            verticalVelocity = Mathf.Abs(acceleration) * speed;

            velocity = m_Controller.Velocity;
            velocity.y += verticalVelocity;

            //Debug.Log(velocity);
            return velocity;
        }


        protected Vector3 CalculateVelocity( Vector3 target, Vector3 origin, float time )
        {
            //  Define the distance x and y first.
            Vector3 distance = target - origin;
            Vector3 distanceXZ = distance;
            distanceXZ.y = 0;


            //  Create a float that repsents our distance
            float verticalDistance = distance.y;              //  vertical distance
            float horizontalDistance = distanceXZ.magnitude;   //  horizontal distance


            //  Calculate the initial velocity.  This is distance / time.
            float velocityXZ = horizontalDistance / time;
            float velocityY = (verticalDistance / time) + 0.5f * Mathf.Abs(Physics.gravity.y) * time;



            Vector3 result = distanceXZ.normalized;
            result *= velocityXZ;
            result.y = velocityY;

            return result;
        }

    }

}


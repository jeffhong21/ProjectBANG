namespace CharacterController
{
    using UnityEngine;


    public class Fall : CharacterAction
    {
        public override int ActionID { get { return m_ActionID = ActionTypeID.Fall; } set { m_ActionID = value; } }


        [SerializeField]
        protected float m_minFallHeight = 1f;
        [SerializeField]
        protected SurfaceEffect m_landSurfaceImpact;
        [SerializeField]
        protected float m_minSurfaceImpactVelocity = 1f;


        protected bool m_isAirborne;
        protected Vector3 m_currentPosition;
        protected Vector3 m_startPosition;
        protected float m_fallHeight;



        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if(!m_Controller.Grounded && m_rigidbody.velocity.y < 0)
            {
                m_currentPosition = m_transform.position;
                if (!m_isAirborne)
                {
                    m_startPosition = m_currentPosition;
                    m_isAirborne = true;
                }

                m_fallHeight = m_startPosition.y - m_currentPosition.y;

                if(Mathf.Abs(m_fallHeight) > m_minFallHeight)
                {
                    return true;
                }
            }
            else
            {
                if(m_Controller.Grounded || m_rigidbody.velocity.y >= 0 && m_isAirborne)
                {
                    m_currentPosition = default;
                    m_startPosition = default;
                    m_fallHeight = 0;
                    m_isAirborne = false;
                }
            }



            return false;
		}


		protected override void ActionStarted()
        {
            m_animatorMonitor.SetActionID(ActionID);
        }


        public override bool CheckGround()
        {
            float radius = 0.1f;
            Vector3 origin = m_transform.position + Vector3.up * (0.1f);
            origin += Vector3.up * radius;

            if(Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit groundHit, 0.3f * 2, m_layers.SolidLayers))
            {
                m_Controller.Grounded = groundHit.distance < 0.3f;
            }
            else
            {
                m_Controller.Grounded = false;
            }


            if (m_Controller.Grounded && m_rigidbody.velocity.y > -1.01f)
            {
                m_animatorMonitor.SetActionID(0);
            }


            DebugUI.DebugUI.Log(this, "GroundDistance", groundHit.distance, DebugUI.RichTextColor.Red);

            return false;
        }




        public override bool CanStopAction()
        {
            if (m_Controller.Grounded && m_rigidbody.velocity.y > -0.01f)
            {
                return true;
            }

            return false;
        }


        public override bool UpdateAnimator()
        {
            m_fallHeight = m_startPosition.y - m_currentPosition.y;
            m_animatorMonitor.SetActionFloatData(Mathf.Abs(m_fallHeight));



            return true;
        }


        protected override void ActionStopped()
        {

            m_currentPosition = default;
            m_startPosition = default;
            m_fallHeight = 0;
            m_isAirborne = false;


            DebugUI.DebugUI.Remove(this, "GroundDistance");
        }








    }
}


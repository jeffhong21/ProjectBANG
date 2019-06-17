namespace CharacterController
{
    using UnityEngine;


    public class Fall : CharacterAction
    {
        protected enum LandingType { 
            Default = 1, 
            Hard = 2, 
            Roll = 3
        };

        protected LandingType m_LandingType = LandingType.Default;

        [SerializeField]
        protected float m_MinFallHeight = 1f;
        [SerializeField]
        protected GameObject m_LandSurfaceImpact;
        [SerializeField]
        protected float m_MinSurfaceImpactVelocity = 1f;



        protected RaycastHit m_RaycastHit;
        protected float m_StartHeight;

        
        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if(m_Controller.Grounded){
                m_StartHeight = m_Rigidbody.position.y;
            }
            if (m_Controller.Grounded == false  &&
                m_Rigidbody.velocity.y < 0 )
            {
                if(m_StartHeight - m_Rigidbody.position.y > m_MinFallHeight)
                {
                    if (m_Debug) Debug.LogFormat(" Rigidbody velocity: {0} | Fall Height: {1}", m_Rigidbody.velocity.y, m_StartHeight - m_Rigidbody.position.y);
                    return true;

                }
                //if (Mathf.Abs(m_Rigidbody.velocity.y) > m_MinFallHeight)
                //{
                //    if(m_Debug) Debug.LogFormat(" Rigidbody velocity: {0} | Fall Height: {1}", m_Rigidbody.velocity.y, m_StartHeight - m_Rigidbody.position.y);
                //    return true;
                //}

            }

            return false;
		}

		protected override void ActionStarted()
        {
            m_Animator.SetInteger(HashID.ActionID, (int)ActionTypeDefinition.Fall);
            m_ActionStartTime = Time.time;
        }


        public override bool CheckGround()
        {
            m_Controller.Grounded = false;
            if (Physics.Raycast(m_Rigidbody.position, Vector3.down, out m_RaycastHit, 0.5f, m_Layers.SolidLayers)){
                if(m_RaycastHit.transform != m_Transform)
                {
                    m_Controller.Grounded = true;

                    if (Time.time - m_ActionStartTime > 1)
                        m_LandingType = LandingType.Hard;
                    else if (Time.time - m_ActionStartTime > 1 && m_Controller.InputVector.magnitude > 0.2f)
                        m_LandingType = LandingType.Roll;
                    else
                        m_LandingType = LandingType.Default;



                    if(m_Debug) Debug.LogFormat("Falling has landed. Hit {0} | Total air time: {1}", m_RaycastHit.transform.name, Time.time - m_ActionStartTime);
                    //Debug.Break();
                }

            }

            return false;
        }





		public override bool CanStopAction()
        {
            if (m_Controller.Grounded)
                return true;
            return false;
        }


        protected override void ActionStopped()
        {
            m_Animator.SetInteger(HashID.ActionIntData, (int)m_LandingType);
            //if(m_Debug) Debug.LogFormat("Total air time: {0}", Time.time - m_ActionStartTime);
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


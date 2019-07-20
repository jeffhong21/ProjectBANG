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
        protected float m_MinFallHeight = 2f;
        [SerializeField]
        protected SurfaceEffect m_LandSurfaceImpact;
        [SerializeField]
        protected float m_MinSurfaceImpactVelocity = 1f;


        protected Vector3 m_LandPosition;
        protected RaycastHit m_RaycastHit;
        protected float m_StartHeight;
        protected float m_FallHeight;
        
        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if (m_Controller.Grounded == false  &&
                m_Rigidbody.velocity.y < -1f )
            {
                if (Physics.Raycast(m_Transform.position, Vector3.down, out m_RaycastHit, 50, m_Layers.SolidLayers))
                {
                    if (m_RaycastHit.distance > m_MinFallHeight)
                    {
                        Debug.LogFormat(" **  Rigidbody velocity: {0} | Fall Height: {1}", m_Rigidbody.velocity.y, m_RaycastHit.distance);
                        return true;
                    }
                }

            }

            return false;
		}

		protected override void ActionStarted()
        {
            m_Animator.SetInteger(HashID.ActionID, (int)ActionTypeDefinition.Fall);
            m_ActionStartTime = Time.time;

        }


        //public override bool CheckGround()
        //{
        //    float checkDistance = 1;
        //    if (Physics.Raycast(m_Transform.position, Vector3.down, out m_RaycastHit, checkDistance, m_Layers.SolidLayers))
        //    {
        //        m_Controller.Grounded = true;

        //        if (Time.time - m_ActionStartTime > 1)
        //            m_LandingType = LandingType.Hard;
        //        else if (Time.time - m_ActionStartTime > 1 && m_Controller.InputVector.sqrMagnitude > 0.2f)
        //            m_LandingType = LandingType.Roll;
        //        else
        //            m_LandingType = LandingType.Default;



        //        if (m_Debug) Debug.LogFormat("Falling has landed. Hit {0} | Total air time: {1}", m_RaycastHit.transform.name, Time.time - m_ActionStartTime);
        //        //Debug.Break();


        //    }

        //    return false;
        //}




        public override bool CanStopAction()
        {
            if (m_Controller.Grounded && m_Rigidbody.velocity.y > -0.01f){
                return true;
            }

            return false;
        }


        //protected override void ActionStopped()
        //{
        //    m_Animator.SetInteger(HashID.ActionIntData, (int)m_LandingType);
        //    Debug.LogFormat("Total air time: {0}", Time.time - m_ActionStartTime);
        //}


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

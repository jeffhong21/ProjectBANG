namespace CharacterController
{
    using UnityEngine;


    public class Fall : CharacterAction
    {
        protected enum LandingType { 
            Default, 
            Hard, 
            Roll
        };

        protected LandingType m_LandingType = LandingType.Default;

        [SerializeField]
        protected float m_MinFallHeight = 1f;
        [SerializeField]
        protected GameObject m_LandSurfaceImpact;
        [SerializeField]
        protected float m_MinSurfaceImpactVelocity = 1f;

        private float m_Airtime;
        private float m_FallVelocity;
        private bool m_PlaySurfaceLandImpact;

        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if (m_Controller.Grounded == false  &&
                m_Rigidbody.velocity.y < 0      &&
                Mathf.Abs(m_Controller.GroundDistance) > (m_MinFallHeight == 0 ? m_MinFallHeight + 0.2f : m_MinFallHeight))
            {
                return true;
            }

            return false;
		}

		protected override void ActionStarted()
        {
            m_Animator.SetInteger(HashID.ActionID, 2);



            m_Airtime = Time.time;
        }



		public override bool UpdateMovement()
		{
            m_Airtime += m_DeltaTime;

            return base.UpdateMovement();
		}



		public override bool CanStopAction()
        {
            if (Mathf.Abs(m_Rigidbody.velocity.y) > m_MinSurfaceImpactVelocity){
                m_FallVelocity = (float)System.Math.Round(m_Rigidbody.velocity.y, 2);
                m_PlaySurfaceLandImpact = true;
            }

            if (m_Controller.Grounded)
                return true;
            return false;
        }


        protected override void ActionStopped()
        {
            if (m_FallVelocity > 10)
                m_LandingType = LandingType.Hard;
            else if (m_FallVelocity > 20)
                m_LandingType = LandingType.Roll;
            else
                m_LandingType = LandingType.Default;
            m_Animator.SetInteger(HashID.ActionIntData, (int)m_LandingType);

            if(m_PlaySurfaceLandImpact){
                //Debug.LogFormat("Landing Velocity({0}) is greater than m_MinSurfaceImpactVelocity({1})", m_FallVelocity, m_MinSurfaceImpactVelocity);
            }


            m_PlaySurfaceLandImpact = false;
            m_FallVelocity = 0;
        }


        //  Returns the state the given layer should be on.
        public override string GetDestinationState(int layer)
        {
            if (layer == 0){
                //return "JumpingDown.JumpingDown";
                return "Falling";
            }
            return "";
        }





    }
}


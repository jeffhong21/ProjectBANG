namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Sprint : CharacterAction
    {
        [SerializeField]
        protected float m_SpeedChangeMultiplier = 2f;
        [SerializeField]
        protected float m_MinSpeedChange = -2f;
        [SerializeField]
        protected float m_MaxSpeedChange = 2f;
        [SerializeField]
        protected float m_StaminaDecreaseRate = 0.5f;
        [SerializeField]
        protected float m_StaminaIncreaseRate = 0.1f;
        [SerializeField]
        protected float m_MaxStanima = 100;

        [SerializeField, DisplayOnly]
        private float m_CurrentStanima = 100;
        [SerializeField, DisplayOnly]
        private Vector3 m_SpeedInput;


		//
		// Methods
		//
		protected override void Awake()
		{
            base.Awake();

            m_CurrentStanima = m_MaxStanima;
		}


		public override bool CanStartAction()
		{
            if(base.CanStartAction()){
                return m_CurrentStanima > (m_MaxStanima * 0.1f);
            }
            return false;
		}


		public override void UpdateAction()
		{
            if(m_CurrentStanima < m_MaxStanima){
                m_CurrentStanima = Mathf.Clamp(m_CurrentStanima + m_StaminaIncreaseRate, 0, 100);
            }
		}



        protected override void ActionStarted()
        {
            
        }


        protected override void ActionStopped()
        {


        }


        public override bool Move()
        {

            m_SpeedInput = m_Controller.InputVector;
            m_SpeedInput.z = Mathf.Clamp(m_Controller.InputVector.z * m_SpeedChangeMultiplier, m_MinSpeedChange, m_MaxSpeedChange);
            m_Controller.InputVector = m_SpeedInput;

            m_CurrentStanima = Mathf.Clamp(m_CurrentStanima - m_StaminaDecreaseRate, 0, 100);

            //m_Rigidbody.AddForce(m_Transform.forward, ForceMode.VelocityChange);

            return true;
        }


		public override bool IsConcurrentAction()
		{
            return true;
		}


	}

}


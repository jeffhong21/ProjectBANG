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
        private Vector3 m_SpeedInput;


		//
		// Methods
		//
		protected override void Awake()
		{
            base.Awake();

            m_CurrentStanima = m_MaxStanima;
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
            if (m_CurrentStanima > 0)
            {
                m_SpeedInput = m_Controller.InputVector;
                m_SpeedInput.z = Mathf.Clamp(m_Controller.InputVector.z * m_SpeedChangeMultiplier, m_MinSpeedChange, m_MaxSpeedChange);
                m_Controller.InputVector = m_SpeedInput;

                m_CurrentStanima = Mathf.Clamp(m_CurrentStanima - m_StaminaDecreaseRate, 0, 100);
            }

            return true;
        }


		public override bool IsConcurrentAction()
		{
            return true;
		}


	}

}


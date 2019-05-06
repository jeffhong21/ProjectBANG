namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Die : CharacterAction
    {
        private int m_DeathTypeIndex;
        [SerializeField]
        private bool m_IsDead;


        protected virtual void Start()
        {
            EventHandler.RegisterEvent<Vector3, Vector3, GameObject>(m_GameObject, "OnDeath", OnDeath);
        }

		//
		// Methods
		//


        public override bool CanStartAction()
        {
            if (m_IsDead)
                return true;
            return false;
		}

		public override bool CanStopAction()
        {
            if (m_IsDead == false)
                return true;
            return false;
        }


        protected override void ActionStarted()
        {
            m_IsDead = true;
            m_AnimatorMonitor.SetActionID(4);
            m_AnimatorMonitor.SetIntDataValue(m_DeathTypeIndex);

        }


        protected override void ActionStopped()
        {
            //Debug.LogFormat("Action Stopped.  {0} killed", m_GameObject.name);
        }


        public override string GetDestinationState(int layer)
        {
            if (layer == m_AnimatorMonitor.FullBodyLayerIndex)
                return "Die";

            return string.Empty;
        }



        private void OnDeath(Vector3 position, Vector3 force, GameObject attacker)
        {
            m_DeathTypeIndex = GetDeathTypeIndex(position, force, attacker);
            //m_DeathTypeIndex = Random.Range(0, 4);
            m_IsDead = true;
            //Debug.LogFormat("{0} killed", m_GameObject.name);
        }


        protected virtual int GetDeathTypeIndex(Vector3 hitLocation, Vector3 force, GameObject attacker)
        {
            int index = 0;
            float fwd = Vector3.Dot(m_Transform.forward + (m_Transform.up * hitLocation.y), hitLocation);
            //float right = Vector3.Dot(m_Transform.right + (m_Transform.up * hitLocation.y), hitLocation);
            if (fwd >= 0.45 || fwd <= -0.45)
            {
                if (fwd >= 0.45)
                    index = Random.Range(0, 2);
                else
                    index = Random.Range(2, 4);
            }

            return index;
        }









    }

}


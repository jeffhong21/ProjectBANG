namespace CharacterController
{
    using UnityEngine;


    public class TakeDamage : CharacterAction
    {
        private int m_DamageTypeIndex;
        private bool m_IsDamaged;

        [SerializeField]
        protected float m_MinDamageAmount;


        protected virtual void Start()
        {
            EventHandler.RegisterEvent<float, Vector3, Vector3, GameObject>(m_GameObject, EventIDs.OnTakeDamage, OnTakeDamage);
        }





        public override bool CanStartAction()
        {
            if (m_IsDamaged)
                return true;
            return false;
        }


        public override bool CanStopAction()
        {
            if (m_IsDamaged)
                return true;
            return false;
        }



		protected override void ActionStarted()
		{
            m_IsDamaged = true;
            m_AnimatorMonitor.SetActionID(m_IsDamaged ? 10 : 0);
            m_AnimatorMonitor.SetIntDataValue(0);
            //Debug.Log("Damage Type Index: " + m_DamageTypeIndex);
		}


		protected override void ActionStopped()
        {
            m_IsDamaged = false;
            m_AnimatorMonitor.SetActionID(0);
            m_AnimatorMonitor.SetIntDataValue(0);
        }



        public override bool IsConcurrentAction()
        {
            return true;
        }

        //  Turn IK off so the hands are not used when taking damage.
        public override bool CanUseIK(int layer)
        {
            return true;
        }


        //  Returns the state the given layer should be on.
        public override string GetDestinationState(int layer)
        {
            if (layer == m_AnimatorMonitor.AdditiveLayerIndex)
                return "Take Damage";
            
            return string.Empty;
        }


        private void OnTakeDamage(float amount, Vector3 position, Vector3 force, GameObject attacker)
        {
            //Debug.LogFormat("-- {0} recieved {1} of damage.", m_GameObject.name, amount);
            m_DamageTypeIndex = GetDamageTypeIndex(amount, position, force, attacker);
            m_IsDamaged = true;

            if(amount > m_MinDamageAmount){
            }
        }


        protected virtual int GetDamageTypeIndex(float amount, Vector3 position, Vector3 force, GameObject attacker)
        {
            int index = 0;

            var direction = m_Transform.forward;
            direction.y = position.y;
            float fwd = Vector3.Dot(direction, position);

            if (fwd >= 0.45 || fwd <= -0.45)
            {
                if (fwd >= 0.45)
                    index = 0;
                else
                    index = 1;
            }

            //Debug.LogFormat("{0} has take damage from {1}. | fwd: {2}) |", gameObject.name, attacker.name, fwd);
            return index;
        }


        protected virtual void Reset()
        {
            m_StartType = ActionStartType.Automatic;
            m_StopType = ActionStopType.Automatic;
        }
    }

}


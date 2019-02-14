namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections;

    public class Health : MonoBehaviour
    {
        //
        // Fields
        // 
        [SerializeField]
        protected bool m_Invincible;
        [SerializeField, Bang.ReadOnly]
        protected float m_CurrentHealth;
        [SerializeField]
        protected float m_MaxHealth = 100f;
        [SerializeField]
        protected bool m_DeactivateOnDeath;
        [SerializeField]
        protected float m_DeactivateOnDeathDelay = 5f;
        [SerializeField]
        protected GameObject[] m_SpawnedObjectsOnDeath;
        [SerializeField]
        protected GameObject[] m_DestroyedObjectsOnDeath;
        [SerializeField]
        protected LayerMask m_DeathLayer;
        [SerializeField]
        protected float m_TimeInvincibleAfterSpawn;

        protected GameObject m_GameObject;
        protected Transform m_Transform;


        //
        // Properties
        // 
        public bool Invincible{
            set { m_Invincible = value; }
        }

        public float MaxHealth{
            get { return m_MaxHealth; }
            set { m_MaxHealth = value; }
        }

        public float CurrentHealth{
            get { return m_CurrentHealth; }
        }



        //
        // Methods
        // 
        protected virtual void Awake()
        {
            m_CurrentHealth = m_MaxHealth;
            m_GameObject = gameObject;
            m_Transform = transform;


        }


		private void OnEnable()
		{
            //EventHandler.RegisterEvent<float, Vector3, Vector3, GameObject>(m_GameObject, "OnTakeDamage", TakeDamage);
            //EventHandler.RegisterEvent<Vector3, Vector3, GameObject>(m_GameObject, "Death", Death);
		}


		private void OnDisable()
		{
            //EventHandler.UnregisterEvent<float, Vector3, Vector3, GameObject>(m_GameObject, "OnTakeDamage", TakeDamage);
            //EventHandler.UnregisterEvent<Vector3, Vector3, GameObject>(m_GameObject, "Death", Death);
		}



		public void SetHealth(float value)
        {
            m_CurrentHealth += value;
        }


        public virtual void TakeDamage(float amount, Vector3 hitLocation, Vector3 hitDirection, GameObject attacker)
        {
            if (m_Invincible) return;

            if(m_CurrentHealth > 0){
                EventHandler.ExecuteEvent(gameObject, "OnTakeDamage", amount, hitLocation, hitDirection, attacker);
                //  Change health amount.
                m_CurrentHealth -= amount;

                //  If current health is zero, call death.
                if(m_CurrentHealth <= 0){
                    EventHandler.ExecuteEvent(gameObject, "Death", hitLocation, hitDirection, attacker);
                    EventHandler.ExecuteEvent(gameObject, "Ragdoll", 0f);
                }
            }
        }


        public bool IsAlive(){
            return m_CurrentHealth >= 0;
        }


        public void InstantDealth(){
            m_CurrentHealth = 0;
        }


        public virtual void Heal(float amount)
        {
            if(m_CurrentHealth + amount > m_MaxHealth){
                m_CurrentHealth = m_MaxHealth;
            }
        }


        protected virtual void Death(Vector3 hitLocation, Vector3 hitDirection, GameObject attacker)
        {
            //Debug.LogFormat("{0} killed by {1}", m_GameObject.name, attacker.name);
            //  Deactivate gameobject on death.


        }




    }

}

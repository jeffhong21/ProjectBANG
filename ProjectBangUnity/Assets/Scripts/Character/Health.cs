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
        [SerializeField]
        protected float m_CurrentHealth;
        [SerializeField]
        protected float m_MaxHealth = 100f;
        [SerializeField]
        protected bool m_DeactivateOnDeath;
        [SerializeField]
        protected float m_DeactivateOnDeathDelay;
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


        public void SetHealth(float value)
        {
            m_CurrentHealth += value;
        }


        public virtual void TakeDamage(float amount, Vector3 hitLocation, Vector3 hitDirection, GameObject attacker)
        {
            if (m_Invincible) return;

            if(m_CurrentHealth > 0){
                //  Change health amount.
                m_CurrentHealth -= amount;

                //  If current health is zero, call death.
                if(m_CurrentHealth <= 0){

                    //  Deactivate gameobject on death.
                    if(m_DeactivateOnDeath){
                        StartCoroutine(DeactivateDelay());
                    }
                }
            }
        }


        public bool IsAlive()
        {
            return m_CurrentHealth >= 0;
        }


        public void InstantDealth()
        {
            m_CurrentHealth = 0;
        }


        public virtual void Heal(float amount)
        {
            if(m_CurrentHealth + amount > m_MaxHealth){
                m_CurrentHealth = m_MaxHealth;
            }
        }


        private IEnumerator DeactivateDelay()
        {
            yield return new WaitForSeconds(m_DeactivateOnDeathDelay);;

            float startSinkingTime = Time.time;
            float sinkSpeed = 0.5f;

            while(true)
            {
                m_Transform.Translate(-Vector3.up * sinkSpeed * Time.deltaTime);
                if (Time.time > startSinkingTime + 2f){
                    m_GameObject.SetActive(false);
                    yield break;
                }
                yield return null;
            }
        }




    }

}

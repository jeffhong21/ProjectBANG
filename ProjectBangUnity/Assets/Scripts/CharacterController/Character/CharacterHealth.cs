namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections;

    public class CharacterHealth : Health
    {



        public override void TakeDamage(float amount, Vector3 position, Vector3 force, GameObject attacker, GameObject hitGameObject)
        {
            if (m_Invincible) return;

            if (m_CurrentHealth > 0)
            {
                EventHandler.ExecuteEvent(gameObject, EventIDs.OnTakeDamage, amount, position, force, attacker);
                //  Change health amount.
                m_CurrentHealth -= amount;
                SpawnParticles(m_DamageEffect, position);

                //var rigb = hitGameObject.GetComponent<Rigidbody>();
                //rigb.AddForceAtPosition(force * 100, position, ForceMode.Impulse);
                //Debug.LogFormat("{0} hit {1}", attacker.name, hitGameObject.name);
                if (m_CurrentHealth <= 0){
                    Die(position, force, attacker);
                }
            }
        }



        protected override void Die(Vector3 position, Vector3 force, GameObject attacker)
        {
            base.Die(position, force, attacker);

            //Debug.LogFormat("{0} killed by {1}", m_GameObject.name, attacker.name);

            GetComponent<CharacterIK>().enabled = false;

             // Deactivate gameobject on death.
            if (m_DeactivateOnDeath)
            {
                StartCoroutine(DeactivateDelay());
            }
        }



        private IEnumerator DeactivateDelay()
        {
            yield return new WaitForSeconds(m_DeactivateOnDeathDelay); ;

            float startSinkingTime = Time.time;
            float sinkSpeed = 0.5f;

            while (true)
            {
                m_Transform.Translate(-Vector3.up * sinkSpeed * Time.deltaTime);
                if (Time.time > startSinkingTime + 2f)
                {
                    m_GameObject.SetActive(false);
                    yield break;
                }
                yield return null;
            }
        }
    }

}

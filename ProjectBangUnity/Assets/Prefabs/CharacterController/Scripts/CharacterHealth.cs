namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections;

    public class CharacterHealth : Health
    {



        protected override void Death(Vector3 hitLocation, Vector3 hitDirection, GameObject attacker)
        {
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
            yield return new WaitForSeconds(m_DeactivateOnDeathDelay/2); ;

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

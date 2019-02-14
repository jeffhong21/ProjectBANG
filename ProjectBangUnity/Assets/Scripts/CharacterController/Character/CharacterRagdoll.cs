namespace CharacterController
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;


    public class CharacterRagdoll : MonoBehaviour
    {
        protected Rigidbody m_Rigidbody;
        protected Collider m_Collider;
        protected Animator m_Aniimator;
        protected GameObject m_GameObject;

        List<Collider> m_RagdollColliders = new List<Collider>();
        List<Rigidbody> m_RagdollRigb = new List<Rigidbody>();


        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Collider = GetComponent<Collider>();
            m_Aniimator = GetComponent<Animator>();
            m_GameObject = gameObject;

            SetupRagdoll();
        }


		private void OnEnable()
		{
            EventHandler.RegisterEvent<float>(m_GameObject, "Ragdoll", EnableRagdoll);
		}

		private void OnDisable()
		{
            EventHandler.UnregisterEvent<float>(m_GameObject, "Ragdoll", EnableRagdoll);
		}


		private void SetupRagdoll()
        {
            Rigidbody[] rigids = m_GameObject.GetComponentsInChildren<Rigidbody>();

            for (int i = 0; i < rigids.Length; i++)
            {
                if (rigids[i] == m_Rigidbody)
                {
                    continue;
                }

                Collider col = rigids[i].gameObject.GetComponent<Collider>();
                col.isTrigger = true;
                m_RagdollRigb.Add(rigids[i]);
                m_RagdollColliders.Add(col);
                rigids[i].isKinematic = true;
                rigids[i].mass *= 10;
                //r.gameObject.layer = 10;
            }

        }


        public void EnableRagdoll(float t)
        {
            StartCoroutine(EnableRagdoll_AfterDelay(t));
        }


        IEnumerator EnableRagdoll_AfterDelay(float t)
        {
            yield return new WaitForSeconds(t);
            EnableRagdoll_Actual();

            yield return new WaitForEndOfFrame();
            m_Aniimator.enabled = false; //  this will stop ragdolls from exploding.
            m_Collider.enabled = false;
            m_Rigidbody.isKinematic = true;
        }


        void EnableRagdoll_Actual()
        {
            for (int i = 0; i < m_RagdollColliders.Count; i++)
            {

                m_RagdollColliders[i].isTrigger = false;
                m_RagdollRigb[i].isKinematic = false;
            }
        }
    }
}
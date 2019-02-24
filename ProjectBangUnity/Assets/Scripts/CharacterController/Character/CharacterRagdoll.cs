namespace CharacterController
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;


    public class CharacterRagdoll : MonoBehaviour
    {
        protected CharacterLocomotion m_Controller;
        protected Rigidbody m_Rigidbody;
        protected Collider m_Collider;
        protected Animator m_Aniimator;
        protected GameObject m_GameObject;
        protected Transform m_Transform;

        List<Collider> m_RagdollColliders = new List<Collider>();
        List<Rigidbody> m_RagdollRigb = new List<Rigidbody>();

        public LayerMask m_RagdollLayer = ~(1 << 11);
        public LayerMask m_IgnoreForGround = ~(1 << 10 | 1 << 11);


        private void Awake()
        {
            m_Controller = GetComponent<CharacterLocomotion>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Collider = GetComponent<Collider>();
            m_Aniimator = GetComponent<Animator>();
            m_GameObject = gameObject;
            m_Transform = transform;
            SetupRagdoll();

            //m_RagdollLayer = ~(1 << 11);
            //m_IgnoreForGround = ~(1 << 10 | 1 << 11);
        }


		private void OnEnable()
		{
            EventHandler.RegisterEvent<Vector3, Vector3, float>(m_GameObject, "OnRagdoll", EnableRagdoll);
		}

		private void OnDisable()
		{
            EventHandler.UnregisterEvent<Vector3, Vector3, float>(m_GameObject, "OnRagdoll", EnableRagdoll);
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
                rigids[i].gameObject.layer = 11;

                if(rigids[i].GetComponent<DamageReciever>() == null){
                    rigids[i].gameObject.AddComponent<DamageReciever>();
                }
            }
        }


        public void EnableRagdoll(Vector3 position, Vector3 direction, float t)
        {
            StartCoroutine(EnableRagdoll_AfterDelay(position, direction, t));
        }


        IEnumerator EnableRagdoll_AfterDelay(Vector3 position, Vector3 direction, float t)
        {
            yield return new WaitForSeconds(t);
            EnableRagdoll_Actual(position, direction);

            yield return new WaitForEndOfFrame();
            m_Controller.enabled = false;
            m_Aniimator.enabled = false; //  this will stop ragdolls from exploding.
            m_Collider.enabled = false;
            m_Rigidbody.isKinematic = true;
        }


        void EnableRagdoll_Actual(Vector3 position, Vector3 direction)
        {
            var dir = position - m_Transform.position;
            for (int i = 0; i < m_RagdollColliders.Count; i++)
            {
                m_RagdollColliders[i].enabled = true;
                m_RagdollColliders[i].isTrigger = false;
            }
            for (int i = 0; i < m_RagdollRigb.Count; i++)
            {
                m_RagdollRigb[i].isKinematic = false;
                m_RagdollRigb[i].useGravity = true;
                m_RagdollRigb[i].AddExplosionForce(500f, direction, 50f);
            }
        }
    }
}
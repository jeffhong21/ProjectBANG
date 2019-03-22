using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterController
{
    [RequireComponent(typeof(SphereCollider), typeof(AudioSource))]
    public class CharacterFootTrigger : MonoBehaviour
    {
        [SerializeField]
        protected float radius = 0.1f;
        protected SphereCollider trigger;
        protected AudioSource audioSource;
        protected CharacterFootsteps m_Footsteps;

        private float timeDelay;

        public Collider Trigger{
            get { return trigger; }
        }

        public AudioSource AudioSource{
            get { return audioSource; }
        }


        private void Awake()
        {
            trigger = GetComponent<SphereCollider>();
            audioSource = GetComponent<AudioSource>();

            trigger.isTrigger = true;
            trigger.radius = radius;
            audioSource.playOnAwake = false;

        }

		private void OnEnable()
		{
            timeDelay = Time.timeSinceLevelLoad + 0.5f;
		}


		public void Init(CharacterFootsteps footsteps)
        {
            m_Footsteps = footsteps;
        }


        private void OnTriggerEnter(Collider other)
        {
            if(Time.timeSinceLevelLoad > timeDelay){
                if (other.CompareTag("Ground")){
                    if (m_Footsteps != null)
                    {
                        m_Footsteps.StepOnMesh(this);
                        m_Footsteps.PlayFootFallSound(this);
                    }
                }
            }




        }
    }
}

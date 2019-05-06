using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterController
{
    [RequireComponent(typeof(SphereCollider), typeof(AudioSource))]
    public class CharacterFootTrigger : MonoBehaviour
    {
        private string[] m_GroundTags = { "Ground" };

        [SerializeField]
        protected float radius = 0.1f;
        protected SphereCollider trigger;
        protected AudioSource audioSource;
        protected CharacterFootsteps m_Footsteps;
        protected LayerManager layerManager;

        private float timeDelay;

        public Collider Trigger{
            get { return trigger; }
        }

        public AudioSource AudioSource{
            get { return audioSource; }
        }


        private void Awake()
        {
            layerManager = GetComponentInParent<LayerManager>();
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
                for (int i = 0; i < m_GroundTags.Length; i++)
                {
                    //Debug.Log(other.gameObject.name + " | " + other.gameObject.layer);
                    //if(other.gameObject.layer == layerManager.GroundLayer){
                    //    if (m_Footsteps != null)
                    //    {
                    //        m_Footsteps.StepOnMesh(this);
                    //        m_Footsteps.PlayFootFallSound(this);
                    //    }
                    //}
                    if (other.CompareTag(m_GroundTags[i]))
                    {
                        if (m_Footsteps != null)
                        {
                            m_Footsteps.StepOnMesh(this);
                            m_Footsteps.PlayFootFallSound(this);
                        }
                    }
                }
                ////  Compare Layers.
                //if(other.gameObject.layer == LayerMask.NameToLayer("Solid")){
                //    if (m_Footsteps != null){
                //        m_Footsteps.StepOnMesh(this);
                //        m_Footsteps.PlayFootFallSound(this);
                //    }
                //}
                ////  Compare Tags.
                //else{
                //    for (int i = 0; i < m_GroundTags.Length; i++){
                //        if (other.CompareTag(m_GroundTags[i])){
                //            if (m_Footsteps != null){
                //                m_Footsteps.StepOnMesh(this);
                //                m_Footsteps.PlayFootFallSound(this);
                //            }
                //        }
                //    }
                //}


            }




        }
    }
}

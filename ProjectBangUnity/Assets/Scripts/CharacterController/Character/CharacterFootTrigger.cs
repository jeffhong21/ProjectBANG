using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterController
{
    [RequireComponent(typeof(SphereCollider))]
    public class CharacterFootTrigger : MonoBehaviour
    {
        protected Collider trigger;
        protected CharacterFootsteps m_Footsteps;
        [SerializeField]
        protected FootStepObject footStepObject;


        private float radius = 0.1f;


        public Collider Trigger
        {
            get { return trigger; }
        }


        private void Awake()
        {
            if(trigger == null) trigger = GetComponent<SphereCollider>();
            trigger.isTrigger = true;
            //footStepObject = new FootStepObject(transform);
        }


        public void Init(CharacterFootsteps footsteps)
        {
            m_Footsteps = footsteps;
            trigger = GetComponent<SphereCollider>();
            trigger.isTrigger = true;
            var sphereTrigger = (SphereCollider)trigger;
            sphereTrigger.radius = radius;
        }


        private void OnTriggerEnter(Collider other)
        {
            if(m_Footsteps != null)
            {
                //if(other.GetComponent<MeshRenderer>())
                    //footStepObject.ground = other;

                m_Footsteps.StepOnMesh(footStepObject);
                m_Footsteps.PlayFootFallSound(footStepObject);
            }


        }
    }
}

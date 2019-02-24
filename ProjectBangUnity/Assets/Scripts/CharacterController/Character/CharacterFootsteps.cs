using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterController
{
    public class CharacterFootsteps : MonoBehaviour
    {
        public float minVelocity = 0.8f;
        public float footOffset = 0.08f;
        public float interval = 0.3f;

        public bool debugTextureName;
        public CharacterFootTrigger m_leftFootTrigger;
        public CharacterFootTrigger m_reftFootTrigger;
        public Transform m_currentStep;



        protected CharacterLocomotion m_Controller;
        protected LayerManager m_Layers;
        protected Animator m_Animator;
        protected GameObject m_GameObject;
        protected Transform m_Transform;


        private float nextInterval;


		private void Awake()
		{
            m_Animator = GetComponent<Animator>();
            m_GameObject = gameObject;
            m_Transform = transform;

            if (m_leftFootTrigger == null || m_reftFootTrigger == null)
                AddCharacterFootTriggers();
		}



		private void Update()
		{
            if(Time.time > nextInterval){
                nextInterval += interval;
                if(m_Controller.Velocity.sqrMagnitude > minVelocity){
                    
                }
            }
		}




		public void StepOnMesh(FootStepObject footStepObject)
        {
            if (m_Controller.Velocity.sqrMagnitude > minVelocity)
            {
                m_currentStep = footStepObject.sender;
                if (debugTextureName)
                {
                    Debug.LogFormat("{0}", m_currentStep);
                }
            }

        }

        public void PlayFootFallSound(FootStepObject footStepObject)
        {

        }


        public void AddCharacterFootTriggers()
        {
            var leftFoot = m_Animator.GetBoneTransform(HumanBodyBones.LeftFoot).gameObject;
            m_leftFootTrigger = leftFoot.AddComponent<CharacterFootTrigger>();
            m_leftFootTrigger.Init(this);

            var rightFoot = m_Animator.GetBoneTransform(HumanBodyBones.RightFoot).gameObject;
            m_reftFootTrigger = rightFoot.AddComponent<CharacterFootTrigger>();
            m_reftFootTrigger.Init(this);
        }

	}





    [Serializable]
    public class FootStepObject
    {

        public string name;
        [HideInInspector]
        public Transform sender;
        [HideInInspector]
        public Collider ground;
        [HideInInspector]
        public Terrain terrain;

        public Renderer renderer;


        public bool isTerrain{
            get { return terrain; }
        }


        public FootStepObject(Transform sender){
            this.sender = sender;
        }

        public FootStepObject(Transform sender, Collider ground)
        {
            this.sender = sender;
            this.ground = ground;
        }
    }

}

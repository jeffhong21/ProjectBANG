using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterController
{
    public class CharacterFootsteps : MonoBehaviour
    {
        public float minVelocity = 0.8f;
        [Range(0, 0.2f)]
        public float footOffset = 0.08f;
        public float interval = 0.3f;

        public GameObject m_RightDecal;
        public GameObject m_LeftDecal;

        public bool debugTextureName;
        public CharacterFootTrigger m_leftFootTrigger;
        public CharacterFootTrigger m_rightFootTrigger;
        public Transform m_currentStep;



        protected CharacterLocomotion m_Controller;
        protected LayerManager m_Layers;
        protected Animator m_Animator;
        protected GameObject m_GameObject;
        protected Transform m_Transform;


        private float nextInterval;


		private void Awake()
		{
            m_Controller = GetComponent<CharacterLocomotion>();
            m_Animator = GetComponent<Animator>();
            m_GameObject = gameObject;
            m_Transform = transform;

            if (m_leftFootTrigger == null || m_rightFootTrigger == null)
                AddCharacterFootTriggers();

            m_leftFootTrigger.Init(this);
            m_rightFootTrigger.Init(this);
		}



		private void Update()
		{
            //if(Time.time > nextInterval){
            //    nextInterval += interval;
            //    if(m_Controller.Velocity.sqrMagnitude > minVelocity){
                    
            //    }
            //}
		}




		public void StepOnMesh(FootStepObject footStepObject)
        {
            //Debug.LogFormat("{0}", m_Controller.Velocity.sqrMagnitude);
            if (m_Controller.Velocity.sqrMagnitude > minVelocity)
            {

                m_currentStep = footStepObject.sender;

                //var decal = Instantiate(footStepObject.ID == 0 ? m_RightDecal : m_LeftDecal, Vector3.zero, Quaternion.identity, m_currentStep).transform;
                //decal.parent = null;
                //var position = decal.position;
                //position.y = 0;
                //decal.position = position;
                //decal.rotation = Quaternion.identity;
                //decal.localScale = Vector3.one;

                //Destroy(decal.gameObject, 5);

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
            m_rightFootTrigger = rightFoot.AddComponent<CharacterFootTrigger>();
            m_rightFootTrigger.Init(this);
        }

	}





    [Serializable]
    public class FootStepObject
    {
        public int ID;
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

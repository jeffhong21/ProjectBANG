
namespace CharacterController
{
    using UnityEngine;

    public class CharacterFootsteps : MonoBehaviour
    {
        [SerializeField]
        protected bool m_SpawnFootprints;
        [SerializeField]
        protected GameObject m_Decal;
        [SerializeField]
        protected AudioClip[] footstepClips = new AudioClip[0];
        [SerializeField]
        protected CharacterFootTrigger m_leftFootTrigger;
        [SerializeField]
        protected CharacterFootTrigger m_rightFootTrigger;
        [SerializeField]
        protected Transform m_currentStep;
        [SerializeField]
        protected float m_FootstepTimer = 0.25f;
        private float m_FootstepThreshold;


        protected CharacterLocomotion m_Controller;
        protected LayerManager m_Layers;
        protected Animator m_Animator;
        protected GameObject m_GameObject;
        protected Transform m_Transform;




		private void Awake()
		{
            m_Controller = GetComponent<CharacterLocomotion>();
            m_Layers = GetComponent<LayerManager>();
            m_Animator = GetComponent<Animator>();
            m_GameObject = gameObject;
            m_Transform = transform;



            if (m_leftFootTrigger == null) AddCharacterFootTriggers(m_Animator.GetBoneTransform(HumanBodyBones.LeftToes), out m_leftFootTrigger);
            if (m_rightFootTrigger == null) AddCharacterFootTriggers(m_Animator.GetBoneTransform(HumanBodyBones.RightToes), out m_rightFootTrigger);

            m_leftFootTrigger.Init(this);
            m_rightFootTrigger.Init(this);
		}



		private void OnEnable()
		{
			
		}



		public void StepOnMesh(CharacterFootTrigger sender)
        {
            if(Time.timeSinceLevelLoad > m_FootstepThreshold && m_SpawnFootprints)
            {
                m_currentStep = sender.transform;
                RaycastHit hit;
                if (Physics.Raycast(m_currentStep.position, m_currentStep.TransformVector(-m_currentStep.up), out hit, 1f, m_Layers.GroundLayer))
                {
                    //var footStep = Instantiate(m_Decal, null);
                    var position = hit.point + m_Controller.transform.up * 0.02f;
                    var rotation = Quaternion.LookRotation(m_Controller.transform.forward, Vector3.up);
                    //if (m_currentStep.localPosition.x > 0){
                    //    rotation = rotation * Quaternion.Euler(0, 0, 180);
                    //}

                    //var rotationDirection = Vector3.Cross(hit.point - m_currentStep.transform.position, -m_currentStep.forward);
                    //var newRotation = Quaternion.FromToRotation(m_Transform.forward, rotationDirection);
                    //rotation = rotation * newRotation;

                    var footStep = ObjectPool.Instantiate(m_Decal, position, rotation);
                }

                m_FootstepThreshold = Time.timeSinceLevelLoad + m_FootstepTimer;
            }

        }

        public void PlayFootFallSound(CharacterFootTrigger sender)
        {
            var index = Random.Range(0, footstepClips.Length);
            var clip = footstepClips[index];
            sender.AudioSource.clip = clip;
            sender.AudioSource.Play();
        }



        private void AddCharacterFootTriggers(Transform foot, out CharacterFootTrigger footTrigger)
        {
            footTrigger = foot.GetComponent<CharacterFootTrigger>();
            if(footTrigger == null){
                footTrigger = foot.gameObject.AddComponent<CharacterFootTrigger>();
            }
        }

	}





    [System.Serializable]
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

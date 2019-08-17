
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
        protected float m_FootstepTimer = 0.1f;
        [SerializeField]
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


		}



		private void OnEnable()
		{
            m_leftFootTrigger.Init(this);
            m_rightFootTrigger.Init(this);
        }



		public void StepOnMesh(CharacterFootTrigger sender)
        {
            if (m_Controller.Grounded && Time.timeSinceLevelLoad > m_FootstepThreshold && m_SpawnFootprints)
            {
                if(sender.transform.position.y < m_Controller.transform.position.y + 0.1f)
                {
                    var position = sender.transform.position + m_Controller.transform.up * 0.02f;
                    var fwdDirection = Vector3.Cross(m_Transform.right, m_Controller.GroundHit.normal);
                    var rotation = Quaternion.LookRotation(fwdDirection, m_Controller.GroundHit.normal);

                    ObjectPool.Get(m_Decal, position, rotation);

                    PlayFootFallSound(sender);
                }
                m_FootstepThreshold = Time.timeSinceLevelLoad + m_FootstepTimer;
                m_currentStep = sender.transform;
            }

        }

        public void PlayFootFallSound(CharacterFootTrigger sender)
        {
            if (m_Controller.Grounded && Time.timeSinceLevelLoad > m_FootstepThreshold && m_SpawnFootprints)
            {
                var index = Random.Range(0, footstepClips.Length);
                var clip = footstepClips[index];
                sender.AudioSource.clip = clip;
                sender.AudioSource.Play();

            }

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

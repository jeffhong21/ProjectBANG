
namespace CharacterController
{
    using UnityEngine;

    public class CharacterFootsteps : MonoBehaviour
    {
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

            //if (m_leftFootTrigger == null || m_rightFootTrigger == null)
                //AddCharacterFootTriggers();

            if (m_leftFootTrigger == null) AddCharacterFootTriggers(m_Animator.GetBoneTransform(HumanBodyBones.LeftFoot), out m_leftFootTrigger);
            if (m_rightFootTrigger == null) AddCharacterFootTriggers(m_Animator.GetBoneTransform(HumanBodyBones.RightFoot), out m_rightFootTrigger);

            m_leftFootTrigger.Init(this);
            m_rightFootTrigger.Init(this);
		}



        public void StepOnMesh(CharacterFootTrigger sender)
        {
            m_currentStep = sender.transform;
            RaycastHit hit;
            if (Physics.Raycast(m_currentStep.position, m_currentStep.TransformVector(-m_currentStep.up), out hit, 1f, m_Layers.SolidLayer))
            {
                var footStep = Instantiate(m_Decal, null);
                var rotation = Quaternion.LookRotation(m_Transform.forward, Vector3.up);
                var position = hit.point + Vector3.up * 0.001f;
                if (m_currentStep.localPosition.x > 0){
                    rotation = rotation * Quaternion.Euler(0, 0, 180);
                }

                footStep.transform.position = position;
                footStep.transform.rotation = rotation;
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

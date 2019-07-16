namespace CharacterController
{
    using UnityEngine;


    public class Item : MonoBehaviour
    {
        //
        // Fields
        //
        [SerializeField]
        protected ItemType m_ItemType;
        [SerializeField]
        protected int m_SlotID = -1;
        [SerializeField]
        protected GameObject m_DropPrefab;
        [SerializeField]
        protected Sprite m_Icon;
        [SerializeField]
        protected bool m_ShowCrosshairsOnAim;

        [SerializeField]
        protected GameObject m_Object;
        [SerializeField]
        protected Transform m_HolsterTarget;
        [SerializeField]
        protected int m_HolsterID = -1;

        [SerializeField, Tooltip("The ik position of the non dominant hand.")]      //  Currently not used.
        protected Transform m_NonDominantHandIKTarget;
        [SerializeField]
        protected Transform m_NonDominantHandIKTargetHint;


        [SerializeField]
        protected Vector3 m_LocalSpawnPosition;
        [SerializeField]
        protected Vector3 m_LocalSpawnRotation;





        public ItemType ItemType{ get { return m_ItemType; } }
        public int SlotID { get { return m_SlotID; } set { m_SlotID = value; } }
        public GameObject DropPrefab { get { return m_DropPrefab; } set { m_DropPrefab = value; }}





        protected Animator m_Animator;
        [SerializeField, DisplayOnly]
        protected GameObject m_Character;
        [SerializeField, DisplayOnly]
        protected CharacterLocomotion m_Controller;
        protected Inventory m_Inventory;
        protected GameObject m_GameObject;
        protected Transform m_Transform;


        //
        // Methods
        //
        protected virtual void Awake()
        {

            m_GameObject = gameObject;
            m_Transform = transform;

            if(m_NonDominantHandIKTarget == null){
                m_NonDominantHandIKTarget = new GameObject("NonDominantHandIKTarget").transform;
                m_NonDominantHandIKTarget.parent = transform;
                m_NonDominantHandIKTarget.localPosition = Vector3.zero;
                m_NonDominantHandIKTarget.localEulerAngles = new Vector3(3, 50, 180);
            }
        }


		protected void OnEnable()
		{

		}


		protected void OnDisable()
		{

		}


		public virtual void Initialize(Inventory inventory)
        {
            m_Controller = inventory.GetComponent<CharacterLocomotion>();
            m_Character = inventory.gameObject;
            m_Inventory = inventory;
            m_Animator = m_Controller.GetComponent<Animator>();
        }


        public virtual void SetActive(bool active)
        {
            gameObject.SetActive(active);

            if(active)
                ItemActivated();
            else
                ItemDeactivated();
        }


        protected virtual void ItemActivated()
        {
            if (m_HolsterTarget){
                m_GameObject.SetActive(true);
            }
            m_GameObject.SetActive(true);
            //Debug.LogFormat("Setting {0} active Actived", gameObject.name);
        }


        protected virtual void ItemDeactivated()
        {
            if (m_HolsterTarget){
                m_GameObject.SetActive(false);
            } else {
                m_GameObject.SetActive(false);
            }
            //Debug.LogFormat("Setting {0} active as Deactivated", gameObject.name);
        }

        /// <summary>
        /// Callback from the controller when the item starts to aim.
        /// </summary>
        protected virtual void OnStartAim()
        {
            
        }


        /// <summary>
        /// Callback from the controller when the item is aimed or no longer aimed.
        /// </summary>
        /// <param name="aim">If set to <c>true</c> aim.</param>
        protected virtual void OnAim(bool aim)
        {
            
        }

    }

}
namespace CharacterController
{
    using JH_Utils;
    using UnityEngine;


    public class ItemObject : MonoBehaviour
    {
        //
        // Fields
        //
        [Header("--  States  --")]
        [SerializeField]
        protected string m_ItemName;
        [SerializeField]
        protected int m_ItemID;
        [SerializeField]
        protected int m_MovementSetID;
        [SerializeField]
        protected ItemAnimStates m_AnimStates = new ItemAnimStates();
        [Header("--  Item Settings --")]
        [SerializeField]
        protected bool m_CanAim = true;
        [SerializeField]
        protected bool m_RequiredAim = true;
        [SerializeField]
        protected bool m_TwoHandedItem;
        [SerializeField, Tooltip("The ik position of the non dominant hand.")]      //  Currently not used.
        protected Transform m_NonDominantHandPosition;
        [SerializeField]
        protected Vector3 m_PositionOffset;
        [SerializeField]
        protected Vector3 m_RotationOffset;


        [SerializeField]
        protected GameObject m_CrosshairsSprite;


        //protected Collider[] m_Collider;
        protected GameObject m_GameObject;
        protected Transform m_Transform;
        protected Animator m_Animator;
        [SerializeField, DisplayOnly]
        protected GameObject m_Character;
        [SerializeField, DisplayOnly]
        protected CharacterLocomotion m_Controller;
        protected Inventory m_Inventory;





        public string ItemName
        {
            get { return m_ItemName; }
            set { m_ItemName = value; }
        }

        public int ItemID
        {
            get { return m_ItemID; }
        }

        public int MovementSetID
        {
            get { return m_MovementSetID; }
        }

        public ItemAnimStates AnimStates
        {
            get { return m_AnimStates; }
        }

        public Vector3 PositionOffset
        {
            get { return m_PositionOffset; }
        }

        public Vector3 RotationOffset
        {
            get { return m_RotationOffset; }
        }

        public Transform NonDominantHandPosition
        {
            get { return m_NonDominantHandPosition; }
        }


        public GameObject CrosshairsSprite
        {
            get { return m_CrosshairsSprite; }
        }



        //
        // Methods
        //
        protected virtual void Awake()
        {
            m_Animator = GetComponent<Animator>();

            m_GameObject = gameObject;
            m_Transform = transform;


            if(m_NonDominantHandPosition == null){
                m_NonDominantHandPosition = new GameObject("nonDominantHandIK").transform;
                m_NonDominantHandPosition.parent = transform;
                m_NonDominantHandPosition.localPosition = Vector3.zero;
                m_NonDominantHandPosition.localEulerAngles = new Vector3(3, 50, 180);
            }
        }


		protected void OnEnable()
		{
            if (m_Controller)
            {
                m_Controller.OnAim += OnAim;
            }
		}


		protected void OnDisable()
		{
            if (m_Controller)
            {
                m_Controller.OnAim -= OnAim;
            }
		}


		public virtual void Initialize(Inventory inventory)
        {
            m_Character = inventory.gameObject;
            m_Inventory = inventory;
            m_Controller = m_Character.GetComponent<CharacterLocomotion>();

            m_Controller.OnAim += OnAim;

            //Debug.LogFormat("Initializing Weapon to {0}", m_Character);
        }

        public virtual void SetActive(bool active)
        {
            gameObject.SetActive(active);

            if(active) ItemActivated();
            else if (!active) ItemDeactivated();
        }


        protected virtual void ItemActivated()
        {
            //if (m_HolsterTarget)
            //{
            //    m_GameObject.SetActive(true);
            //}
            //gameObject.SetActive(true);
            //Debug.LogFormat("Setting {0} active Actived", gameObject.name);
        }


        protected virtual void ItemDeactivated()
        {
            //if (m_HolsterTarget)
            //{
            //    m_GameObject.SetActive(false);
            //}
            //gameObject.SetActive(false);
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
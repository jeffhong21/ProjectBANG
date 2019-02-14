namespace CharacterController
{
    using JH_Utils;
    using UnityEngine;


    public class Item : MonoBehaviour
    {
        //
        // Fields
        //
        [Header("States")]
        [SerializeField]
        protected string m_ItemName;
        [SerializeField]
        protected string m_DefaultStates;
        [SerializeField]
        protected string m_AimStates;
        [SerializeField]
        protected string m_EquipStates;
        [SerializeField]
        protected string m_UnequipStates;
        [Header("Settings")]
        [SerializeField]
        protected bool m_CanAim = true;
        [SerializeField]
        protected bool m_RequiredAim = true;
        [SerializeField]
        protected bool m_TwoHandedItem;
        [SerializeField, Tooltip("The ik position of the non dominant hand.")]      //  Currently not used.
        protected Transform m_NonDominantHandPosition;
        [SerializeField, Tooltip("Where should the item be placed when unequipped.")]
        protected Transform m_HolsterTarget;
        [SerializeField, Tooltip("The game object??")]
        protected GameObject m_ItemPickup;
        [SerializeField]
        protected ItemPositions m_ItemPosition;
        [SerializeField]
        protected ItemPositions m_HandIKPosition;

        [SerializeField]
        protected GameObject m_CrosshairsSprite;

        //protected Collider[] m_Collider;
        protected GameObject m_GameObject;
        protected Transform m_Transform;
        protected Animator m_Animator;
        [SerializeField, DisplayOnly]
        protected GameObject m_Character;
        protected CharacterLocomotion m_Controller;
        protected Inventory m_Inventory;





        public string ItemName
        {
            get { return m_ItemName; }
            set { m_ItemName = value; }
        }

        public string DefaultStates
        {
            get { return m_DefaultStates; }
        }

        public string AimStates
        {
            get { return m_AimStates; }
        }

        public string EquipStates
        {
            get { return m_EquipStates; }
        }

        public string UnequipStates
        {
            get { return m_UnequipStates; }
        }


        public ItemPositions ItemPosition
        {
            get { return m_ItemPosition; }
        }

        public ItemPositions HandIKPosition
        {
            get { return m_HandIKPosition; }
        }

        public Transform NonDominantHandPosition
        {
            get { return m_NonDominantHandPosition; }
        }

        public GameObject ItemPickup
        {
            get { return m_ItemPickup; }
        }

        public GameObject CrosshairsSprite
        {
            get { return m_CrosshairsSprite; }
        }



        //
        // Methods
        //
        public virtual void Awake()
        {

            m_GameObject = gameObject;
            m_Transform = transform;

            m_Controller = GetComponent<CharacterLocomotion>();
            m_Animator = GetComponent<Animator>();
        }


        public virtual void Initialize(Inventory inventory)
        {
            m_Character = inventory.gameObject;
            m_Inventory = inventory;
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
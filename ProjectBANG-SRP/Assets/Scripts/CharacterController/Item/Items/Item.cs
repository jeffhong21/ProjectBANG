namespace CharacterController
{
    using UnityEngine;


    public class Item : MonoBehaviour
    {
        //
        // Fields
        //
        [SerializeField]
        protected ItemType itemType;
        [SerializeField]
        protected int slotID = -1;
        [SerializeField]
        protected int animatorItemID;
        [SerializeField]
        protected int animatorMovementSetID;
        [SerializeField]
        protected GameObject dropPrefab;



        [SerializeField]
        protected Transform holsterTarget;


        [SerializeField, Tooltip("The ik position of the non dominant hand.")]      //  Currently not used.
        protected Transform m_NonDominantHandIKTarget;
        [SerializeField]
        protected Transform m_NonDominantHandIKTargetHint;


        [SerializeField]
        protected Vector3 localSpawnPosition;
        [SerializeField]
        protected Vector3 localSpawnRotation;





        public ItemType ItemType{ get { return itemType; } }

        public int SlotID { get { return slotID; } set { slotID = value; } }

        public int AnimatorItemID { get { return animatorItemID; } }

        public int AnimatorMovementSetID { get { return animatorMovementSetID; } }

        public GameObject DropPrefab { get { return dropPrefab; } set { dropPrefab = value; }}




        protected Animator m_Animator;
        [SerializeField, DisplayOnly]
        protected GameObject m_Character;
        protected CharacterLocomotion m_Controller;
        protected InventoryBase m_Inventory;
        protected GameObject m_GameObject;
        protected Transform m_Transform;


        //
        // Methods
        //
        protected virtual void Awake()
        {

        }


        //protected void OnEnable()
        //{
        //    //Debug.LogFormat("Character: {0}");

        //    EventHandler.RegisterEvent(m_Character, EventIDs.OnAnimatorEquipItem, () => SetActive(true));
        //    EventHandler.RegisterEvent(m_Character, EventIDs.OnAnimatorUnequipItem, () => SetActive(false));
        //    EventHandler.RegisterEvent(m_Character, EventIDs.OnAnimatorDropItem, ItemDeactivated);
        //    EventHandler.RegisterEvent(m_Character, EventIDs.OnAnimatorPickupItem, ItemActivated);
        //}

        //protected void OnDisable()
        //{
        //    EventHandler.UnregisterEvent(m_Character, EventIDs.OnAnimatorEquipItem, () => SetActive(true));
        //    EventHandler.UnregisterEvent(m_Character, EventIDs.OnAnimatorUnequipItem, () => SetActive(false));
        //    EventHandler.UnregisterEvent(m_Character, EventIDs.OnAnimatorDropItem, ItemDeactivated);
        //    EventHandler.UnregisterEvent(m_Character, EventIDs.OnAnimatorPickupItem, ItemActivated);
        //}


        public virtual void Initialize(InventoryBase inventory)
        {
            m_Controller = inventory.GetComponent<CharacterLocomotion>();
            m_Character = inventory.gameObject;
            m_Inventory = inventory;
            m_Animator = m_Controller.GetComponent<Animator>();

            m_GameObject = gameObject;
            m_Transform = transform;


            if (m_NonDominantHandIKTarget == null) {
                m_NonDominantHandIKTarget = new GameObject("NonDominantHandIKTarget").transform;
                m_NonDominantHandIKTarget.parent = transform;
                m_NonDominantHandIKTarget.localPosition = Vector3.zero;
                m_NonDominantHandIKTarget.localEulerAngles = new Vector3(3, 50, 180);
            }

            localSpawnPosition += transform.localPosition;
            localSpawnRotation += transform.localEulerAngles;



        }


        /// <summary>
        /// Activations and deactivates the item.  Also places it at the holster if there is one.
        /// </summary>
        /// <param name="active"></param>
        public virtual void SetActive(bool active)
        {
            Debug.LogFormat("{0} is now {1}", itemType.name, active);
            if (active) {
                if (holsterTarget) {
                    var parent = m_Inventory.GetItemEquipSlot(slotID);
                    if (parent != null) {
                        m_GameObject.transform.parent = parent;
                        m_GameObject.transform.localPosition = localSpawnPosition;
                        m_GameObject.transform.localEulerAngles = localSpawnRotation;
                        
                    }
                }
                m_GameObject.SetActive(true);

            } else {
                if (holsterTarget) {
                    m_GameObject.transform.parent = holsterTarget;
                    m_GameObject.transform.localPosition = Vector3.zero;
                    m_GameObject.transform.localEulerAngles = Vector3.zero;
                }
                m_GameObject.SetActive(false);
            }

        }


        /// <summary>
        /// Prepare item for use.
        /// </summary>
        protected virtual void ItemActivated()
        {
            m_GameObject.transform.localPosition = localSpawnPosition;
            m_GameObject.transform.localEulerAngles = localSpawnRotation;

        }

        /// <summary>
        /// Item is no longer equipped.
        /// </summary>
        protected virtual void ItemDeactivated()
        {


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


        public virtual bool TryUse()
        {
            return false;
        }
    }

}
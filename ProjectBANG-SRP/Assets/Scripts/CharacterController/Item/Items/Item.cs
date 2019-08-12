namespace CharacterController
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    public class Item : MonoBehaviour
    {

        [Serializable] public class EquipItemEvent : UnityEvent<bool> { }
        [Serializable] public class UnequipItemEvent : UnityEvent<bool> { }
        [Serializable] public class PickupItemEvent : UnityEvent { }
        [Serializable] public class DropItemEvent : UnityEvent { }
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

        [Space()]
        public EquipItemEvent OnEquipEvent = new EquipItemEvent();
        public UnequipItemEvent OnUnequipEvent = new UnequipItemEvent();


        public ItemType ItemType{ get { return itemType; } }

        public int SlotID { get { return slotID; } set { slotID = value; } }

        public int AnimatorItemID { get { return animatorItemID; } }

        public int AnimatorMovementSetID { get { return animatorMovementSetID; } }

        public GameObject DropPrefab { get { return dropPrefab; } set { dropPrefab = value; }}




        protected Animator animator;
        [SerializeField, DisplayOnly]
        protected GameObject character;
        protected CharacterLocomotion charLocomotion;
        protected Inventory inventory;
        protected GameObject mGameObject;
        protected Transform mTransform;


        //
        // Methods
        //
        protected virtual void Awake()
        {
            //character = transform.root.gameObject;
            //if (character != null) {
            //    inventory = character.GetComponent<Inventory>();
            //}


            OnEquipEvent.AddListener(SetActive);
            OnUnequipEvent.AddListener(SetActive);
        }

        protected virtual void OnDestroy()
        {

            if (character != null)
            {
                EventHandler.UnregisterEvent(character, EventIDs.OnAnimatorEquipItem, () => SetActive(true));
                EventHandler.UnregisterEvent(character, EventIDs.OnAnimatorUnequipItem, () => SetActive(false));
                //EventHandler.UnregisterEvent(character, EventIDs.OnAnimatorDropItem, ItemDeactivated);
                //EventHandler.UnregisterEvent(character, EventIDs.OnAnimatorPickupItem, ItemActivated);
            }

            OnEquipEvent.RemoveAllListeners();
            OnUnequipEvent.RemoveAllListeners();
        }




        public virtual void Initialize( Inventory _inventory )
        {
            charLocomotion = _inventory.GetComponent<CharacterLocomotion>();
            character = _inventory.gameObject;
            inventory = _inventory;
            animator = charLocomotion.GetComponent<Animator>();

            mGameObject = gameObject;
            mTransform = transform;

            //if (slotID > -1) {
            //    //  Parent the item to a item equip slot.
            //    Transform parent = inventory.GetItemEquipSlot(slotID);
            //    if (parent != null) {
            //        mGameObject.transform.parent = parent;
            //        mGameObject.transform.localPosition = localSpawnPosition;
            //        mGameObject.transform.localEulerAngles = localSpawnRotation;
            //    }
            //}

            if (character != null) {
                EventHandler.RegisterEvent(character, EventIDs.OnAnimatorEquipItem, () => SetActive(true));
                EventHandler.RegisterEvent(character, EventIDs.OnAnimatorUnequipItem, () => SetActive(false));
                //EventHandler.RegisterEvent(character, EventIDs.OnAnimatorDropItem, ItemDeactivated);
                //EventHandler.RegisterEvent(character, EventIDs.OnAnimatorPickupItem, ItemActivated);
                //Debug.LogFormat("Registering event equip event for {0}", itemType.name);
            }


            if (m_NonDominantHandIKTarget == null) {
                m_NonDominantHandIKTarget = new GameObject("NonDominantHandIKTarget").transform;
                m_NonDominantHandIKTarget.parent = mTransform;
                m_NonDominantHandIKTarget.localPosition = Vector3.zero;
                m_NonDominantHandIKTarget.localEulerAngles = new Vector3(3, 50, 180);
            }





        }


        /// <summary>
        /// Activations and deactivates the item.  Also places it at the holster if there is one.
        /// </summary>
        /// <param name="active"></param>
        public virtual void SetActive(bool active)
        {
            Debug.LogFormat("<b><color=red>{0}</color> is now {1}</b>", itemType.name, active);

            if (active) {
                if (holsterTarget)
                {
                    if(slotID > -1) {
                        //  Parent the item to a item equip slot.
                        Transform parent = inventory.GetItemEquipSlot(slotID);
                        if (parent != null) {
                            mGameObject.transform.parent = parent;
                            //mGameObject.transform.localPosition = Vector3.zero;
                            //mGameObject.transform.localEulerAngles = Vector3.zero;
                        }
                    }

                }

                //  Activate item.
                ItemActivated();
                //  Fire off events.
                //OnEquipEvent.Invoke(active);

            } else {
                if (holsterTarget){
                    //  Parent the object to the holster and reset position and rotation values.
                    mGameObject.transform.parent = holsterTarget;
                    mGameObject.transform.localPosition = Vector3.zero;
                    mGameObject.transform.localEulerAngles = Vector3.zero;
                }

                //  Deactivate the item.
                ItemDeactivated();
                //  Fire off events.
                //OnEquipEvent.Invoke(active);
            }
        }


        /// <summary>
        /// Prepare item for use. 
        /// </summary>
        protected virtual void ItemActivated()
        {
            mGameObject.transform.localPosition = localSpawnPosition;
            mGameObject.transform.localEulerAngles = localSpawnRotation;

            mGameObject.SetActive(true);



            //  Update the inventory.
            var slotIndex = inventory.GetItemSlotIndex(itemType);
            inventory.EquipItem(slotIndex);

        }

        /// <summary>
        /// Item is no longer equipped. 
        /// </summary>
        protected virtual void ItemDeactivated()
        {
            mGameObject.SetActive(false);
            //  Update the inventory.
            inventory.UnequipCurrentItem();
            //Debug.LogFormat("Setting {0} active as Deactivated", gameObject.name);
        }








    }

}
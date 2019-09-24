namespace CharacterController
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;

    using CharacterController.CharacterInventory;

    public class Item : MonoBehaviour
    {

        [Serializable] public class EquipItemEvent : UnityEvent<ItemType> { }
        //[Serializable] public class UnequipItemEvent : UnityEvent { }
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
        protected ItemSocketType socket = ItemSocketType.RightHand;
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


        [Header("Events")]
        public AnimationEventTrigger equipEvent = new AnimationEventTrigger("OnAnimatorEquipItem");
        public AnimationEventTrigger unequipEvent = new AnimationEventTrigger("OnAnimatorUnequipItem");

        [Space()]
        public EquipItemEvent OnEquipEvent = new EquipItemEvent();
        public UnityEvent OnUnequipEvent = new UnityEvent();


        public ItemType ItemType{ get { return itemType; } }

        public int SlotID { get { return slotID; } set { slotID = value; } }

        public ItemSocketType Socket { get { return socket; } }

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



        }

        protected virtual void OnDestroy()
        {

            if (character != null)
            {
                EventHandler.UnregisterEvent(character, EventIDs.OnAnimatorEquipItem, ItemActivated);
                EventHandler.UnregisterEvent(character, EventIDs.OnAnimatorUnequipItem, ItemDeactivated);
                //EventHandler.UnregisterEvent(character, EventIDs.OnAnimatorDropItem, ItemDeactivated);
                //EventHandler.UnregisterEvent(character, EventIDs.OnAnimatorPickupItem, ItemActivated);
            }

            if (OnEquipEvent.GetPersistentEventCount() > 0) OnEquipEvent.RemoveAllListeners();
            if (OnUnequipEvent.GetPersistentEventCount() > 0) OnUnequipEvent.RemoveAllListeners();
        }




        public virtual void Initialize( Inventory _inventory )
        {
            charLocomotion = _inventory.GetComponent<CharacterLocomotion>();
            character = _inventory.gameObject;
            inventory = _inventory;
            animator = charLocomotion.GetComponent<Animator>();

            mGameObject = gameObject;
            mTransform = transform;



            if (character != null) {
                EventHandler.RegisterEvent(character, EventIDs.OnAnimatorEquipItem, ItemActivated);
                EventHandler.RegisterEvent(character, EventIDs.OnAnimatorUnequipItem, ItemDeactivated);
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


            OnEquipEvent.AddListener(inventory.EquipItem);
            OnUnequipEvent.AddListener(inventory.UnequipCurrentItem);


        }


        /// <summary>
        /// Activations and deactivates the item.  Also places it at the holster if there is one.
        /// </summary>
        /// <param name="active"></param>
        public virtual void SetActive(bool active)
        {
            Debug.LogFormat("<b><color=red>{0}</color> is now {1}</b>", itemType.name, active);

            if (active) {
                //if (holsterTarget && slotID > -1)
                //{
                //    //  Parent the item to a item equip slot.
                //    Transform parent = inventory.GetItemSocket(slotID);
                //    if (parent != null) {
                //        mGameObject.transform.parent = parent;
                //        mGameObject.transform.localPosition = localSpawnPosition;
                //        mGameObject.transform.localEulerAngles = localSpawnRotation;
                //    }
                //}


                //  Show the item.
                mGameObject.SetActive(true);
            }
            else
            {
                if (holsterTarget){
                    //  Parent the object to the holster and reset position and rotation values.
                    mGameObject.transform.parent = holsterTarget;
                    mGameObject.transform.localPosition = Vector3.zero;
                    mGameObject.transform.localEulerAngles = Vector3.zero;
                }


                //  Deactivate the item.
                mGameObject.SetActive(false);

            }
        }


        /// <summary>
        /// Prepare item for use. 
        /// </summary>
        protected virtual void ItemActivated()
        {
            if (equipEvent.waitForAnimationEvent == false) {
                StartCoroutine(ActivateItem(SetActive, true));
            }

            Debug.LogFormat("{0} contains {1} events.", OnEquipEvent, OnEquipEvent.GetPersistentEventCount());
            ////  Fire off events.
            //OnEquipEvent.Invoke(itemType);
            //  Update the inventory.
            inventory.EquipItem(inventory.GetItemSlotIndex(itemType));
            SetActive(true);
        }

        /// <summary>
        /// Item is no longer equipped. 
        /// </summary>
        protected virtual void ItemDeactivated()
        {
            if (unequipEvent.waitForAnimationEvent == false) {
                StartCoroutine(ActivateItem(SetActive, false));
            }
            Debug.LogFormat("{0} contains {1} events.", OnUnequipEvent, OnUnequipEvent.GetPersistentEventCount());
            //  Fire off events.
            //OnEquipEvent.Invoke(active);
            //  Update the inventory.
            //Debug.LogFormat("Setting {0} active as Deactivated", gameObject.name);
            inventory.UnequipCurrentItem();
            SetActive(false);
        }





        private IEnumerator ActivateItem(Action<bool> setActive, bool active )
        {

            //float currentTime = 0;
            //float endDuration = active ? item.equipEvent.duration : item.unequipEvent.duration;
            //while (currentTime < endDuration)
            //{
            //    if (currentTime > 10) break;
            //    currentTime += Time.deltaTime;

            //    yield return null;
            //}

            float duration = active ? equipEvent.duration : unequipEvent.duration;
            Debug.LogFormat("Current time: {0}", Time.time);
            yield return new WaitForSeconds(duration);
            Debug.LogFormat("Finished WaitForSeconds. Time is: {0}", Time.time);
            setActive(active);
        }


    }

}
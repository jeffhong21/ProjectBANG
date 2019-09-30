using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

using CharacterController.CharacterInventory;

namespace CharacterController
{
    public class Item : MonoBehaviour
    {

        //
        // Fields
        //
        [SerializeField]
        protected ItemType m_itemType;


        [SerializeField]
        protected Transform holsterTarget;


        [Group("Hand IK")] [Tooltip("The ik position of the non dominant hand.")]      //  Currently not used.
        [SerializeField] protected Transform m_NonDominantHandIKTarget;
        [Group("Hand IK")] [Tooltip("The ik hint transform for non dominant hand.")] 
        [SerializeField] protected Transform m_NonDominantHandIKTargetHint;


        //[SerializeField]
        //protected Vector3 localSpawnPosition;
        //[SerializeField]
        //protected Vector3 localSpawnRotation;


        //[Header("Events")]
        //public AnimationEventTrigger equipEvent = new AnimationEventTrigger("OnAnimatorEquipItem");
        //public AnimationEventTrigger unequipEvent = new AnimationEventTrigger("OnAnimatorUnequipItem");


        //public EquipItemEvent OnEquipEvent = new EquipItemEvent();
        //public UnityEvent OnUnequipEvent = new UnityEvent();


        public ItemType ItemType{ get { return m_itemType; } }

        public int AnimatorItemID { get { return m_itemType.m_itemID; } }

        public int AnimatorMovementSetID { get { return m_itemType.m_movementSetID; } }







        [SerializeField, DisplayOnly]
        protected GameObject m_character;
        protected CharacterLocomotion m_controller;
        protected Inventory m_inventory;
        protected GameObject m_gameObject;
        protected Transform m_transform;


        //
        // Methods
        //
        protected virtual void Awake()
        {

        }


        protected virtual void OnDestroy()
        {

            if (m_character != null)
            {
                EventHandler.UnregisterEvent(m_character, EventIDs.OnAnimatorEquipItem, ItemActivated);
                EventHandler.UnregisterEvent(m_character, EventIDs.OnAnimatorUnequipItem, ItemDeactivated);
                //EventHandler.UnregisterEvent(m_character, EventIDs.OnAnimatorDropItem, ItemDeactivated);
                //EventHandler.UnregisterEvent(m_character, EventIDs.OnAnimatorPickupItem, ItemActivated);
            }

            //if (OnEquipEvent.GetPersistentEventCount() > 0) OnEquipEvent.RemoveAllListeners();
            //if (OnUnequipEvent.GetPersistentEventCount() > 0) OnUnequipEvent.RemoveAllListeners();
        }




        public virtual void Initialize(Inventory inventory )
        {
            m_controller = inventory.GetComponent<CharacterLocomotion>();
            m_character = inventory.gameObject;
            m_inventory = inventory;

            m_gameObject = gameObject;
            m_transform = transform;

            


            if (m_character != null) {
                EventHandler.RegisterEvent(m_character, EventIDs.OnAnimatorEquipItem, ItemActivated);
                EventHandler.RegisterEvent(m_character, EventIDs.OnAnimatorUnequipItem, ItemDeactivated);
                //EventHandler.RegisterEvent(m_character, EventIDs.OnAnimatorDropItem, ItemDeactivated);
                //EventHandler.RegisterEvent(m_character, EventIDs.OnAnimatorPickupItem, ItemActivated);
                //Debug.LogFormat("Registering event equip event for {0}", m_itemType.name);
            }


            if (m_NonDominantHandIKTarget == null) {
                m_NonDominantHandIKTarget = new GameObject("NonDominantHandIKTarget").transform;
                m_NonDominantHandIKTarget.parent = m_transform;
                m_NonDominantHandIKTarget.localPosition = Vector3.zero;
                m_NonDominantHandIKTarget.localEulerAngles = new Vector3(3, 50, 180);
            }


            //OnEquipEvent.AddListener(m_inventory.EquipItem);
            //OnUnequipEvent.AddListener(m_inventory.UnequipCurrentItem);


        }


        /// <summary>
        /// Activations and deactivates the item.  Also places it at the holster if there is one.
        /// </summary>
        /// <param name="active"></param>
        public virtual void SetActive(bool active)
        {
//            Debug.LogFormat("<b><color=red>{0}</color> is now {1}</b>", m_itemType.name, active);

            if (active) {
                //if (holsterTarget && slotID > -1)
                //{
                //    //  Parent the item to a item equip slot.
                //    Transform parent = m_inventory.GetItemSocket(slotID);
                //    if (parent != null) {
                //        m_gameObject.transform.parent = parent;
                //        m_gameObject.transform.localPosition = localSpawnPosition;
                //        m_gameObject.transform.localEulerAngles = localSpawnRotation;
                //    }
                //}


                //  Show the item.
                m_gameObject.SetActive(true);
            }
            else
            {
                if (holsterTarget){
                    //  Parent the object to the holster and reset position and rotation values.
                    m_gameObject.transform.parent = holsterTarget;
                    m_gameObject.transform.localPosition = Vector3.zero;
                    m_gameObject.transform.localEulerAngles = Vector3.zero;
                }


                //  Deactivate the item.
                m_gameObject.SetActive(false);

            }
        }


        /// <summary>
        /// Prepare item for use. 
        /// </summary>
        protected virtual void ItemActivated()
        {
            //if (equipEvent.waitForAnimationEvent == false) {
            //    StartCoroutine(ActivateItem(SetActive, true));
            //}

            //Debug.LogFormat("{0} contains {1} events.", OnEquipEvent, OnEquipEvent.GetPersistentEventCount());
            ////  Fire off events.
            //OnEquipEvent.Invoke(m_itemType);
            //  Update the m_inventory.
            m_inventory.EquipItem(m_inventory.GetItemSlotIndex(m_itemType));
            m_inventory.EquipItem(m_itemType);
            SetActive(true);
        }

        /// <summary>
        /// Item is no longer equipped. 
        /// </summary>
        protected virtual void ItemDeactivated()
        {
            //if (unequipEvent.waitForAnimationEvent == false) {
            //    StartCoroutine(ActivateItem(SetActive, false));
            //}
            //Debug.LogFormat("{0} contains {1} events.", OnUnequipEvent, OnUnequipEvent.GetPersistentEventCount());
            //  Fire off events.
            //OnEquipEvent.Invoke(active);
            //  Update the m_inventory.
            //Debug.LogFormat("Setting {0} active as Deactivated", gameObject.name);
            m_inventory.UnequipCurrentItem();
            SetActive(false);
        }





        //private IEnumerator ActivateItem(Action<bool> setActive, bool active )
        //{

        //    //float currentTime = 0;
        //    //float endDuration = active ? item.equipEvent.duration : item.unequipEvent.duration;
        //    //while (currentTime < endDuration)
        //    //{
        //    //    if (currentTime > 10) break;
        //    //    currentTime += Time.deltaTime;

        //    //    yield return null;
        //    //}

        //    float duration = active ? equipEvent.duration : unequipEvent.duration;
        //    Debug.LogFormat("Current time: {0}", Time.time);
        //    yield return new WaitForSeconds(duration);
        //    Debug.LogFormat("Finished WaitForSeconds. Time is: {0}", Time.time);
        //    setActive(active);
        //}


    }

}
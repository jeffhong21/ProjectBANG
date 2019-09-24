﻿namespace CharacterController
{
    /*  TODO: Add a set itemType count method.
     *
     *
     *
     *
     *
     * */
    using UnityEngine;
    using System;
    using System.Collections;

    using CharacterController.CharacterInventory;

    [DisallowMultipleComponent]
    public class Inventory : InventoryBase
    {
        [Serializable]
        public struct InventorySlot
        {
            public Item item;
            public int quantity;
            public bool isActive;
        }

        [SerializeField, DisplayOnly]
        protected Item equppiedItem;
        [SerializeField, DisplayOnly]
        protected Item previouslyEquippedItem;
        [SerializeField, DisplayOnly]
        protected int nextActiveSlot;


        [SerializeField, DisplayOnly]
        protected InventorySlot[] m_inventorySlot;

        public Item EquippedItem {
            get { return equppiedItem; }
        }

        protected int NextActiveSlot {
            get {
                for (int i = 0; i < m_inventorySlot.Length + 0; i++) {
                    if (m_inventorySlot[i].item == null) {
                        nextActiveSlot = i;
                        return nextActiveSlot;
                    }
                        
                }
                return -1;
            }
        }




        #region Sockets

        [SerializeField] protected ItemEquipSocket m_rightHandSocket;
        #endregion


        [Header("----- Debug -----")]
        [SerializeField] private bool m_debug;




        private void Start()
        {
            m_inventorySlot = new InventorySlot[SlotCount];
            EventHandler.RegisterEvent<ItemAction, bool>(m_gameObject, EventIDs.OnItemActionActive, OnItemActionActive);

            //  Get item sockets.
            SetSockets(m_animator);

            //  Parent Items to the correct socket.
            GetComponentsInChildren<Item>(true, m_AllItems);
            for (int i = 0; i < m_AllItems.Count; i++)
            {
                //  Add the item to the inventory.
                AddItem(m_AllItems[i], false);
                //  Move Item to the correct socket.
                m_AllItems[i].transform.SetParent(m_rightHandSocket.transform);
            }

            //  Load default items.
            LoadDefaultLoadout();
        }



        private void OnDestroy()
        {
            EventHandler.UnregisterEvent<ItemAction, bool>(m_gameObject, EventIDs.OnItemActionActive, OnItemActionActive);
        }


        private void OnValidate()
        {
            if (m_animator == null) m_animator = GetComponent<Animator>();

            if (m_animator != null) SetSockets(m_animator);
        }





        protected void SetSockets(Animator animator)
        {
            if(m_rightHandSocket == null) {
                var handTransform = animator.GetBoneTransform(HumanBodyBones.RightHand);
                var socket = handTransform.GetComponentInChildren<ItemEquipSocket>(true);

                if(socket == null) {
                    var go = new GameObject("R Hand Socket", typeof(ItemEquipSocket));
                    go.transform.parent = handTransform;
                    go.transform.localPosition = Vector3.zero;
                    socket = go.GetComponent<ItemEquipSocket>();
                }
                m_rightHandSocket = socket;
            }
            m_rightHandSocket.Init(animator);
        }





        /// <summary>
        /// Loads up each itemType in thje DefaultLoadout.
        /// </summary>
        public void LoadDefaultLoadout()
        {
            if (m_DefaultLoadout != null) {
                for (int index = 0; index < m_DefaultLoadout.Length; index++) {
                    var itemType = m_DefaultLoadout[index].ItemType;
                    var amount = m_DefaultLoadout[index].Amount;
                    var equipItem = m_DefaultLoadout[index].Equip;
                    PickupItemType(itemType, amount, true, equipItem, false);
                }
            }

            if (m_debug) {
                for (int i = 0; i < m_inventorySlot.Length; i++) {
                    var itemSlot = m_inventorySlot[i];
                    Debug.Log(itemSlot.item != null ? itemSlot.item.name + " | equipped: (" + itemSlot.isActive + ") " : "[<b>Slot (" + (int)(i + 1) + "</b>] is empty>");
                }
            }

        }


        /// <summary>
        /// Adds the item to the inventory and adds the specified amount of ItemType.
        /// It should be assumed that the itemType has already been mapped internally.
        /// </summary>
        /// <param name="itemType"> The ItemType to add. </param>
        /// <param name="count"> The amount of itemType to add. </param>
        /// <param name="immediatePickup"> Should the item be picked up immediately. If false, item will be added with an animation. </param>
        /// <param name="forceEquip"> Should the item be forced to equip. </param>
        /// <param name="notifyOnPickup"> Should other objects be notified the itemType was pickedup? </param>
        /// <returns> Returns true if the ItemType was pickedup.</returns>
        public override bool PickupItemType( ItemType itemType, float count, bool immediatePickup, bool forceEquip, bool notifyOnPickup = true )
        {
            if (!enabled || itemType == null || count <= 0)
                return false;

            //  Add the itemType count.
            bool pickedUpItem = InternalPickupItemType(itemType, count);

            //  Notify those interested that an item has been picked up.
            if (notifyOnPickup) {

            }

            if (pickedUpItem)
            {
                //  Add item to invetory slot.
                Item item;
                if (m_ItemTypeItemMap.TryGetValue(itemType, out item)) {
                    if (NextActiveSlot >= 0) {
                        m_inventorySlot[NextActiveSlot].item = item;
                    }
                }

                if (forceEquip) {
                    int slotIndex = GetItemSlotIndex(itemType);
                    EquipItem(slotIndex);
                }
            }

            return pickedUpItem;
        }



        /// <summary>
        /// Removes ItemType from the inventory and updates the inventory internals.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns>Returns the item that was removed.</returns>
        public override Item RemoveItemType( ItemType itemType, int slotIndex )
        {
            Item item = InternalRemoveItemType(itemType);

            if(item != null) {
                //  Remove the item from the inventory slot.
                if (m_inventorySlot[slotIndex].item != null && m_inventorySlot[slotIndex].item.ItemType == itemType) {
                    m_inventorySlot[slotIndex].item = null;
                }
            }
            return item;
        }



        /// <summary>
        /// Add the item to the inventory.  Does not add the actual ItemType.  PickupItem does that.
        /// </summary>
        /// <param name="item">Item to add to the inventory.</param>
        /// <returns>True if the item was added to the inventory.</returns>
        public override bool AddItem( Item item, bool immediatelyEquip )
        {
            if (item.ItemType == null) {
                Debug.LogError("Error: Item " + item + "has no ItemType");
                return false;
            }

            //  Check if inventory already contains the items type.  There should only be 1 item type.
            if (m_ItemTypeItemMap.ContainsKey(item.ItemType))
                return false;

            //  The itemType doesn't exist in the inventory.
            m_ItemTypeItemMap.Add(item.ItemType, item);

            //  Item can be added without being pickedup up yet.  Add to the ItemTypeCount
            if (!m_ItemTypeCount.ContainsKey(item.ItemType))
                m_ItemTypeCount.Add(item.ItemType, 0);



            //  Add the item to the list of items.
            m_AllItems.Add(item);
            item.Initialize(this);

            InternalAddItem(item, immediatelyEquip);

            return true;
        }



        /// <summary>
        /// GEt item from the inventory.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override Item GetItem( int index ) { return m_inventorySlot[index].item; }


        /// <summary>
        /// Get the item types slot index.  If return is -1, than that means there is no coresponding item and itemType is a consumable.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public int GetItemSlotIndex( ItemType itemType )
        {
            for (int index = 0; index < m_inventorySlot.Length; index++) {
                if (GetItem(index).ItemType == itemType) {
                    return index;
                }
            }
            return -1;
        }


        /// <summary>
        /// Is the item in the inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool HasItem( Item item )
        {
            for (int i = 0; i < m_inventorySlot.Length; i++) {
                if (m_inventorySlot[i].item == null) { continue; }
                if (m_inventorySlot[i].item != item) { continue; }
                return true;
            }
            return false;
            //return item != null && GetItem(GetItemSlotIndex(item.ItemType), item.ItemType) != null;
        }



        /// <summary>
        /// Equip the item at slot index.  We can only transition from Unequip => Equip or Unequip only.
        /// We cannot transition from Equip => Unequip
        /// </summary>
        /// <param name="slotIndex"></param>
        /// <returns></returns>
        public Item EquipItem( int slotIndex )
        {
            if (slotIndex < 0) {
                IsSwitching = false;
                return null;
            }

            Item item = m_inventorySlot[slotIndex].item;

            if (item != null)
            {
                previouslyEquippedItem = equppiedItem;
                equppiedItem = item;

                //  Execute the equip event.
                InternalEquipItem(item);
                //  Set is switching to off.
                IsSwitching = false;
            }
            else if(equppiedItem == item) {
                UnequipCurrentItem();
            }

            return item;
        }


        public void EquipItem( ItemType itemType )
        {
            EquipItem(GetItemSlotIndex(itemType));
        }

        /// <summary>
        /// Equips the next equipable item in the slot.
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        public Item EquipNextItem(bool next)
        {
            int index = 0;
            if(EquippedItem != null) index = GetItemSlotIndex(EquippedItem.ItemType);
            
            if (index < 0 || index > SlotCount) return null;
            //Debug.Log("Currently equipped: " +  EquippedItem.ItemType + " | SlotIndex: " + index);

            if (next) {
                if (index == SlotCount - 1)
                    index = 0;
                else index++;
            } else {
                if (index == 0)
                    index = SlotCount - 1;
                else index--;
            }

            Item item = EquipItem(index);
            return item;
        }





        public void UnequipCurrentItem()
        {
            if (equppiedItem != null) {
                IsSwitching = true;
                //  Execute the equip event.
                InternalUnequipCurrentItem(previouslyEquippedItem);
            }

            previouslyEquippedItem = equppiedItem;
            equppiedItem = null;


        }




        public override void UseItem( ItemType itemType, float count )
        {
            InternalUseItem(itemType, count);

            float existingAmount;
            if (!m_ItemTypeCount.TryGetValue(itemType, out existingAmount)) {
                Debug.LogError("Error: Trying top use " + itemType.name + " when ItemType does not exist.");
                return;
            }

            m_ItemTypeCount[itemType] = Mathf.Clamp(existingAmount + count, 0, itemType.Capacity);
        }


        public override void Reload( ItemType itemType, float amount )
        {
            throw new NotImplementedException();
        }



        protected void UpdateInventorySlots( int slot1, int slot2 )
        {
            var tempSlot = m_inventorySlot[slot2];
            m_inventorySlot[slot2] = m_inventorySlot[slot1];
            m_inventorySlot[slot1] = tempSlot;
        }





        private void OnItemActionActive(ItemAction action, bool activated )
        {
            if(activated)
            {

            } 
            else
            {
                if (IsSwitching) {
                    IsSwitching = false;
                }
            }
        }





    }

}

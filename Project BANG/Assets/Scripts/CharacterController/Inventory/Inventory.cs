namespace CharacterController
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

    [DisallowMultipleComponent]
    public class Inventory : InventoryBase
    {

        [SerializeField, DisplayOnly]
        protected Item currentlyEquippedItem;
        [SerializeField, DisplayOnly]
        protected Item previouslyEquippedItem;
        [SerializeField, DisplayOnly]
        protected int nextActiveSlot;


        [SerializeField, DisplayOnly]
        protected InventorySlot[] m_InventorySlots;

        public Item CurrentlyEquippedItem {
            get { return currentlyEquippedItem; }
        }

        protected int NextActiveSlot {
            get {
                for (int i = 0; i < m_InventorySlots.Length + 0; i++) {
                    if (m_InventorySlots[i].item == null) {
                        nextActiveSlot = i;
                        return nextActiveSlot;
                    }
                        
                }
                return -1;
            }
        }








        protected virtual void Awake()
        {
            m_GameObject = gameObject;
            m_Animator = GetComponent<Animator>();


            itemEquipSlots = GetComponentsInChildren<ItemEquipSlot>(true);

            GetComponentsInChildren<Item>(true, m_AllItems);
            for (int i = 0; i < m_AllItems.Count; i++) {
                AddItem(m_AllItems[i], false);
                if (m_AllItems[i].SlotID > -1)
                    m_AllItems[i].transform.parent = GetItemEquipSlot(m_AllItems[i].SlotID);

            }

            m_InventorySlots = new InventorySlot[SlotCount];

            EventHandler.RegisterEvent<ItemAction, bool>(m_GameObject, EventIDs.OnItemActionActive, OnItemActionActive);
        
        }


        private void OnDestroy()
        {
            EventHandler.UnregisterEvent<ItemAction, bool>(m_GameObject, EventIDs.OnItemActionActive, OnItemActionActive);

        }






        private void Start()
        {
            //  REgister for OnDeath and OnRespawn.

            //  Load default items.
            LoadDefaultLoadout();
        }


        private void OnValidate()
        {
            m_Animator = GetComponent<Animator>();
            itemEquipSlots = GetComponentsInChildren<ItemEquipSlot>(true);
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
            bool pickedupItem = InternalPickupItemType(itemType, count);

            //  Notify those interested that an item has been picked up.
            if (notifyOnPickup) {

            }

            if (pickedupItem)
            {
                //  Add item to invetory slot.
                Item item;
                if (m_ItemTypeItemMap.TryGetValue(itemType, out item)) {
                    if (NextActiveSlot >= 0) {
                        m_InventorySlots[NextActiveSlot].item = item;
                    }
                }

                if (forceEquip) {
                    int slotIndex = GetItemSlotIndex(itemType);
                    EquipItem(slotIndex);
                }
            }

            return pickedupItem;
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
                if (m_InventorySlots[slotIndex].item != null && m_InventorySlots[slotIndex].item.ItemType == itemType) {
                    m_InventorySlots[slotIndex].item = null;
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
        public override Item GetItem( int index ) { return m_InventorySlots[index].item; }


        /// <summary>
        /// Get the item types slot index.  If return is -1, than that means there is no coresponding item and itemType is a consumable.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public int GetItemSlotIndex( ItemType itemType )
        {
            for (int index = 0; index < m_InventorySlots.Length; index++) {
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
            for (int i = 0; i < m_InventorySlots.Length; i++) {
                if (m_InventorySlots[i].item == null) { continue; }
                if (m_InventorySlots[i].item != item) { continue; }
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

            Item item = m_InventorySlots[slotIndex].item;
            if (item != null)
            {
                previouslyEquippedItem = currentlyEquippedItem;
                currentlyEquippedItem = item;

                //  Execute the equip event.
                InternalEquipItem(item);
            }

            IsSwitching = false;
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
            if(CurrentlyEquippedItem != null) index = GetItemSlotIndex(CurrentlyEquippedItem.ItemType);
            
            if (index < 0 || index > SlotCount) return null;
            //Debug.Log("Currently equipped: " +  CurrentlyEquippedItem.ItemType + " | SlotIndex: " + index);

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
            if (currentlyEquippedItem != null) {
                IsSwitching = true;
                //  Execute the equip event.
                InternalUnequipCurrentItem(previouslyEquippedItem);
            }

            previouslyEquippedItem = currentlyEquippedItem;
            currentlyEquippedItem = null;


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
            var tempSlot = m_InventorySlots[slot2];
            m_InventorySlots[slot2] = m_InventorySlots[slot1];
            m_InventorySlots[slot1] = tempSlot;
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

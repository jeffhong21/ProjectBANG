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
    using System.Collections.Generic;


    public class Inventory : InventoryBase
    {

        [SerializeField, DisplayOnly]
        protected Item m_EquipedItem;
        [SerializeField, DisplayOnly]
        protected Item m_LastEquipedItem;
        [SerializeField, DisplayOnly]
        protected int m_NextActiveSlot;


        public Item CurrentlyEquippedItem {
            get { return m_EquipedItem; }
        }

        protected int NextActiveSlot {
            get {
                for (int i = 0; i < m_InventorySlots.Length; i++) {
                    if (m_InventorySlots[i].item == null) {
                        m_NextActiveSlot = i;
                        return m_NextActiveSlot;
                    }
                        
                }
                return -1;
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
        /// Equip the item at slot index.
        /// </summary>
        /// <param name="slotIndex"></param>
        /// <returns></returns>
        public Item EquipItem( int slotIndex )
        {
            if (slotIndex < 0) return null;

            Item item = m_InventorySlots[slotIndex].item;
            if (item != null) {
                m_LastEquipedItem = m_EquipedItem;
                m_EquipedItem = item;

                if(m_LastEquipedItem != null) m_LastEquipedItem.SetActive(false);
                m_EquipedItem.SetActive(true);

                //  Execute the equip event.
                InternalEquipItem(item);
            }
            return item;
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
            if (m_EquipedItem != null)
            {
                m_EquipedItem.SetActive(false);
                m_LastEquipedItem = m_EquipedItem;
                m_EquipedItem = null;


                //  Execute the equip event.
                InternalUnequipCurrentItem(m_LastEquipedItem);

            }
        }




        protected void UpdateInventorySlots( int slot1, int slot2 )
        {
            var tempSlot = m_InventorySlots[slot2];
            m_InventorySlots[slot2] = m_InventorySlots[slot1];
            m_InventorySlots[slot1] = tempSlot;
        }






    }

}

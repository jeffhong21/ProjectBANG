namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;


    [DisallowMultipleComponent]
    public class Inventory : MonoBehaviour
    {
        #region Events
        //public delegate void OnInventoryUseItem(ItemType itemType, float remaining);
        //public delegate void OnInventoryPickupItem(Item item, float count, bool immediatePickup, bool forceEquip);
        //public delegate void OnInventoryEquipItem(Item item);
        //public delegate void OnInventoryUnequipItem(Item item);
        //public delegate void OnInventoryAddItem(Item item);
        //public delegate void OnInventoryRemoveItem(Item item, int slotID);

        //public event OnInventoryUseItem InventoryUseItem = delegate { };
        //public event OnInventoryPickupItem InventoryPickupItem = delegate { };
        //public event OnInventoryEquipItem InventoryEquipItem = delegate { };
        //public event OnInventoryUnequipItem InventoryUnequipItem = delegate { };
        //public event OnInventoryAddItem InventoryAddItem = delegate { };
        //public event OnInventoryRemoveItem InventoryRemoveItem = delegate{ };
        #endregion



        protected List<Item> m_AllItems = new List<Item>();
        protected List<ItemType> m_AllItemTypes = new List<ItemType>();

        [Tooltip("The ItemTypes that the character starts out with.")]
        [SerializeField] protected ItemAmount[] m_DefaultLoadout = new ItemAmount[0];
        [Tooltip("The max amount of inventory slots.")]
        [SerializeField] protected int m_SlotCount = 6;


        protected Dictionary<ItemType, Item> m_ItemTypeItemMap = new Dictionary<ItemType, Item>();
        protected Dictionary<ItemType, float> m_ItemTypeCount = new Dictionary<ItemType, float>();

        [SerializeField, DisplayOnly]
        protected Item[] m_ActiveItems;
        protected Item m_EquipedItem;
        protected Item m_LastEquipedItem;

        [SerializeField]
        protected Transform m_LeftItemSlot;
        [SerializeField]
        protected Transform m_RightItemSlot;

        protected Animator m_Animator;
        protected GameObject m_GameObject;


        //
        //  Properties
        //  
        public int SlotCount { get { return m_SlotCount; } }

        public bool IsSwitching { get; protected set; }





        protected int NextActiveSlot {
            get { return Array.IndexOf(m_ActiveItems, null); }
        }



        //
        // Methods
        //
        protected virtual void Awake()
		{
            m_GameObject = gameObject;
            m_Animator = GetComponent<Animator>();
            m_LeftItemSlot = m_Animator.GetBoneTransform(HumanBodyBones.LeftHand);
            m_RightItemSlot = m_Animator.GetBoneTransform(HumanBodyBones.RightHand);


            m_ActiveItems = new Item[SlotCount];
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
            if (m_LeftItemSlot == null) m_LeftItemSlot = m_Animator.GetBoneTransform(HumanBodyBones.LeftHand);
            if (m_RightItemSlot == null) m_RightItemSlot = m_Animator.GetBoneTransform(HumanBodyBones.RightHand);
        }



        /// <summary>
        /// Loads up each itemType in thje DefaultLoadout.
        /// </summary>
		public void LoadDefaultLoadout()
        {
            if(m_DefaultLoadout != null) {
                for (int i = 0; i < m_DefaultLoadout.Length; i++) {
                    PickupItemType(
                        m_DefaultLoadout[i].ItemType,
                        m_DefaultLoadout[i].Amount,
                        true, i == 0 || m_DefaultLoadout[i].Equip,
                        false
                        );
                }
            }

        }


        /// <summary>
        /// Adds the specified amount of ItemType to the inventory.
        /// </summary>
        /// <param name="itemType"> The ItemType to add. </param>
        /// <param name="count"> The amount of itemType to add. </param>
        /// <param name="immediatePickup"> Should the item be picked up immediately. If false, item will be added with an animation. </param>
        /// <param name="forceEquip"> Should the item be forced to equip. </param>
        /// <param name="notifyOnPickup"> Should other objects be notified the itemType was pickedup? </param>
        /// <returns> Returns true if the ItemType was pickedup.</returns>
        public bool PickupItemType(ItemType itemType, float count, bool immediatePickup, bool forceEquip, bool notifyOnPickup = true)
        {
            if (!enabled || itemType == null || count <= 0)
                return false;

            bool pickedupItem = PickupItemType(itemType, count);

            int slotIndex = GetItemSlotIndex(itemType);
            //  Notify those interested that an item has been picked up.
            if (notifyOnPickup) {

            }
            
            if(slotIndex != -1) {
                var item = GetItem(slotIndex, itemType);
                if (!m_AllItems.Contains(item)) {
                    m_AllItems.Add(item);
                }
            }
            if (!m_AllItemTypes.Contains(itemType)) {
                m_AllItemTypes.Add(itemType);
            }

            return pickedupItem;
        }


        /// <summary>
        /// Adds the specified amount of ItemType to the inventory.
        /// </summary>
        /// <param name="itemType"> The ItemType to add. </param>
        /// <param name="count"> The amount of itemType to add. </param>
        /// <returns> Returns true if the ItemType was pickedup.</returns>
        protected bool PickupItemType(ItemType itemType, float count)
        {
            float existingAmount;
            if (!m_ItemTypeCount.TryGetValue(itemType, out existingAmount)) {
                m_ItemTypeCount.Add(itemType, Mathf.Min(count, itemType.Capacity));
            } else {
                //  The itemType will nmot be pickedup if at capacity.
                if (existingAmount >= itemType.Capacity)
                    return false;
                m_ItemTypeCount[itemType] = Mathf.Clamp(existingAmount + count, 0, itemType.Capacity);

            }

            return true;
        }

        //TODO:  Need to implement ItemTypePickup notifications.
        /// <summary>
        /// ItemType has been picked up.  Notify anyone interested.
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="count"></param>
        /// <param name="immediatePickup"></param>
        /// <param name="forceEquip"></param>
        protected void ItemTypePickup( ItemType itemType, float count, bool immediatePickup, bool forceEquip )
        {

        }


        /// <summary>
        /// Removes ItemType from the inventopry.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public Item RemoveItemType( ItemType itemType, int slotIndex)
        {
            float existingAmount;
            if (!m_ItemTypeCount.TryGetValue(itemType, out existingAmount))
                return null;

            Item item;
            if (!m_ItemTypeItemMap.TryGetValue(itemType, out item)) {
                //  Remove the item.  ItemType does not correspond to the item, so it should be removed completely.
                m_ItemTypeCount[itemType] = 0;
                return null;
            }

            //  Remove a single item.
            m_ItemTypeCount[itemType] = Mathf.Clamp(existingAmount - 1, 0, itemType.Capacity);

            //  Remove the item from the inventory slot.
            if(m_ActiveItems[slotIndex] != null && m_ActiveItems[slotIndex].ItemType == itemType) {
                m_ActiveItems[slotIndex] = null;
            }



            return item;
        }


        /// <summary>
        /// Add the item to the inventory.  Does not add the actual ItemType.  PickupItem does that.
        /// </summary>
        /// <param name="item">Item to add to the inventory.</param>
        /// <returns>True if the item was added to the inventory.</returns>
        public bool AddItem( Item item, bool immediatelyEquip )
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
            //  Notify others that an item has been added to the inventory.
            EventHandler.ExecuteEvent(m_GameObject, EventIDs.OnInventoryAddItem, item);

            //  Pickup event should be called when the count is greater than zero.
            //  This allows the item Type to be pickedup before the item has been added.
            float count = 0;
            if ((count + GetItemTypeCount(item.ItemType)) > 0) {
                //item.Pickup();
                EventHandler.ExecuteEvent(m_GameObject, EventIDs.OnInventoryPickupItem, item, count, immediatelyEquip, false);
            }


            return true;
        }



        /// <summary>
        /// Get item from iventory list.
        /// </summary>
        /// <param name="slotID"></param>
        /// <returns></returns>
        public Item GetItem(int slotID, ItemType itemType ) {
            if(m_ActiveItems[slotID] != null && m_ActiveItems[slotID].ItemType == itemType) {
                return m_ActiveItems[slotID];
            }
            return null;
        }


        /// <summary>
        /// Get the item from the inventory.
        /// </summary>
        /// <param name="slotID"></param>
        /// <returns></returns>
        public Item GetItem( int slotID ){ return m_ActiveItems[slotID]; }


        /// <summary>
        /// Get the amount of ItemType from the inventory.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public float GetItemTypeCount(ItemType itemType)
        {
            float count;
            m_ItemTypeCount.TryGetValue(itemType, out count);
            return count;
        }


        /// <summary>
        /// Get the item types slot index.  If return is -1, than that means there is no coresponding item and itemType is a consumable.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public int GetItemSlotIndex(ItemType itemType )
        {
            for (int index = 0; index < m_ActiveItems.Length; index++) {
                if(GetItem(index, itemType) != null) {
                    return index;
                }
            }
            return -1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool HasItem(Item item)
        {
            for (int i = 0; i < m_ActiveItems.Length; i++) {
                if (m_ActiveItems[i] == null) { continue; }
                if (m_ActiveItems[i] != item) { continue; }
                return true;
            }
            return false;
            //return item != null && GetItem(GetItemSlotIndex(item.ItemType), item.ItemType) != null;
        }


        /// <summary>
        /// Equips the itemType.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public Item EquipItem(ItemType itemType)
        {
            Item item;
            if(m_ItemTypeItemMap.TryGetValue(itemType, out item)){
                Debug.LogError("Error: Unable to equip item with ItemType " + itemType + ".  This itemType has not been added to the inventory.");
                return null;
            }

            //  Inventory at slot X is now item.
            //m_ActiveItems[slotID]
            m_LastEquipedItem = m_EquipedItem;
            m_EquipedItem = item;
            return item;
        }


        public void UnequipCurrentItem()
        {
            if(m_EquipedItem != null) {
                m_LastEquipedItem = m_EquipedItem;
                m_EquipedItem = null;
            }
        }

 
        /// <summary>
        /// Calls the itemTypes item use function.
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="count"></param>
        public void UseItem(ItemType itemType, float count)
        {
            float existingAmount;
            if (!m_ItemTypeCount.TryGetValue(itemType, out existingAmount))
            {
                Debug.LogError("Error: Trying top use " + itemType.name + " when ItemType does not exist.");
                return;
            }

            m_ItemTypeCount[itemType] = Mathf.Clamp(existingAmount + count, 0, itemType.Capacity);
        }



        /// <summary>
        /// Returns list of all items in inventory.  Used by the editor.
        /// </summary>
        /// <returns></returns>
        public List<Item> GetAllItems() { return m_AllItems; }


        /// <summary>
        /// Returns list of all items in inventory.  Used by the editor.
        /// </summary>
        /// <returns></returns>
        public List<ItemType> GetAllItemTypes() { return m_AllItemTypes; }










        [Serializable]
        public class EquipItemSlots
        {
            [SerializeField] protected Transform m_LeftHandSlot;
            [SerializeField] protected Transform m_RightHandSlot;

            public Transform LeftHandSlot
            {
                get { return m_LeftHandSlot; }
                set { m_LeftHandSlot = value; }
            }

            public Transform RightHandSlot
            {
                get { return m_RightHandSlot; }
                set { m_RightHandSlot = value; }
            }

        }



        [Serializable]
        public class ItemAmount
        {
            [SerializeField] protected ItemType m_Item;
            [SerializeField] protected int m_Amount = 1;
            [SerializeField] protected bool m_Equip = true;


            public ItemType ItemType{
                get { return m_Item; }
                private set { m_Item = value; }
            }

            public int Amount{
                get { return Mathf.Clamp(m_Amount, 0, m_Item.Capacity); }
                set { m_Amount = Mathf.Clamp(value, 0, m_Item.Capacity); }
            }


            public bool Equip{
                get{ return m_Equip; }
                private set { m_Equip = value; }
            }


            public ItemAmount(ItemType itemType, int amount){
                Initialize(itemType, amount);
            }

            public void Initialize(ItemType itemType, int amount){
                ItemType = itemType;
                Amount = amount;
            }
        }
	}

}

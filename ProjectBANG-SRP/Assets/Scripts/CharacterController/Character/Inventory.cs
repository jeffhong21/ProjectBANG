namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;


    [DisallowMultipleComponent]
    public class Inventory : MonoBehaviour
    {
        public delegate void OnInventoryUseItem(ItemType itemType, float remaining);
        public delegate void OnInventoryPickupItem(Item item, float count, bool immediatePickup, bool forceEquip);
        public delegate void OnInventoryEquipItem(Item item);
        public delegate void OnInventoryUnequipItem(Item item);
        public delegate void OnInventoryAddItem(Item item);
        public delegate void OnInventoryRemoveItem(Item item, int slotID);

        public event OnInventoryUseItem onInventoryUseItem = delegate { };
        public event OnInventoryPickupItem InventoryPickupItem = delegate { };
        public event OnInventoryEquipItem InventoryEquipItem = delegate { };
        public event OnInventoryUnequipItem InventoryUnequipItem = delegate { };
        public event OnInventoryAddItem InventoryAddItem = delegate { };
        public event OnInventoryRemoveItem InventoryRemoveItem = delegate{ };




        protected List<Item> m_AllItems = new List<Item>();
        protected List<ItemType> m_AllItemTypes = new List<ItemType>();
        [SerializeField]
        protected ItemAmount[] m_DefaultLoadout = new ItemAmount[0];
        [SerializeField]
        protected int m_SlotCount = 6;

        protected Dictionary<ItemType, Item> m_ItemTypeItemMap = new Dictionary<ItemType, Item>();
        protected Dictionary<ItemType, float> m_ItemTypeCount = new Dictionary<ItemType, float>();
        [SerializeField, DisplayOnly]
        protected Item[] m_InventorySlots;
        //protected Item m_LastActiveItem;

        [SerializeField]
        protected Transform m_LeftItemSlot;
        [SerializeField]
        protected Transform m_RightItemSlot;

        protected Animator m_Animator;
        protected GameObject m_GameObject;



        public int SlotCount
        {
            get { return m_SlotCount; }
        }

        public Dictionary<ItemType, float> ItemTypeCount { get { return m_ItemTypeCount; } }

        //
        // Methods
        //
		private void Awake()
		{
            m_GameObject = gameObject;
            m_Animator = GetComponent<Animator>();
            m_LeftItemSlot = m_Animator.GetBoneTransform(HumanBodyBones.LeftHand);
            m_RightItemSlot = m_Animator.GetBoneTransform(HumanBodyBones.RightHand);


            m_InventorySlots = new Item[SlotCount];
        }


		private void Start()
		{
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
        /// 
        /// </summary>
		public void LoadDefaultLoadout()
        {
            for (int i = 0; i < m_DefaultLoadout.Length; i++){
                ItemAmount loadoutItem = m_DefaultLoadout[i];

                PickupItemType(loadoutItem.ItemType, loadoutItem.Amount, true, i == 0 || loadoutItem.Equip, false);
            }
        }


        /// <summary>
        /// Add the item to the inventory.  Does not add the actual ItemType.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool AddItem(Item item)
        {
            if(item.ItemType == null){
                Debug.LogError("Error: Item " + item + "has no ItemType");
                return false;
            }


            if (m_ItemTypeItemMap.ContainsKey(item.ItemType))
                return false;

            m_ItemTypeItemMap.Add(item.ItemType, item);

            if (!m_ItemTypeCount.ContainsKey(item.ItemType))
                m_ItemTypeCount.Add(item.ItemType, 0);

            m_AllItems.Add(item);
            //  Execute OnInvetoryAdd
            EventHandler.ExecuteEvent(m_GameObject, EventIDs.OnInventoryAdd, item);
            //  Execute OnInventoryPickupItem if itemType > 0;
            float count = 0;
            if((count + GetItemTypeCount(item.ItemType)) > 0)
            {
                //item.Pickup();
            }


            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool HasItem(Item item) { return item != null && GetItem(item.SlotID) != null; }



        public Item EquipItem(ItemType itemType)
        {
            Item item;
            if(m_ItemTypeItemMap.TryGetValue(itemType, out item)){
                Debug.LogError("Error: Unable to equip item with ItemType " + itemType + ".  This itemType has not been added to the inventory.");
                return null;
            }

            //  Inventory at slot X is now item.
            return item;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemType"> The ItemType to add. </param>
        /// <param name="count"> The amount of itemType to add. </param>
        /// <param name="immediatePickup"> Should the item be picked up immediately </param>
        /// <param name="forceEquip"> Should the item be forced to equip. </param>
        /// <param name="notifyOnPickup"> Should other objects be notified the itemType was pickedup? </param>
        /// <returns> Returns true if the ItemType was pickedup.</returns>
        public bool PickupItemType(ItemType itemType, float count, bool immediatePickup, bool forceEquip, bool notifyOnPickup)
        {
            if (!enabled || itemType == null || count <= 0)
                return false;

            float existingAmount = 0;
            if(!m_ItemTypeCount.TryGetValue(itemType, out existingAmount))
            {
                m_ItemTypeCount.Add(itemType, Mathf.Min(count, itemType.Capacity));
            }
            else
            {
                //  Already at capacity.
                if (existingAmount >= itemType.Capacity)
                    return false;
            }
            m_ItemTypeCount[itemType] = Mathf.Clamp(existingAmount + count, 0, itemType.Capacity);

            return true;
        }



        public Item GetItem(int slotID)
        {
            return m_InventorySlots[slotID];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public float GetItemTypeCount(ItemType itemType)
        {
            float count = 0;
            m_ItemTypeCount.TryGetValue(itemType, out count);
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="count"></param>
        public void UseItem(ItemType itemType, float count)
        {
            float existingAmount = 0;
            if (!m_ItemTypeCount.TryGetValue(itemType, out existingAmount))
            {
                Debug.LogError("Error: Trying top use " + itemType.name + " when ItemType does not exist.");
                return;
            }

            m_ItemTypeCount[itemType] = Mathf.Clamp(existingAmount + count, 0, itemType.Capacity);
        }

        /// <summary>
        /// Removes ItemType from the inventopry.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public Item RemoveItemType(ItemType itemType)
        {
            float existingAmount = 0;
            if (!m_ItemTypeCount.TryGetValue(itemType, out existingAmount))
                return null;

            Item item;
            if (!m_ItemTypeItemMap.TryGetValue(itemType, out item))
            {
                //  Remove the item.  ItemType does not correspond to the item.
                m_ItemTypeCount[itemType] = 0;
                return null;
            }

            //  MISSING



            return item;
        }














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
//namespace CharacterController
//{
//    using UnityEngine;
//    using System;
//    using System.Collections.Generic;
//    using JH_Utils;


//    public class Inventory_Original : MonoBehaviour
//    {
//        [Serializable]
//        public class ItemAmount
//        {
//            [SerializeField]
//            protected ItemType m_ItemType;
//            [SerializeField, HideInInspector]
//            protected Item m_Item;
//            [SerializeField]
//            protected int m_Amount = 1;
//            [SerializeField, HideInInspector]
//            protected int m_Capacity;
//            [SerializeField]
//            protected bool m_Equip = true;


//            public ItemType ItemType
//            {
//                get { return m_ItemType; }
//            }

//            public Item Item
//            {
//                get { return m_Item; }
//                set { m_Item = value; }
//            }

//            public int Amount
//            {
//                get
//                {
//                    if (m_Amount < 0)
//                        m_Amount = 0;
//                    return m_Amount;
//                }
//                set
//                {
//                    var total = m_Amount + value;
//                    var capacity = m_ItemType.GetCapacity();
//                    if (total > capacity)
//                        value = total - capacity;
//                    m_Amount = value;
//                }
//            }

//            public int Capacity
//            {
//                get { return m_ItemType.GetCapacity(); }
//            }

//            public bool Equip
//            {
//                get
//                {
//                    if (m_ItemType.GetType() == typeof(ConsumableItem))
//                        m_Equip = false;
//                    return m_Equip;
//                }
//            }


//            public ItemAmount()
//            {
//            }

//            public ItemAmount(ItemType itemType, int amount)
//            {
//                m_ItemType = itemType;
//                m_Amount = amount;
//                m_Capacity = itemType.GetCapacity();

//                if (m_ItemType.GetType() == typeof(ConsumableItem))
//                    m_Equip = false;
//            }


//            public ItemAmount(ItemType itemType, Item item, int amount)
//            {
//                m_ItemType = itemType;
//                m_Item = item;
//                m_Amount = amount;
//                m_Capacity = itemType.GetCapacity();

//                if (m_ItemType.GetType() == typeof(ConsumableItem))
//                    m_Equip = false;
//            }

//        }





//        [SerializeField]
//        protected ItemAmount[] m_DefaultLoadout;

//        [SerializeField]
//        protected ItemType[] m_Loadout;
//        [SerializeField]
//        protected ItemAmount[] m_CurrentInventory;

//        protected Dictionary<ItemType, ItemAmount> m_Inventory;

//        [SerializeField, DisplayOnly]
//        protected ItemType m_EquippedItemType;


//        private int m_CurrentItemIndex = -1;
//        private bool m_Switching;



//        private ItemEquipSlot[] m_ItemSlots;
//        private ItemEquipSlot m_RightHandSlot;
//        private ItemEquipSlot m_LeftHandSlot;
//        private Transform m_ItemHolder;

//        private Animator m_Animator;



//        public ItemAmount[] DefaultLoadout
//        {
//            get { return m_DefaultLoadout; }
//            set { m_DefaultLoadout = value; }
//        }

//        public bool IsSwitchingItem
//        {
//            get { return m_Switching; }
//        }

//        public ItemType EquippedItemType
//        {
//            get { return m_EquippedItemType; }
//        }


//        //
//        // Methods
//        //
//        private void Awake()
//        {
//            m_Animator = GetComponent<Animator>();
//            m_ItemSlots = GetComponentsInChildren<ItemEquipSlot>();

//            m_Inventory = new Dictionary<ItemType, ItemAmount>();

//            m_ItemHolder = new GameObject("Items").transform;
//            m_ItemHolder.parent = transform;
//            m_ItemHolder.localPosition = transform.position + transform.up;



//            for (int i = 0; i < m_ItemSlots.Length; i++)
//            {
//                if (m_ItemSlots[i].ID == 0) m_RightHandSlot = m_ItemSlots[i];
//                if (m_ItemSlots[i].ID == 1) m_LeftHandSlot = m_ItemSlots[i];
//            }
//            LoadDefaultLoadout();
//        }



//        private void OnEnable()
//        {

//        }


//        private void OnDisable()
//        {

//        }



//        public void LoadDefaultLoadout()
//        {
//            for (int i = 0; i < m_DefaultLoadout.Length; i++)
//            {
//                var loadoutItem = m_DefaultLoadout[i];
//                PickupItem(loadoutItem.ItemType, loadoutItem.Amount, loadoutItem.Equip, false);
//            }
//        }




//        public void PickupItem(ItemType itemType, int amount, bool equip, bool immediateActivation)
//        {
//            if (!m_Inventory.ContainsKey(itemType))
//            {
//                //Debug.LogFormat("Inventory does not contain {0}. ", itemType);
//                ItemAmount itemAmount = new ItemAmount(itemType, amount);

//                if (itemType.GetType() == typeof(PrimaryItem))
//                {
//                    PrimaryItem primaryItem = (PrimaryItem)itemType;
//                    itemAmount.Item = primaryItem.CreateItem(m_RightHandSlot.transform);
//                    itemAmount.Item.SetActive(false);
//                }


//                m_Inventory.Add(itemType, itemAmount);
//                //  Update current inventory.
//                m_CurrentInventory = GrowArray(m_CurrentInventory, 1);
//                var nextSlotIndex = Array.IndexOf(m_CurrentInventory, null);
//                m_CurrentInventory[nextSlotIndex] = itemAmount;

//            }
//            else
//            {
//                if (itemType.Stackable || itemType is ConsumableItem)
//                {
//                    //Debug.LogFormat("Inventory does contain {0}. ", itemType);
//                    m_Inventory[itemType].Amount += amount;
//                }
//            }

//            if (equip)
//            {
//                EquipItem(itemType);
//            }

//        }




//        public Item GetCurrentItem(ItemType itemType)
//        {
//            if (m_EquippedItemType == null)
//                return null;

//            if (m_EquippedItemType == itemType)
//            {
//                return m_Inventory[itemType].Item;
//            }
//            return null;
//        }


//        public Item GetItem(ItemType itemType)
//        {
//            if (itemType.GetType() == typeof(PrimaryItem))
//            {
//                return m_Inventory[itemType].Item;
//            }
//            return null;
//        }


//        public void UseItem(ItemType itemType, int amount)
//        {
//            if (m_EquippedItemType == null)
//                return;

//            IUseableItem item = (IUseableItem)GetCurrentItem(itemType);
//            if (item != null && item.TryUse())
//            {
//                //  Update the inventory.
//                //  Play animation.
//                if (itemType.GetType() == typeof(PrimaryItem))
//                {
//                    var p = (PrimaryItem)itemType;
//                    p.ConsumableItem.CurrentAmount -= amount;
//                    //p.ConsumableItem.Capacity
//                }
//            }
//        }


//        public void ReloadItem(ItemType itemType, int amount)
//        {
//            if (m_EquippedItemType == null)
//                return;

//            IReloadableItem item = (IReloadableItem)GetCurrentItem(itemType);
//            if (item != null && item.TryStartReload())
//            {
//                //  Update the inventory.
//                //  Play animation.
//                if (itemType.GetType() == typeof(PrimaryItem))
//                {
//                    var p = (PrimaryItem)itemType;
//                    p.ConsumableItem.CurrentAmount += amount;
//                    //p.ConsumableItem.Capacity
//                }
//            }
//        }


//        public void SwitchItem(bool next)
//        {

//        }


//        public void EquipItem(ItemType itemType)
//        {
//            if (m_Inventory.ContainsKey(itemType))
//            {
//                if (itemType.GetType() == typeof(PrimaryItem))
//                {
//                    m_EquippedItemType = itemType as PrimaryItem;
//                    //Debug.Log(m_Inventory[m_EquippedItemType].Item);
//                    //m_Inventory[m_EquippedItemType].Item.SetActive(true);
//                }
//            }
//            //int itemIndex = Array.IndexOf(m_CurrentInventory, typeof(PrimaryItem));
//            //m_EquippedItemType = m_CurrentInventory[itemIndex];
//        }


//        public void EquipItem(int itemIndex)
//        {
//            if (itemIndex >= m_CurrentInventory.Length)
//            {
//                Debug.LogFormat("Item index is incorrect");
//                return;
//            }
//            if (m_CurrentInventory[itemIndex].GetType() == typeof(PrimaryItem))
//            {
//                m_EquippedItemType = m_CurrentInventory[itemIndex].ItemType as PrimaryItem;
//            }
//        }


//        public void UnequipCurrentItem()
//        {
//            GetCurrentItem(m_EquippedItemType).SetActive(false);
//            m_EquippedItemType = null;

//            EventHandler.ExecuteEvent(gameObject, "OnInventoryEquip", (Item)null);
//        }


//        public void ToggleEquippedItem()
//        {
//            //  Equip item.
//            if (m_EquippedItemType == null)
//            {
//                m_EquippedItemType = m_CurrentInventory[0].ItemType;
//                m_Inventory[m_EquippedItemType].Item.SetActive(true);
//                EventHandler.ExecuteEvent(gameObject, "OnInventoryEquip", m_CurrentInventory[0].Item);
//            }
//            //  Unequipp
//            else
//            {
//                UnequipCurrentItem();
//            }
//            //Debug.LogFormat("**  Toggling item. **");
//        }














//        private void OnInventoryAddItem(Item item)
//        {

//        }

//        private void OnInventoryPickupItem(Item item, float count, bool immediatePickup, bool forceEquip)
//        {

//        }

//        private void OnInventoryEquipItem(Item item, int slotID)
//        {

//        }

//        private void OnInventoryUnequipItem(Item item, int slotID)
//        {

//        }

//        private void OnInventoryRemoveItem(Item item, int slotID)
//        {

//        }





//        private T[] GrowArray<T>(T[] array, int increase)
//        {
//            T[] newArray = array;
//            Array.Resize(ref newArray, array.Length + increase);
//            return newArray;
//        }

//        private T[] ShrinkArray<T>(T[] array, int idx)
//        {
//            T[] newArray = new T[array.Length - 1];
//            if (idx > 0)
//                Array.Copy(array, 0, newArray, 0, idx);

//            if (idx < array.Length - 1)
//                Array.Copy(array, idx + 1, newArray, idx, array.Length - idx - 1);

//            return newArray;
//        }
//    }

//}
namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;



    public class Inventory : MonoBehaviour
    {
        public delegate void OnInventoryUseItem(ItemType itemType, float remaining);
        public delegate void OnInventoryPickupItem(Item item, float count, bool immediatePickup, bool forceEquip);
        public delegate void OnInventoryEquipItem(Item item);
        public delegate void OnInventoryUnequipItem(Item item);
        public delegate void OnInventoryAddItem(Item item);
        public delegate void OnInventoryRemoveItem(Item item, int slotID);

        public event OnInventoryUseItem InventoryUseItem = delegate { };
        public event OnInventoryPickupItem InventoryPickupItem = delegate { };
        public event OnInventoryEquipItem InventoryEquipItem = delegate { };
        public event OnInventoryUnequipItem InventoryUnequipItem = delegate { };
        public event OnInventoryAddItem InventoryAddItem = delegate { };
        public event OnInventoryRemoveItem InventoryRemoveItem = delegate{ };



        //[Header("-- Default Loadout --")]
        [SerializeField]
        protected ItemAmount[] m_DefaultLoadout;
        [SerializeField]
        protected ItemType[] m_Loadout;
        //[SerializeField]
        protected int m_NextInventorySlot;
        [SerializeField]
        protected CharacterEquipPoints m_EquipPoints;

        [SerializeField, DisplayOnly]
        protected ItemType m_EquippedItem;
        protected int m_EquippedItemIndex = -1;
        protected ItemType m_LastEquippedItem;
        protected bool m_Switching;


        protected Dictionary<ItemType, Item> m_Inventory;
        protected CharacterLocomotion m_Controller;
        protected AnimatorMonitor m_AnimatorMonitor;
        protected Animator m_Animator;




        //
        // Properties
        //
        public ItemAmount[] DefaultLoadout{
            get { return m_DefaultLoadout; }
            set { m_DefaultLoadout = value; }
        }

        public ItemType[] Loadout{
            get { return m_Loadout; }
        }

        public int EquippedItemIndex{
            get{ return m_EquippedItemIndex; }
            private set{
                if (m_EquippedItem == null)
                    m_EquippedItemIndex = -1;
                else 
                    m_EquippedItemIndex = Mathf.Clamp(value, 0, m_Loadout.Length);
            }
        }

        public int NextInventorySlot{
            get{
                m_NextInventorySlot = Array.IndexOf(m_Loadout, null);
                return m_NextInventorySlot;
            }
        }



        public ItemType EquippedItemType{
            get { return m_EquippedItem; }
        }

        public int CurrentItemID{
            get{
                if (m_EquippedItem == null)
                    return 0;
                return GetCurrentItem(m_EquippedItem).ItemID;
            }
        }

        //
        // Methods
        //
		private void Awake()
		{
            m_Controller = GetComponent<CharacterLocomotion>();
            m_AnimatorMonitor = GetComponent<AnimatorMonitor>();
            m_Animator = GetComponent<Animator>();
            m_Inventory = new Dictionary<ItemType, Item>();
            m_Loadout = new ItemType[6];

            //  Setup item hands slots.
            CacheItemEquipSlots();


		}


		private void Start()
		{
            //  Load default items.
            LoadDefaultLoadout();
		}


		private void CacheItemEquipSlots()
		{
            //  Setup item hands slots.
            ItemEquipSlot[] itemSlots = GetComponentsInChildren<ItemEquipSlot>();
            if (itemSlots.Length == 0){
                var rightHand = m_Animator.GetBoneTransform(HumanBodyBones.RightHand);
            }

            if(itemSlots.Length > 0){
                for (int i = 0; i < itemSlots.Length; i++)
                {
                    if (itemSlots[i].ID == 0) m_EquipPoints.RightHandSlot = itemSlots[i];
                    if (itemSlots[i].ID == 1) m_EquipPoints.LeftHandSlot = itemSlots[i];
                }
            }
		}


		private void OnEnable()
		{
            EventHandler.RegisterEvent(gameObject, "OnItemEquip", OnItemEquip);
            EventHandler.RegisterEvent(gameObject, "OnItemUnequip", OnItemUnequip);
		}



        private void OnDisable()
        {
            EventHandler.UnregisterEvent(gameObject, "OnItemEquip", OnItemEquip);
            EventHandler.UnregisterEvent(gameObject, "OnItemUnequip", OnItemUnequip);
        }


		private void OnValidate()
		{
            CacheItemEquipSlots();
		}


		public void LoadDefaultLoadout()
        {
            for (int i = 0; i < m_DefaultLoadout.Length; i++){
                var loadoutItem = m_DefaultLoadout[i];

                PickupItem(loadoutItem.ItemType, loadoutItem.Amount, i == 0 ? true : loadoutItem.Equip);
            }
        }




        public bool PickupItem(ItemType item, int amount, bool equip)
        {
            bool itemAddedToInventory = false;
            //  If ItemType is already in the inventory.
            if (m_Inventory.ContainsKey(item))
            {
                if (item is PrimaryItem){
                    //Item itemObject = InstantiateItem(item, m_ItemHolder);
                    //PrimaryItem primaryItem = (PrimaryItem)item;
                    //m_Inventory.Add(primaryItem, itemObject);
                    ////  TODO:  Get loadout item index and update loadout item.

                    itemAddedToInventory = true;
                }
                else if (item is ConsumableItem){
                    //  Add the amount to the inventory consumableItem.
                    itemAddedToInventory = true;
                }
                //Debug.LogFormat("{0} is already in inventory. ", item);
            }
            //  Pickup item not in inventory.
            else
            {
                if (item is PrimaryItem)
                {
                    var parent = m_EquipPoints.RightHandSlot.transform;
                    var customEquipPoint = ((PrimaryItem)item).CustomEquipPoint;
                    if( !string.IsNullOrWhiteSpace(customEquipPoint)){
                        if (m_EquipPoints.RightHandSlot.GetEquipPoint(customEquipPoint) != null)
                            parent = m_EquipPoints.RightHandSlot.GetEquipPoint(customEquipPoint);
                    }

                    Item itemObject = InstantiateItem(item, parent);
                    PrimaryItem primaryItem = (PrimaryItem)item;
                    m_Inventory.Add(primaryItem, itemObject);
                    //  Add item to the Loadout.
                    m_Loadout[NextInventorySlot] = primaryItem;

                    itemAddedToInventory = true;
                    //Debug.LogFormat("NextInventorySlot: {0} | m_NextInventorySlot: {1} | nextSlot {2} |", NextInventorySlot, m_NextInventorySlot, nextSlot);
                }
                else if (item is ConsumableItem)
                {
                    ConsumableItem consumableItem = (ConsumableItem)item;
                    m_Inventory.Add(consumableItem, null);

                    itemAddedToInventory = true;
                }
                //Debug.LogFormat("Adding {0} to inventory. ", item);
            }


            //  Set itemObjects settings.
            if(item is PrimaryItem)
            {
                PrimaryItem primaryItem = (PrimaryItem)item;
                //m_Inventory[item].ItemAnimName = primaryItem.ItemAnimName;
                if(equip){
                    EquipItem(item);
                } else {
                    m_Inventory[item].SetActive(false);
                }
            }
            else if (item is ConsumableItem)
            {
                ConsumableItem consumableItem = (ConsumableItem)item;
            }
            else{
                
            }

            return itemAddedToInventory;
        }


        private Item InstantiateItem(ItemType item, Transform parent)
        {
            if(item is PrimaryItem)
            {
                Item itemObject = Instantiate( ((PrimaryItem)item).OriginalObject).GetComponent<Item>();
                itemObject.transform.parent = parent;
                itemObject.transform.localPosition = Vector3.zero;
                itemObject.transform.localEulerAngles = Vector3.zero;
                //itemObject.transform.localPosition = itemObject.PositionOffset;
                //itemObject.transform.localEulerAngles = itemObject.RotationOffset;

                itemObject.Initialize(this);
                return itemObject;
            }

            return null;
        }



        public Item GetCurrentItem(){
            return GetCurrentItem(m_EquippedItem);
        }


        public Item GetCurrentItem(ItemType item)
        {
            if(item == null) return null;

            if(m_EquippedItem == item){
                return m_Inventory[item];
            }
            return null;
        }


        public Item GetItem(ItemType item)
        {
            if(m_Inventory.ContainsKey(item)){
                if(item is PrimaryItem){
                    return m_Inventory[item];
                }
            }
            return null;
        }


        public ItemType GetNextItem(bool next)
        {
            int n = next ? 1 : -1;
            for (int i = EquippedItemIndex + n; i < m_Loadout.Length || i < 0; i += n){
                if(m_Loadout[i] != null){
                    return m_Loadout[i];
                }
            }
            return null;
        }



        #region Inventory Actions

        public void UseItem(ItemType item, int amount)
        {
            if (m_EquippedItem == null)
                return;

            if(m_Controller.Aiming){
                IUseableItem useableItem = (IUseableItem)GetCurrentItem(item);
                if (useableItem != null && useableItem.TryUse())
                {
                    //  Update the inventory.

                    //  Play animation.
                    if (item.GetType() == typeof(PrimaryItem))
                    {
                        PrimaryItem primaryItem = (PrimaryItem)item;
                        //primaryItem.ConsumableItem.CurrentAmount -= amount;
                        //p.ConsumableItem.Capacity
                    }

                    InventoryUseItem(item, amount);
                }
            }

        }


        public void ReloadItem(ItemType item, int amount)
        {
            if (m_EquippedItem == null)
                return;

            IReloadableItem reloadableItem = (IReloadableItem)GetCurrentItem(item);
            if (reloadableItem != null && reloadableItem.TryStartReload()){
                //  Update the inventory.
                //  Play animation.
                if (item.GetType() == typeof(PrimaryItem)){
                    PrimaryItem primaryItem = (PrimaryItem)item;
                    //primaryItem.ConsumableItem.CurrentAmount += amount;
                    //p.ConsumableItem.Capacity
                }
            }
        }


        public void SwitchItem(bool next)
        {
            // If the current equipped item is the last in the inventory, than unequip item.
            if((next && m_EquippedItemIndex == m_Loadout.Length) || (next == false && m_EquippedItemIndex == 0)){
                UnequipCurrentItem();
            }
            //  If nothiing is equipped, equip the first item.
            else if(m_EquippedItemIndex == -1){
                EquipItem(0);
            }
            else{
                int nextItemIndex = next == true ? 1 : -1;
                m_EquippedItemIndex += nextItemIndex;

                //  If next item is null, unequip item.
                if(m_Loadout[m_EquippedItemIndex] == null){
                    UnequipCurrentItem();
                }
                else{
                    EquipItem(m_EquippedItemIndex);
                }
            }

            //Debug.LogFormat("- Switching {0} to {1}", m_LastEquippedItem, m_EquippedItem);
        }


        public void EquipItem(ItemType item)
        {
            if (m_Inventory.ContainsKey(item)){
                if (item.GetType() == typeof(PrimaryItem)){
                    var itemIndex = Array.IndexOf(m_Loadout, (PrimaryItem)item);
                    EquipItem(itemIndex);
                }
            }
        }


        public void EquipItem(int itemIndex)
        {
            if (itemIndex >= m_Loadout.Length || itemIndex < 0){
                Debug.LogFormat("- {0}:  Equip ItemType index is incorrect.  ItemIndex: {1}", GetType(), itemIndex);
                return;
            }

            m_LastEquippedItem = m_EquippedItem;

            //Debug.LogFormat("{0} is {1}", m_Loadout[itemIndex], m_Loadout[itemIndex] is PrimaryItem);
            if (m_Loadout[itemIndex] is PrimaryItem)
            {
                ////  Turn item object off.
                //Item itemObject = GetCurrentItem(m_LastEquippedItem);
                //if (itemObject != null)
                    //itemObject.SetActive(false);
                
                //  Set equipped item and set index.
                m_EquippedItemIndex = itemIndex;
                m_EquippedItem = m_Loadout[itemIndex] as PrimaryItem;
                //GetCurrentItem(m_EquippedItem).SetActive(true);

                ////Debug.Log(m_Inventory[m_EquippedItem].Item);
                //m_Inventory[m_EquippedItem].SetActive(true);
                ////GetItem(m_EquippedItem).transform.parent = m_RightHandSlot.transform;

                m_Animator.SetInteger(HashID.ItemID, GetItem(m_EquippedItem).ItemID);
                m_Animator.SetInteger(HashID.MovementSetID, GetItem(m_EquippedItem).MovementSetID);

                EventHandler.ExecuteEvent(gameObject, "OnInventoryEquip", GetItem(m_Loadout[itemIndex]));
                return;
            }
        }


        public void UnequipCurrentItem()
        {
            m_LastEquippedItem = m_EquippedItem;

            //Item itemObject = GetCurrentItem();
            //if (itemObject != null)
                //itemObject.SetActive(false);
            m_EquippedItem = null;
            m_EquippedItemIndex = -1;
            m_Animator.SetInteger(HashID.ItemID, 0);
            m_Animator.SetInteger(HashID.MovementSetID, 0);

            EventHandler.ExecuteEvent(gameObject, "OnInventoryEquip", (Item)null);
        }


        public void ToggleEquippedItem()
        {
            //  Equip item.
            if (m_EquippedItem == null && m_LastEquippedItem != null){
                EquipItem(m_LastEquippedItem);
            }
            //  Unequipp
            else if(m_EquippedItem != null){
                UnequipCurrentItem();
            }
            //Debug.LogFormat("**  Toggling item. **");
        }

        #endregion










        private void OnItemEquip()
        {
            //m_Switching = true;
            //Debug.LogFormat("<color=magenta> OnItemEquip </color> | EquippedItem: {0}", m_EquippedItem);

            if (m_EquippedItem != null)
                m_Inventory[m_EquippedItem].SetActive(true);
        }

        private void OnItemUnequip()
        {
            //m_Switching = true;

            //Debug.LogFormat("<color=yellow> OnItemUnequip </color> | CurrentItem: {0}", itemObject);

            if (m_LastEquippedItem != null)
                GetItem(m_LastEquippedItem).SetActive(false);
        }








        [Serializable]
        public class CharacterEquipPoints
        {
            [SerializeField] protected ItemEquipSlot m_LeftHandSlot;
            [SerializeField] protected ItemEquipSlot m_RightHandSlot;

            public ItemEquipSlot LeftHandSlot
            {
                get { return m_LeftHandSlot; }
                set { m_LeftHandSlot = value; }
            }

            public ItemEquipSlot RightHandSlot
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
                get { return Mathf.Clamp(m_Amount, 0, m_Item.GetCapacity()); }
                set { m_Amount = Mathf.Clamp(value, 0, m_Item.GetCapacity()); }
            }


            public bool Equip{
                get{
                    if (m_Item.GetType() == typeof(ConsumableItem))
                        m_Equip = false;
                    return m_Equip;
                }
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
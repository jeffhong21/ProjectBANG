namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    using JH_Utils;

    public class Inventory : MonoBehaviour
    {
        //[Header("-- Default Loadout --")]
        [SerializeField]
        protected ItemAmount[] m_DefaultLoadout;
        [SerializeField]
        protected Item[] m_Loadout;
        //[SerializeField]
        protected int m_NextInventorySlot;
        [SerializeField]
        protected CharacterEquipPoints m_EquipPoints;

        [Header("-- Equipped Item --")]
        [SerializeField, DisplayOnly]
        protected Item m_EquippedItem;
        [SerializeField, DisplayOnly]
        protected int m_EquippedItemIndex = -1;
        [SerializeField, DisplayOnly]
        protected Item m_LastEquippedItem;
        [SerializeField]
        protected bool m_Switching;


        protected Dictionary<Item, ItemObject> m_Inventory;
        protected AnimatorMonitor m_AnimatorMonitor;
        protected Animator m_Animator;
        protected Transform m_ItemHolder;


    
        //
        // Properties
        //
        public ItemAmount[] DefaultLoadout{
            get { return m_DefaultLoadout; }
            set { m_DefaultLoadout = value; }
        }

        public Item[] Loadout{
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

        public bool IsSwitchingItem{
            get {
                if(GetCurrentItem() != null){
                    var switching = m_Animator.GetCurrentAnimatorStateInfo(m_AnimatorMonitor.UpperBodyLayerIndex).shortNameHash == Animator.StringToHash(GetCurrentItem().ItemName);
                }
                return m_Switching;
            }
        }

        public Item EquippedItemType{
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
            m_AnimatorMonitor = GetComponent<AnimatorMonitor>();
            m_Animator = GetComponent<Animator>();
            m_Inventory = new Dictionary<Item, ItemObject>();
            m_Loadout = new Item[6];

            //  Setup item hands slots.
            CacheItemEquipSlots();

            //  Set up item holder.
            m_ItemHolder = new GameObject("Item Holder").transform;
            m_ItemHolder.parent = transform;
            m_ItemHolder.localPosition = Vector3.up;

            //  Load default items.
            LoadDefaultLoadout();
		}


		private void CacheItemEquipSlots()
		{
            //  Setup item hands slots.
            ItemEquipSlot[] itemSlots = GetComponentsInChildren<ItemEquipSlot>();
            for (int i = 0; i < itemSlots.Length; i++){
                if (itemSlots[i].ID == 0) m_EquipPoints.RightHandSlot = itemSlots[i];
                if (itemSlots[i].ID == 1) m_EquipPoints.LeftHandSlot = itemSlots[i];
            }
		}


		private void OnEnable()
		{
            EventHandler.RegisterEvent(gameObject, "ItemEquip", OnItemEquip);
		}



        private void OnDisable()
        {

        }


		private void OnValidate()
		{
            CacheItemEquipSlots();
		}


		public void LoadDefaultLoadout()
        {
            for (int i = 0; i < m_DefaultLoadout.Length; i++){
                var loadoutItem = m_DefaultLoadout[i];
                PickupItem(loadoutItem.Item, loadoutItem.Amount,loadoutItem.Equip);
            }
        }




        public bool PickupItem(Item item, int amount, bool equip)
        {
            bool itemAddedToInventory = false;
            //  If Item is already in the inventory.
            if (m_Inventory.ContainsKey(item))
            {
                if (item is PrimaryItem){
                    //ItemObject itemObject = InstantiateItem(item, m_ItemHolder);
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

                    ItemObject itemObject = InstantiateItem(item, parent);
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
                m_Inventory[item].SetActive(false);
                //m_Inventory[item].ItemName = primaryItem.ItemName;

                if(equip){
                    EquipItem(item);
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


        private ItemObject InstantiateItem(Item item, Transform parent)
        {
            if(item is PrimaryItem)
            {
                ItemObject itemObject = Instantiate( ((PrimaryItem)item).OriginalObject).GetComponent<ItemObject>();
                itemObject.transform.parent = parent;
                itemObject.transform.localPosition = Vector3.zero;
                itemObject.transform.localEulerAngles = Vector3.zero;
                //itemObject.transform.localPosition = itemObject.PositionOffset;
                //itemObject.transform.localEulerAngles = itemObject.RotationOffset;

                return itemObject;
            }

            return null;
        }



        public ItemObject GetCurrentItem()
        {
            return GetCurrentItem(m_EquippedItem);
        }


        public ItemObject GetCurrentItem(Item item)
        {
            if(item == null) return null;

            if(m_EquippedItem == item){
                return m_Inventory[item];
            }
            return null;
        }


        public ItemObject GetItem(Item item)
        {
            if(m_Inventory.ContainsKey(item)){
                if(item is PrimaryItem){
                    return m_Inventory[item];
                }
            }
            return null;
        }


        public Item GetNextItem(bool next)
        {
            int n = next ? 1 : -1;
            for (int i = EquippedItemIndex + n; i < m_Loadout.Length || i < 0; i += n){
                if(m_Loadout[i] != null){
                    return m_Loadout[i];
                }
            }
            return null;
        }


        public void UseItem(Item item, int amount)
        {
            if (m_EquippedItem == null)
                return;

            IUseableItem useableItem = (IUseableItem)GetCurrentItem(item);
            if(useableItem != null && useableItem.TryUse()){
                //  Update the inventory.
                //  Play animation.
                if(item.GetType() == typeof(PrimaryItem)){
                    PrimaryItem primaryItem = (PrimaryItem)item;
                    //primaryItem.ConsumableItem.CurrentAmount -= amount;
                    //p.ConsumableItem.Capacity
                }
            }
        }


        public void ReloadItem(Item item, int amount)
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



        public void EquipItem(Item item)
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
                Debug.LogFormat("- {0}:  Equip Item index is incorrect.  ItemIndex: {1}", GetType(), itemIndex);
                return;
            }

            m_LastEquippedItem = m_EquippedItem;

            //Debug.LogFormat("{0} is {1}", m_Loadout[itemIndex], m_Loadout[itemIndex] is PrimaryItem);
            if (m_Loadout[itemIndex] is PrimaryItem)
            {
                
                //  Turn item object off.
                ItemObject itemObject = GetCurrentItem(m_LastEquippedItem);
                if (itemObject != null)
                    itemObject.SetActive(false);
                //  Set equipped item and set index.
                m_EquippedItemIndex = itemIndex;
                m_EquippedItem = m_Loadout[itemIndex] as PrimaryItem;
                //Debug.Log(m_Inventory[m_EquippedItem].ItemObject);
                m_Inventory[m_EquippedItem].SetActive(true);
                //GetItem(m_EquippedItem).transform.parent = m_RightHandSlot.transform;

                EventHandler.ExecuteEvent(gameObject, "OnInventoryEquip", GetItem(m_Loadout[itemIndex]));
                return;
            }
        }


        public void UnequipCurrentItem()
        {
            m_LastEquippedItem = m_EquippedItem;

            ItemObject itemObject = GetCurrentItem();
            if (itemObject != null)
                itemObject.SetActive(false);
            

            m_EquippedItem = null;
            m_EquippedItemIndex = -1;


            EventHandler.ExecuteEvent(gameObject, "OnInventoryEquip", (ItemObject)null);
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






        private void OnItemEquip()
        {
            //m_Switching = true;
            //Debug.Log("OnItemEquip");

        }



        private void OnInventoryAddItem(ItemObject item)
        {

        }

        private void OnInventoryPickupItem(ItemObject item, float count, bool immediatePickup, bool forceEquip)
        {

        }

        private void OnInventoryEquipItem(ItemObject item)
        {
            ItemObject itemObject = GetCurrentItem();
            if (itemObject != null)
                itemObject.SetActive(false);

            item.SetActive(true);
        }

        private void OnInventoryUnequipItem(ItemObject item)
        {

        }

        private void OnInventoryRemoveItem(ItemObject item, int slotID)
        {

        }





        [Serializable]
        public class CharacterEquipPoints
        {
            [SerializeField] protected ItemEquipSlot m_RightHandSlot;
            [SerializeField] protected ItemEquipSlot m_LeftHandSlot;

            public ItemEquipSlot RightHandSlot
            {
                get { return m_RightHandSlot; }
                set { m_RightHandSlot = value; }
            }
            public ItemEquipSlot LeftHandSlot
            {
                get { return m_LeftHandSlot; }
                set { m_LeftHandSlot = value; }
            }
        }



        [Serializable]
        public class ItemAmount
        {
            [SerializeField] protected Item m_Item;
            [SerializeField] protected int m_Amount = 1;
            [SerializeField] protected bool m_Equip = true;


            public Item Item{
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


            public ItemAmount(Item itemType, int amount){
                Initialize(itemType, amount);
            }

            public void Initialize(Item itemType, int amount){
                Item = itemType;
                Amount = amount;
            }
        }
	}

}
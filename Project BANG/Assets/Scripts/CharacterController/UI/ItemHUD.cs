namespace CharacterController.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using TMPro;

    public class ItemHUD : MonoBehaviour
    {
        [Header("Target Character")]
        [SerializeField]
        protected GameObject character;
        [Header("UI Parts")]
        [SerializeField]
        protected TextMeshProUGUI itemName;
        [SerializeField]
        protected TextMeshProUGUI currentAmount;
        [SerializeField]
        protected TextMeshProUGUI totalAmount;



        public GameObject Character { get { return character; } set { character = value; } }


        private void Awake()
        {
            if(character == null)
                character = GameObject.FindGameObjectWithTag("Player");

            gameObject.SetActive(true);

        }

        private void Start()
		{

            if(character != null) {
                EventHandler.RegisterEvent<Item>(character, EventIDs.OnInventoryEquipItem, OnEquipItem);
                EventHandler.RegisterEvent<Item>(character, EventIDs.OnInventoryUnequipItem, OnUnequipItem);
                EventHandler.RegisterEvent<Item>(character, EventIDs.OnInventoryAddItem, OnInventoryAddItem);
                EventHandler.RegisterEvent<Item>(character, EventIDs.OnInventoryRemoveItem, OnInventoryRemoveItem);
                EventHandler.RegisterEvent<Item, int, bool, bool>(character, EventIDs.OnInventoryPickupItem, OnInventoryPickupItem);
            }

        }

        private void OnDestroy()
        {
            if (character != null) {
                EventHandler.UnregisterEvent<Item>(character, EventIDs.OnInventoryEquipItem, OnEquipItem);
                EventHandler.UnregisterEvent<Item>(character, EventIDs.OnInventoryUnequipItem, OnUnequipItem);
                EventHandler.UnregisterEvent<Item>(character, EventIDs.OnInventoryAddItem, OnInventoryAddItem);
                EventHandler.UnregisterEvent<Item>(character, EventIDs.OnInventoryRemoveItem, OnInventoryRemoveItem);
                EventHandler.UnregisterEvent<Item, int, bool, bool>(character, EventIDs.OnInventoryPickupItem, OnInventoryPickupItem);
            }


        }





        private void UseItem(ItemType itemType, float remaining)
        {
            //PrimaryItem primaryItem = (PrimaryItem)itemType;
            ////Debug.LogFormat("{0} was used.  {1} out of {2} remain loaded.", itemType.name, remaining, primaryItem.ConsumableItem.Capacity);

            ////m_CurrentAmmo.text = remaining + " | " + primaryItem.ConsumableItem.Capacity.ToString();
            //m_CurrentAmmo.text = remaining.ToString();

            Debug.LogFormat("<b><color=green>{0}</color></b> was used. <b><color=green>{1}</color></b> remaining.", itemType, remaining);
        }

        private void OnEquipItem(Item item)
        {
            if (item == null) {
                currentAmount.text = "-";
                totalAmount.text = "-";
                itemName.text = "Unarmed";
            } else {
                //var shootable = (ShootableWeapon)item;
                itemName.text = item.ItemType.name;
                //totalAmount.text = primaryWeapon.ConsumableItem.Capacity.ToString();
                ////m_CurrentAmmo.text = string.Format("{0} | {1}", shootable.CurrentAmmo, item.ItemType.ConsumableItem.Capacity);
            }
            Debug.LogFormat("<b><color=green>{0}</color></b> has equipped.", item);

        }

        private void OnUnequipItem(Item item)
        {
            Debug.LogFormat("<b><color=blue>{0}</color></b> has been unequipped.", item);
        }

        private void OnInventoryPickupItem(Item item, int count, bool immediatePickup, bool forceEquip)
        {
            
        }

        private void OnInventoryAddItem( Item item )
        {
            Debug.LogFormat("<b><color=green>{0}</color></b> added to the inventory.", item);
        }

        private void OnInventoryRemoveItem(Item item)
        {
            Debug.LogFormat("<b><color=blue>{0}</color></b> removed from the inventory.", item);
        }


	}
}
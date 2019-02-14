namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using JH_Utils;

    public class Inventory_1 : MonoBehaviour
    {
        
        [SerializeField, DisplayOnly]
        protected Item m_EquippedItemType;
        [SerializeField]
        protected PrimaryItem[] m_Loadout;
        [SerializeField]
        protected Dictionary<Item, ItemObject> m_Inventory;


        private int m_CurrentItemIndex = -1;
        private bool m_Switching;

        private Animator m_Animator;



        public bool IsSwitchingItem{
            get { return m_Switching; }
        }

        public Item EquippedItemType{
            get { return m_EquippedItemType; }
        }



        //
        // Methods
        //
        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_Inventory = new Dictionary<Item, ItemObject>();


            //LoadDefaultLoadout();
        }


        public void PickupItem(Item itemType, int amount, bool equip, bool immediateActivation)
        {
            //if (!m_Inventory.ContainsKey(itemType))
            //{
            //    if (itemType.GetType() == typeof(PrimaryItem))
            //    {
            //        PrimaryItem primaryItem = (PrimaryItem)itemType;
            //        m_Inventory[itemType] = primaryItem.CreateItem(m_RightHandSlot.transform);
            //        m_Inventory[itemType].SetActive(false);
            //    }
            //    else if (itemType.Stackable || itemType is ConsumableItem)
            //    {

            //    }
            //}
            //else
            //{
            //    if (itemType.GetType() == typeof(PrimaryItem))
            //    {
            //        PrimaryItem primaryItem = (PrimaryItem)itemType;
            //        m_Inventory[itemType] = primaryItem.CreateItem(m_RightHandSlot.transform);
            //        m_Inventory[itemType].SetActive(false);
            //    }
            //    else if (itemType.Stackable || itemType is ConsumableItem)
            //    {

            //    }
            //}

            //if (equip)
            //{
            //    EquipItem(itemType);
            //}

        }
    }

}






namespace CharacterController
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;


    public class ItemActionManager : MonoBehaviour
    {
        protected const string ReloadState = "Reload";
        protected const string EquipState = "Equip";
        protected const string UnequipState = "Unequip";



        protected int m_LayerIndex;
        protected float m_TransitionDuration = 0.2f;
        protected AnimatorTransitionInfo m_TransitionInfo;

        protected Inventory m_Inventory;
        protected Animator m_Animator;
        protected AnimatorMonitor m_AnimatorMonitor;
        protected CharacterLocomotion m_Controller;
        protected LayerManager m_LayerManager;



        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_AnimatorMonitor = GetComponent<AnimatorMonitor>();
            m_Controller = GetComponent<CharacterLocomotion>();
            m_LayerManager = GetComponent<LayerManager>();
            m_Inventory = GetComponent<Inventory>();
        }

		private void Start()
		{
            m_LayerIndex = m_AnimatorMonitor.UpperBodyLayerIndex;
		}





		public void UseItem()
        {
            m_Inventory.UseItem(m_Inventory.EquippedItemType, 1);
        }

        public void Reload()
        {
            if (m_Inventory.GetCurrentItem() == null)
                return;
            
            m_Animator.CrossFade(Animator.StringToHash(GetFullStateName(ReloadState)), m_TransitionDuration, m_LayerIndex);


            m_Inventory.ReloadItem(m_Inventory.EquippedItemType, 1);
        }


        public void SwitchItem(bool next)
        {
            m_Inventory.SwitchItem(next);
            m_AnimatorMonitor.SetItemID(m_Inventory.CurrentItemID);
        }


        public void EquipItem(int index)
        {
            m_Inventory.EquipItem(index);
            m_AnimatorMonitor.SetItemID(m_Inventory.CurrentItemID);
        }

        public void ToggleItem()
        {
            m_Inventory.ToggleEquippedItem();
            m_AnimatorMonitor.SetItemID(m_Inventory.CurrentItemID);
        }


        public void DropItem(int itemID)
        {
            
        }




        protected string GetFullStateName(string state)
        {
            var itemObject = m_Inventory.GetCurrentItem();
            if (itemObject == null)
                return null;
            
            var itemName = itemObject.ItemName;
            var layerName = m_AnimatorMonitor.UpperBodyLayerName;

            string stateName = string.Format("{0}.{1}.{2}", layerName, itemName, state);
            return stateName;
        }



        protected int GetItemID()
        {
            var itemObject = m_Inventory.GetCurrentItem();
            if (itemObject == null)
                return 0;

            var itemID = itemObject.ItemID;
            return itemID;
        }

    }

}
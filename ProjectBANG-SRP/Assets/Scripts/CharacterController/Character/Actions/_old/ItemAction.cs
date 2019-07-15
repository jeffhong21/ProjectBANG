namespace CharacterController
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;

    public abstract class ItemAction : CharacterAction
    {





        protected int GetItemID()
        {
            var itemObject = m_Inventory.GetCurrentItem();
            if (itemObject == null)
                return 0;

            var itemID = itemObject.ItemID;
            return itemID;
        }

        protected string GetItemName()
        {
            var itemObject = m_Inventory.GetCurrentItem();
            if (itemObject == null)
                return "";

            var itemName = itemObject.ItemAnimName;
            return itemName;
        }
    }
}
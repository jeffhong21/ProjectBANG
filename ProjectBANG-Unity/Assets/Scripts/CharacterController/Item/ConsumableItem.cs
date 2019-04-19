namespace CharacterController
{
    using UnityEngine;
    using System;


    /// <summary>
    /// Any items that can be consumed by the PrimaryItem. These items cannot be equipped or used independently.
    /// </summary>
    public class ConsumableItem : ItemType
    {
        [SerializeField]
        protected int m_Amount = 1;
        [SerializeField]
        protected GameObject m_DropObject;


        public int Amount
        {
            get { return m_Amount; }
            set { m_Amount = Mathf.Clamp(value, 0, Capacity); }
        }


        public GameObject DropObject
        {
            get { return m_DropObject; }
            set { m_DropObject = value; }
        }

        /// <summary>
        /// What is the max amount of this consumable item the player can carry.
        /// Returns the maximum capacity of an item. Note that this does not specify a maximum value on the number 
        /// of consumable items a particular primary item can hold. That is defined within PrimaryItem.IncludedConsumableItem.
        /// </summary>
        /// <returns>The capacity.</returns>
        public override int GetCapacity()
        {
            return m_Capacity;
        }

    }

}

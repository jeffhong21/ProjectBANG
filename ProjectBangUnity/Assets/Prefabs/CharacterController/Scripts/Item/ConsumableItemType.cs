namespace CharacterController
{
    using UnityEngine;
    using System;


    /// <summary>
    /// Any items that can be consumed by the PrimaryItemType. These items cannot be equipped or used independently.
    /// </summary>
    public class ConsumableItemType : ItemType
    {



        /// <summary>
        /// What is the max amount of this consumable item the player can carry.
        /// Returns the maximum capacity of an item. Note that this does not specify a maximum value on the number 
        /// of consumable items a particular primary item can hold. That is defined within PrimaryItemType.IncludedConsumableItem.
        /// </summary>
        /// <returns>The capacity.</returns>
        public override int GetCapacity()
        {
            return m_Capacity;
        }

    }

}

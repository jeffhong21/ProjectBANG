namespace CharacterController
{
    using UnityEngine;
    using System.Collections.Generic;

    public enum ItemSlotID { None = -1, RightHand = 0, LeftHand = 1 }

    public class ItemEquipSlot : MonoBehaviour
    {
        [SerializeField]
        private ItemSlotID slotID = ItemSlotID.None;




        public ItemSlotID SlotID {
            get { return slotID; }
        }




    }
}
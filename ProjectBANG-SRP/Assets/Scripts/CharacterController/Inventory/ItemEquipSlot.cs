namespace CharacterController
{
    using UnityEngine;
    using System.Collections.Generic;

    public enum ItemSlotID { None, RightHand, LeftHand }

    public class ItemEquipSlot : MonoBehaviour
    {
        [SerializeField]
        private ItemSlotID slotID = ItemSlotID.None;




        public ItemSlotID SlotID {
            get { return slotID; }
        }




    }
}
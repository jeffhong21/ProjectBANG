namespace CharacterController
{
    using UnityEngine;


    public class EquipWeapon : CharacterAction
    {



        protected override void ActionStarted()
        {
            int slotID = m_InputIndex;

            Item item = m_Inventory.GetItem(slotID);
            if(item != null)
            {
                m_Inventory.EquipItem(item.ItemType);
            }
            


        }




        public override string GetDestinationState(int layer)
        {
            return "";
        }
    }

}

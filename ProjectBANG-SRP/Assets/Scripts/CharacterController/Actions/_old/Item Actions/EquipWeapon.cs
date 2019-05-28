namespace CharacterController
{
    using UnityEngine;


    public class EquipWeapon : CharacterAction
    {
        public const int ITEM_STATE_ID = 4;

        [SerializeField]
        protected int m_ItemID;
        [SerializeField]
        protected string m_ItemName;
        [SerializeField]
        protected ItemType m_ItemType;



        protected override void ActionStarted()
        {
            if (m_Inventory.CurrentLoadout[m_InputIndex] != null)
                Debug.LogFormat("Inventory Slot ({0}): {1}", m_InputIndex, m_Inventory.CurrentLoadout[m_InputIndex].ItemName);
            else
                Debug.LogFormat("Inventory Slot ({0}) is empty.", m_InputIndex);
            
            m_Inventory.EquipItem(m_InputIndex);
            
            m_ItemType = m_Inventory.CurrentLoadout[m_InputIndex];
            m_ItemName = m_ItemType.ItemName;
            m_ItemID = m_ItemType.ID;

            m_Animator.SetInteger(HashID.ItemStateIndex, ITEM_STATE_ID);
            m_Animator.SetInteger(HashID.ItemID, m_ItemID);
            m_Animator.SetTrigger(HashID.ItemStateIndexChange);
        }


		protected override void ActionStopped()
		{
            m_Animator.SetInteger(HashID.ItemStateIndex, 0);
            m_Animator.ResetTrigger(HashID.ItemStateIndexChange);
		}


		public override string GetDestinationState(int layer)
        {
            if (layer == m_Animator.GetLayerIndex("Item Layer")){
                return m_ItemName;
            }
            return "";
        }
    }

}

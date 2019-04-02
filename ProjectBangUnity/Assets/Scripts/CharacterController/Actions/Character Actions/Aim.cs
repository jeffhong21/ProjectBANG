namespace CharacterController
{
    using UnityEngine;


    public class Aim : CharacterAction
    {
        protected const int m_ItemStateID = 1;

        [SerializeField]
        protected int m_ItemID;
        [SerializeField]
        protected string m_ItemName;



        //
        // Methods
        //
        //public override bool CanStartAction()
        //{
        //    return m_Controller.Aiming;
        //}

        //public override bool CanStopAction()
        //{
        //    return m_Controller.Aiming;
        //}



        protected override void ActionStarted()
        {
            m_ItemID = GetItemID();
            m_ItemName = GetItemName();

            m_AnimatorMonitor.SetItemID(GetItemID(), m_ItemStateID);
            m_Animator.SetBool(HashID.Aiming, true);
            EventHandler.ExecuteEvent(m_GameObject, "OnAimActionStart", true);
        }


        protected override void ActionStopped()
        {
            
            m_AnimatorMonitor.SetItemID(GetItemID(), 0);
            m_Animator.SetBool(HashID.Aiming, false);
            EventHandler.ExecuteEvent(m_GameObject, "OnAimActionStart", false);
        }



        public override string GetDestinationState(int layer)
        {
            if(layer == m_AnimatorMonitor.ItemLayerIndex)
            {
                return m_ItemName;
                //if(m_Inventory.EquippedItemType != null){
                //    var itemObject = m_Inventory.GetCurrentItem(m_Inventory.EquippedItemType);
                //    var aimState = string.Format("{0}.{1}", itemObject.ItemAnimName, itemObject.AnimStates.AimStateName);
                //    //Debug.Log(aimState);
                //    return aimState;
                //}
            }
            return "";
        }



        public override bool IsConcurrentAction()
        {
            return true;
        }



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
                return "Unarmed";

            var itemName = itemObject.ItemAnimName;
            return itemName;
        }

    }

}

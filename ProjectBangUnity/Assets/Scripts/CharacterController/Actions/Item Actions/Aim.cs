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

        [SerializeField]
        protected bool m_Aiming;


        //
        // Methods
        //
        private void Reset()
        {
            m_InputName = "Fire2";
            m_StartType = ActionStartType.ButtonDown;
            m_StopType = ActionStopType.ButtonToggle;
        }




        protected override void ActionStarted()
        {
            m_Aiming = true;
            m_ItemID = GetItemID();
            m_ItemName = GetItemName();

            m_AnimatorMonitor.SetItemID(GetItemID(), m_ItemStateID);
            //m_Animator.SetBool(HashID.Aiming, m_Aiming);
            //m_AnimatorMonitor.SetItemID(m_ItemID, m_ItemStateID);

            //Debug.LogFormat("{0} is {1}", GetType().Name, this.enabled);

            EventHandler.ExecuteEvent(m_GameObject, "OnAimActionStart", m_Aiming);


        }


        protected override void ActionStopped()
        {
            m_Aiming = false;


            m_Animator.SetBool(HashID.Aiming, m_Aiming);
            m_AnimatorMonitor.SetItemID(GetItemID(), 0);

            EventHandler.ExecuteEvent(m_GameObject, "OnAimActionStart", m_Aiming);
        }



        public override string GetDestinationState(int layer)
        {
            if(layer == m_AnimatorMonitor.UpperBodyLayerIndex)
            {
                return m_ItemName;
                //if(m_Inventory.EquippedItemType != null){
                //    var itemObject = m_Inventory.GetCurrentItem(m_Inventory.EquippedItemType);
                //    var aimState = string.Format("{0}.{1}", itemObject.ItemName, itemObject.AnimStates.AimStateName);
                //    //Debug.Log(aimState);
                //    return aimState;
                //}
            }
            return "";
        }


        public override bool UpdateAnimator()
        {
            m_Animator.SetBool(HashID.Aiming, m_Aiming);
            //m_AnimatorMonitor.SetItemID(m_ItemID, m_ItemStateID);

            return true;
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

            var itemName = itemObject.ItemName;
            return itemName;
        }

    }

}

namespace CharacterController
{
    using UnityEngine;


    public class EquipWeaponCycle : CharacterAction
    {
        [Header("--  Equip Weapon Action Settings --")]
        [SerializeField]
        protected const int m_EquipStateID = 3;
        [SerializeField]
        protected const int m_UnequipStateID = 4;
        [SerializeField]
        protected ItemType m_Item;
        [SerializeField]
        protected string m_ItemName;
        [SerializeField]
        protected int m_LayerIndex;
        [Header("--  States --")]
        [SerializeField]
        protected bool m_IsEquipping;
        [SerializeField]
        protected bool m_IsUnequipping;
        [SerializeField]
        protected bool m_IsSwitching;

        //
        // Methods
        //
        protected virtual void Start()
        {
            m_LayerIndex = m_AnimatorMonitor.UpperBodyLayerIndex;
        }

		//public override void StartAction()
		//{
		//    m_IsActive = true;
		//    ActionStarted();
		//    EventHandler.ExecuteEvent(m_GameObject, "OnCharacterActionActive", this, true);
		//    //m_Animator.CrossFade(Animator.StringToHash(GetDestinationState(m_LayerIndex)), m_TransitionDuration, m_LayerIndex);
		//}

		//public override void StopAction()
		//{
		//    m_IsActive = false;
		//    ActionStopped();
		//    EventHandler.ExecuteEvent(m_GameObject, "OnCharacterActionActive", this, false);
		//}





        public override bool CanStartAction()
        {

            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (!m_IsActive)
                {
                    return true;
                }
            }
            return false;
		}

		protected override void ActionStarted()
        {
            m_Inventory.SwitchItem(true);
            m_AnimatorMonitor.SetItemID(m_Inventory.CurrentItemID);

            //Debug.LogFormat("Starting EquipUnequip");
            //var currentItem = m_Inventory.EquippedItemType;
            //var nextItem = m_Inventory.GetNextItem(true);
            ////  If no item is equipped, than equip next item.
            //if (currentItem == null && nextItem != null)
            //{
            //    //Debug.LogFormat("Equipping {0}", nextItem.name);
            //    m_IsSwitching = false;
            //    m_AnimatorMonitor.SetItemID(m_Inventory.GetItem(nextItem).ItemID);
            //    //m_AnimatorMonitor.SetItemStateIndex(m_EquipStateID);

            //    m_Inventory.EquipItem(nextItem);
            //}
            ////  Switching Items
            //else if (currentItem != null && nextItem != null)
            //{
            //    //Debug.LogFormat("{0} is switching to {1}", currentItem.name, nextItem.name);
            //    m_IsSwitching = true;
            //    m_AnimatorMonitor.SetItemID(m_Inventory.GetItem(nextItem).ItemID);
            //    //m_AnimatorMonitor.SetItemStateIndex(m_EquipStateID);

            //    m_Inventory.SwitchItem(true);
            //}
            ////  Unequipping.
            //else if (nextItem == null)
            //{
            //    //Debug.LogFormat("Unequipping {0}", currentItem.name);
            //    m_Inventory.UnequipCurrentItem();
            //    m_IsSwitching = false;
            //    m_AnimatorMonitor.SetItemID(m_Inventory.GetItem(currentItem).ItemID);
            //    //m_AnimatorMonitor.SetItemStateIndex(m_UnequipStateID);
            //}
            //else
            //{

            //}
        }





        public override bool CanStopAction()
        {
            if (m_IsActive)
            {
                m_TransitionInfo = m_Animator.GetAnimatorTransitionInfo(m_LayerIndex);
                //Debug.LogFormat("Switching transition duration: {0}", duration);
                if (GetNormalizedTime() > 1 - m_TransitionInfo.duration)
                {
                    return true;
                }
            }

            return false;
        }


        protected override void ActionStopped()
        {
            //Debug.LogFormat("Done Equipping Unequipping.");
            //m_AnimatorMonitor.SetItemStateIndex(0);
            //m_IsSwitching = false;



            //if(m_Inventory.EquippedItemType != null){
            //    var itemObject = m_Inventory.GetItem(m_Inventory.EquippedItemType);
            //    if (itemObject != null)
            //        m_AnimatorMonitor.SetMovementSetID(m_Inventory.GetItem(m_Inventory.EquippedItemType).MovementSetID);
            //}
        }

        //public override void ActionWillStart(CharacterAction nextAction)
        //{
        //    if(nextAction.CanStartAction()){
        //        nextAction.StartAction();
        //    }
        //}



        public override string GetDestinationState(int layer)
        {
            //var currentItem = m_Inventory.GetCurrentItem();
            //var nextItem = m_Inventory.GetNextItem(true);

            //if (currentItem == null){
            //    if (nextItem == null)
            //        return null;
            //    return m_Inventory.GetItem(nextItem).ItemAnimName;
            //    //return null;
            //}
            //return currentItem.ItemAnimName;
            return "";
        }


        public override float GetNormalizedTime()
        {
            return m_Animator.GetCurrentAnimatorStateInfo(m_LayerIndex).normalizedTime % 1; ;
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

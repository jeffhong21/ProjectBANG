namespace CharacterController
{
    using UnityEngine;


    public class EquipUnequip : ItemAction
    {

        [SerializeField] protected int equipItemStateIndex = ItemActionID.Equip;
        [SerializeField] protected int unEquipItemStateIndex = ItemActionID.Unequip;


        private bool equipNext = true;
        private int itemSlotIndex = -1;
        [SerializeField] private Item m_equippedItem;

        //protected override void Awake()
        //{
        //    base.Awake();

        //    //EventHandler.RegisterEvent(gameObject, EventIDs.OnAnimatorEquipItem, () => SetActive(true));
        //    //EventHandler.RegisterEvent(m_gameObject, EventIDs.OnAnimatorEquipItemComplete, ()=>CanStopAction); 

        //    //EventHandler.RegisterEvent(gameObject, EventIDs.OnAnimatorUnequipItem, () => SetActive(false));

        //}

        //protected void Start()
        //{
        //    if (m_inventory != null) {

        //    }
        //}


        public override bool CanStartAction()
        {
            if(base.CanStartAction() == false)
            {
                if (m_inventory != null) {
                    for (int number = 0; number < m_inventory.SlotCount; number++) {
                        if (Input.GetKeyDown(number.ToString())) {
                            //itemSlotIndex = Mathf.Clamp(number - 1, 0, m_inventory.SlotCount);
                            itemSlotIndex = number;
                            //Debug.Log("inventory slot " + itemSlotIndex);
                            return true;
                        }
                    }

                    //  None of the number input keys were pressed.  
                    itemSlotIndex = -1;
                }
            }


            return base.CanStartAction();
        }


        public void StartEquipUnequipAction(int index)
        {
            Debug.LogFormat("Inventory Item Slot {0} is {1}", index, m_inventory.GetItem(index) );
            var itemToEquip = m_inventory.EquipItem(index);
            if(itemToEquip != null) {
                itemToEquip.SetActive(true);

                //var stateName = m_animator.GetLayerName(0) + ".Movement.Rifle Movement";
                ////m_animator.CrossFade(stateName, 0.2f, 0, 0);

                //if (m_equippedItem != null) {
                //    //  Unequip
                //    if (itemToEquip == null) {

                //    }

                //    else {

                //    }
                //}
                m_equippedItem = itemToEquip;
                m_animatorMonitor.SetItemID(itemToEquip.AnimatorItemID);
            }
            else {
                if(m_equippedItem != null)
                    m_equippedItem.SetActive(true);
                m_equippedItem = null;
                m_animatorMonitor.SetItemID(0);
            }
            


            m_animatorMonitor.SetMovementSetID(m_equippedItem == null ? 0 : m_equippedItem.AnimatorMovementSetID);

            ActionStarted();
        }


        protected override void ActionStarted()
        {
            //Item currentItem = m_inventory.EquippedItem;
            //Item nextItem;

            ////
            //if (itemSlotIndex > -1)
            //{
            //    nextItem = m_inventory.GetItem(itemSlotIndex);  //  DONOT chanmge what next item is.  We need to know the correct animatorID.
            //    //  If next item is null, do nothing.
            //    if (nextItem == null) {
            //        if (m_Debug) Debug.LogFormat("Item slot {0} contains nothing.", itemSlotIndex);
            //        return;
            //    }

            //    //  Switch the items.  Inform the inventory that it is switching.
            //    if (currentItem != nextItem)
            //    {
            //        //  We want to exit the items statemachine.  We do this by unequipping the current item.
            //        if(currentItem != null) {
            //            m_inventory.UnequipCurrentItem();
            //        }
            //        //  No item is equipped.  Equip the next item.
            //        else{
            //            if (m_Debug) Debug.LogFormat("Equipping {0} at slot index {1} | Current: {2}", nextItem, itemSlotIndex, currentItem);
            //            m_inventory.EquipItem(itemSlotIndex);
            //        }

            //    }
            //    //  Next item is the same as the current item.  unequip current item off.  
            //    else {

            //        if (m_Debug) Debug.LogFormat("Item slot {0} is currently equipped item.", itemSlotIndex);
            //        nextItem = null;
            //        m_inventory.UnequipCurrentItem();
            //    }
            //}
            ////  Alpha keycode wasn't pressed.  So switch to next or prev.
            //else {
            //    nextItem = m_inventory.EquipNextItem(equipNext);
            //    //equipNext = !equipNext;
            //}





            ////  Unequipping current item.  Play unequip animation for current item.
            //if(currentItem != null && nextItem == null) {
            //    m_animatorMonitor.SetItemID(currentItem.AnimatorItemID);
            //    m_animatorMonitor.SetItemStateIndex(ItemActionID.Unequip);
            //}
            ////  Currently has no items equipped.  Play next item equip animation.
            //else if (currentItem == null && nextItem != null) {
            //    m_animatorMonitor.SetItemID(nextItem.AnimatorItemID);
            //    m_animatorMonitor.SetItemStateIndex(ItemActionID.Equip);
            //}
            //else {
            //    m_animatorMonitor.SetItemID(0);
            //    m_animatorMonitor.SetItemStateIndex(0);
            //}

            //m_animatorMonitor.SetItemStateChange();

            Debug.LogFormat("<b>[{0}]</b> ActionStarted", GetType().Name);
        }



        public override bool CanStopAction()
        {
            return Time.time > m_ActionStartTime + 1f;
        }




        protected override void ActionStopped()
        {

        }




    }

}

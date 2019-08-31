namespace CharacterController
{
    using UnityEngine;


    public class EquipUnequip : ItemAction
    {

        [SerializeField] protected int equipItemStateIndex = ItemActionID.Equip;
        [SerializeField] protected int unEquipItemStateIndex = ItemActionID.Unequip;


        private bool equipNext = true;
        private int itemSlotIndex = -1;


        protected override void Awake()
        {
            base.Awake();

            //EventHandler.RegisterEvent(gameObject, EventIDs.OnAnimatorEquipItem, () => SetActive(true));
            //EventHandler.RegisterEvent(m_GameObject, EventIDs.OnAnimatorEquipItemComplete, ()=>CanStopAction); 

            //EventHandler.RegisterEvent(gameObject, EventIDs.OnAnimatorUnequipItem, () => SetActive(false));

        }

        protected void Start()
        {
            if (m_Inventory != null) {

            }
        }


        public override bool CanStartAction()
        {
            if(base.CanStartAction() == false)
            {
                if (m_Inventory != null) {
                    for (int number = 0; number < m_Inventory.SlotCount; number++) {
                        if (Input.GetKeyDown(number.ToString())) {
                            //itemSlotIndex = Mathf.Clamp(number - 1, 0, m_Inventory.SlotCount);
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


        protected override void ActionStarted()
        {
            Item currentItem = m_Inventory.CurrentlyEquippedItem;
            Item nextItem;

            //
            if (itemSlotIndex > -1)
            {
                nextItem = m_Inventory.GetItem(itemSlotIndex);  //  DONOT chanmge what next item is.  We need to know the correct animatorID.
                //  If next item is null, do nothing.
                if (nextItem == null) {
                    if (m_Debug) Debug.LogFormat("Item slot {0} contains nothing.", itemSlotIndex);
                    return;
                }

                //  Switch the items.  Inform the inventory that it is switching.
                if (currentItem != nextItem)
                {
                    //  We want to exit the items statemachine.  We do this by unequipping the current item.
                    if(currentItem != null) {
                        m_Inventory.UnequipCurrentItem();
                    }
                    //  No item is equipped.  Equip the next item.
                    else{
                        if (m_Debug) Debug.LogFormat("Equipping {0} at slot index {1} | Current: {2}", nextItem, itemSlotIndex, currentItem);
                        m_Inventory.EquipItem(itemSlotIndex);
                    }

                }
                //  Next item is the same as the current item.  unequip current item off.  
                else {

                    if (m_Debug) Debug.LogFormat("Item slot {0} is currently equipped item.", itemSlotIndex);
                    nextItem = null;
                    m_Inventory.UnequipCurrentItem();
                }
            }
            //  Alpha keycode wasn't pressed.  So switch to next or prev.
            else {
                nextItem = m_Inventory.EquipNextItem(equipNext);
                //equipNext = !equipNext;
            }





            //  Unequipping current item.  Play unequip animation for current item.
            if(currentItem != null && nextItem == null) {
                m_AnimatorMonitor.SetItemID(currentItem.AnimatorItemID);
                m_AnimatorMonitor.SetItemStateIndex(ItemActionID.Unequip);
            }
            //  Currently has no items equipped.  Play next item equip animation.
            else if (currentItem == null && nextItem != null) {
                m_AnimatorMonitor.SetItemID(nextItem.AnimatorItemID);
                m_AnimatorMonitor.SetItemStateIndex(ItemActionID.Equip);
            }
            else {
                m_AnimatorMonitor.SetItemID(0);
                m_AnimatorMonitor.SetItemStateIndex(0);
            }

            m_AnimatorMonitor.SetItemStateChange();
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

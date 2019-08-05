namespace CharacterController
{
    using UnityEngine;


    public class EquipUnequip : CharacterAction
    {

        bool equipNext = true;
        int itemSlotIndex = -1;


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
                            itemSlotIndex = number;
                            return true;
                        }
                    }
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
                nextItem = m_Inventory.GetItem(itemSlotIndex);

                //  Toggleing current item on and off.  
                if (currentItem == nextItem) {
                    Debug.LogFormat("Item slot {0} is currently equipped item.", itemSlotIndex);
                    nextItem = null;
                    m_Inventory.UnequipCurrentItem();
                }
                //  If next item is null, do nothing.
                else if (nextItem == null) {
                    Debug.LogFormat("Item slot {0} contains nothing.", itemSlotIndex);
                }
                //  Equip the item.
                else {
                    Debug.LogFormat("Equipping {0} at slot index {1} | Current: {2}", nextItem, itemSlotIndex, currentItem);
                    nextItem = m_Inventory.EquipItem(itemSlotIndex);
                }

            }
            else {
                nextItem = m_Inventory.EquipNextItem(equipNext);
                //equipNext = !equipNext;
            }





            //  Unequipping current item.  Play unequip animation for current item.
            if(currentItem != null && nextItem == null) {
                m_Animator.SetInteger(HashID.ItemID, currentItem.AnimatorItemID);
                m_Animator.SetInteger(HashID.ItemStateIndex, 5);
            }
            //  Currently has no items equipped.  Play next item equip animation.
            else if (currentItem == null && nextItem != null) {
                m_Animator.SetInteger(HashID.ItemID, nextItem.AnimatorItemID);
                m_Animator.SetInteger(HashID.ItemStateIndex, 4);
            }
            //  Equippinmg next item.  Unequipp current item, than equip next item.
            else if (currentItem != null && nextItem != null) {
                m_Animator.SetInteger(HashID.ItemID, nextItem.AnimatorItemID);
                m_Animator.SetInteger(HashID.ItemStateIndex, 5);
            }
            else {
                m_Animator.SetInteger(HashID.ItemID, 0);
                m_Animator.SetInteger(HashID.ItemStateIndex, 0);
            }

            m_Animator.SetTrigger(HashID.ItemStateIndexChange);
        }


        public override bool CanStopAction()
        {
            return Time.time > m_ActionStartTime + 1f;
        }

        protected override void ActionStopped()
        {

            itemSlotIndex = -1;
        }


        //protected void SetActive(bool active)
        //{
        //    Debug.LogFormat("OnAnimator set {0} active ({1}", nextItem, active);
        //    if (nextItem != null) nextItem.SetActive(active);
        //}




    }

}

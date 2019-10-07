namespace CharacterController
{
    using UnityEngine;
    using System.Collections;

    public class EquipUnequip : ItemAction
    {
        //public override int ItemStateID
        //{
        //    get { return m_ItemStateID = ItemActionID.Aim; }
        //    set { m_ItemStateID = value; }
        //}

        int m_equipStateID = ItemActionID.Equip;
        int m_unequipStateID = ItemActionID.Unequip;

        [SerializeField] protected int equipItemStateIndex = ItemActionID.Equip;
        [SerializeField] protected int unEquipItemStateIndex = ItemActionID.Unequip;


        private bool equipNext = true;
        private int itemSlotIndex = -1;

        [SerializeField] private Item m_currentItem;
        [SerializeField] private Item m_nextItem;
        private int m_index;
        private bool m_switching;

        public void StartEquipUnequipAction(int index)
        {

            EventHandler.RegisterEvent(m_gameObject, "OnItemUnequip", OnItemUnequip);
            EventHandler.RegisterEvent(m_gameObject, "OnItemEqui[p", OnItemEquip);
            EventHandler.RegisterEvent(m_gameObject, "OnItemUnequipComplete", OnItemUnequipComplete);
            EventHandler.RegisterEvent(m_gameObject, "OnItemEquipComplete", OnItemEquipComplete);

            m_index = index;
            m_currentItem = m_inventory.EquippedItem;
            m_nextItem = m_inventory.GetItem(index);

            //Debug.LogFormat("Inventory Item Slot {0} is {1}\nEquipped item is {2}", index, m_inventory.GetItem(index), m_currentItem);
            if (m_nextItem != null) {
                ActionStarted();
            }


        }


        protected override void ActionStarted()
        {
            m_switching = false;
            m_inventory.EquipItem(m_index);


            //m_animatorMonitor.ChangeAnimatorState(this, unequipStateName);
            //m_animatorMonitor.SetMovementSetID(m_currentItem == null ? 0 : m_currentItem.AnimatorMovementSetID);
            if(m_currentItem != null) {

                int layerIndex = 1;
                string unequipStateName = m_currentItem != null ? m_currentItem.unequipStateName : "No item equipped.";
                unequipStateName = m_animator.GetLayerName(layerIndex) + "." + unequipStateName;
                int hash = Animator.StringToHash(unequipStateName);

                if (m_animator.HasState(layerIndex, hash)) {
                    m_animator.CrossFade(hash, 0.2f, layerIndex, 0);
                    //Debug.LogFormat("<b>Destination State: {0}</b>.  Has state: {1}", unequipStateName, m_animator.HasState(1, hash));

                    m_switching = true;
                }
            }
            else if(m_currentItem == null && m_nextItem != null) {

                int layerIndex = 1;
                string equippedStateName = m_nextItem.equipStateName;
                equippedStateName = m_animator.GetLayerName(layerIndex) + "." + equippedStateName;
                int hash = Animator.StringToHash(equippedStateName);

                if (m_animator.HasState(layerIndex, hash)) {
                    m_animator.CrossFade(hash, 0.2f, layerIndex, 0);
                    //Debug.LogFormat("<b>Destination State: {0}</b>.  Has state: {1}", equippedStateName, m_animator.HasState(1, hash));


                    m_switching = true;
                }
            }
            else {
                Debug.LogFormat("<b>No item equipped: {0}</b>.", "");
            }



        }



        //public override bool CanStopAction()
        //{
        //    return Time.time > m_ActionStartTime + 1f;
        //}


        protected override void ActionStopped()
        {
            EventHandler.UnregisterEvent(m_gameObject, "OnItemUnequip", OnItemUnequip);
            EventHandler.UnregisterEvent(m_gameObject, "OnItemEqui[p", OnItemEquip);
            EventHandler.UnregisterEvent(m_gameObject, "OnItemUnequipComplete", OnItemUnequipComplete);
            EventHandler.UnregisterEvent(m_gameObject, "OnItemEquipComplete", OnItemEquipComplete);
        }



        protected bool GetItemDestinationState(string stateName, int layer, out int hash)
        {
            string layerName = m_animator.GetLayerName(layer);
            string destinationStateName = layerName + "." + stateName;
            hash = Animator.StringToHash(destinationStateName);
            if (m_animator.HasState(layer, hash)) {
                return true;
            }
            return false;
        }



        protected void OnItemUnequip()
        {
            Debug.LogFormat("<b><color=magenta>**OnItemUnequip Animation event has been called</color></b>");

            m_inventory.UnequipCurrentItem();
            if(m_nextItem != null) {
                int layer = 1;
                if(GetItemDestinationState(m_nextItem.equipStateName, layer, out int hash)) {
                    m_animator.CrossFade(hash, 0.2f, layer, 0);
                }


            }

            Debug.Break();
        }

        protected void OnItemUnequipComplete()
        {
            Debug.LogFormat("<b><color=magenta>**OnItemUnequip Animation event has been called</color></b>");
            //Debug.Break();
        }

        protected void OnItemEquip()
        {
            Debug.LogFormat("<b><color=blue>**OnItemEquip Animation event has been called</color></b>");

            m_inventory.EquipItem(m_index);
            //Debug.Break();
        }

        protected void OnItemEquipComplete()
        {
            Debug.LogFormat("<b><color=blue>**OnItemEquip Animation event has been called</color></b>");
            //Debug.Break();
        }

        protected IEnumerator UnequipItem(Item activeItem)
        {


            yield return null;
        }


    }

}



//protected override void ActionStarted()
//{
//    if (m_nextItem != null) {
//        //m_nextItem.SetActive(true);

//        //var stateName = m_animator.GetLayerName(0) + ".Movement.Rifle Movement";
//        ////m_animator.CrossFade(stateName, 0.2f, 0, 0);

//        //if (m_currentItem != null) {
//        //    //  Unequip
//        //    if (itemToEquip == null) {

//        //    }

//        //    else {

//        //    }
//        //}
//        //m_currentItem = m_nextItem;
//        //m_animatorMonitor.SetItemID(m_nextItem.AnimatorItemID);
//    }
//    else {
//        if (m_currentItem != null)
//            m_currentItem.SetActive(true);
//        m_currentItem = null;
//        m_animatorMonitor.SetItemID(0);
//    }



//    m_animatorMonitor.SetMovementSetID(m_currentItem == null ? 0 : m_currentItem.AnimatorMovementSetID);






//    //Item currentItem = m_inventory.EquippedItem;
//    //Item nextItem;

//    ////
//    //if (itemSlotIndex > -1)
//    //{
//    //    nextItem = m_inventory.GetItem(itemSlotIndex);  //  DONOT chanmge what next item is.  We need to know the correct animatorID.
//    //    //  If next item is null, do nothing.
//    //    if (nextItem == null) {
//    //        if (m_Debug) Debug.LogFormat("Item slot {0} contains nothing.", itemSlotIndex);
//    //        return;
//    //    }

//    //    //  Switch the items.  Inform the inventory that it is switching.
//    //    if (currentItem != nextItem)
//    //    {
//    //        //  We want to exit the items statemachine.  We do this by unequipping the current item.
//    //        if(currentItem != null) {
//    //            m_inventory.UnequipCurrentItem();
//    //        }
//    //        //  No item is equipped.  Equip the next item.
//    //        else{
//    //            if (m_Debug) Debug.LogFormat("Equipping {0} at slot index {1} | Current: {2}", nextItem, itemSlotIndex, currentItem);
//    //            m_inventory.EquipItem(itemSlotIndex);
//    //        }

//    //    }
//    //    //  Next item is the same as the current item.  unequip current item off.  
//    //    else {

//    //        if (m_Debug) Debug.LogFormat("Item slot {0} is currently equipped item.", itemSlotIndex);
//    //        nextItem = null;
//    //        m_inventory.UnequipCurrentItem();
//    //    }
//    //}
//    ////  Alpha keycode wasn't pressed.  So switch to next or prev.
//    //else {
//    //    nextItem = m_inventory.EquipNextItem(equipNext);
//    //    //equipNext = !equipNext;
//    //}





//    ////  Unequipping current item.  Play unequip animation for current item.
//    //if(currentItem != null && nextItem == null) {
//    //    m_animatorMonitor.SetItemID(currentItem.AnimatorItemID);
//    //    m_animatorMonitor.SetItemStateIndex(ItemActionID.Unequip);
//    //}
//    ////  Currently has no items equipped.  Play next item equip animation.
//    //else if (currentItem == null && nextItem != null) {
//    //    m_animatorMonitor.SetItemID(nextItem.AnimatorItemID);
//    //    m_animatorMonitor.SetItemStateIndex(ItemActionID.Equip);
//    //}
//    //else {
//    //    m_animatorMonitor.SetItemID(0);
//    //    m_animatorMonitor.SetItemStateIndex(0);
//    //}

//    //m_animatorMonitor.SetItemStateChange();

//    Debug.LogFormat("<b>[{0}]</b> ActionStarted", GetType().Name);
//}
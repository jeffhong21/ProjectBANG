namespace CharacterController
{
    using UnityEngine;


    public class EquipUnequip : CharacterAction
    {

        protected Item equippedITem;


        bool next = true;

        protected override void ActionStarted()
        {
            var item = m_Inventory.EquipNextItem(next);
            next = !next;

            m_Animator.SetInteger(HashID.ItemID, item.AnimatorItemID);
            m_Animator.SetInteger(HashID.ItemStateIndex, 0);
            m_Animator.SetTrigger(HashID.ItemStateIndexChange );
        }


        public override bool CanStopAction()
        {
            return Time.time > m_ActionStartTime + 1f;
        }


    }

}

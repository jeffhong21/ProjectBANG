namespace CharacterController
{
    using UnityEngine;


    public class Aim : ItemAction
    {

        protected int movementsetID;



        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if (base.CanStartAction())
            {
                return m_Controller.CanAim;
            }
            return false;
        }


        protected override void ActionStarted()
        {
            movementsetID = m_inventory.CurrentlyEquippedItem == null ? 0 : m_inventory.CurrentlyEquippedItem.AnimatorMovementSetID;
            //m_animatorMonitor.SetItemID(GetItemID(), m_ItemStateID);
            m_Controller.Aiming = true;
            m_animator.SetBool(HashID.Aiming, m_Controller.Aiming);

            m_animatorMonitor.SetActionID(m_ActionID);
            m_animatorMonitor.SetMovementSetID(movementsetID);


            EventHandler.ExecuteEvent(m_gameObject, EventIDs.OnAimActionStart, m_Controller.Aiming);

        }


        protected override void ActionStopped()
        {

            //m_animatorMonitor.SetItemID(GetItemID(), 0);
            m_Controller.Aiming = false;
            m_animator.SetBool(HashID.Aiming, m_Controller.Aiming);
            EventHandler.ExecuteEvent(m_gameObject, EventIDs.OnAimActionStart, m_Controller.Aiming);
        }



        //public override string GetDestinationState(int layer)
        //{
        //    if (layer == 0){
        //        return m_StateName;
        //    }
        //    return "";
        //}



        public override bool IsConcurrentAction()
        {
            return true;
        }





    }

}

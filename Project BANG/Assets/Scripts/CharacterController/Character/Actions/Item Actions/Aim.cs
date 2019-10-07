namespace CharacterController
{
    using UnityEngine;


    public class Aim : ItemAction
    {

        public override int ItemStateID {
            get { return m_ItemStateID = ItemActionID.Aim; }
            set { m_ItemStateID = value; }
        }

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
            //movementsetID = m_inventory.EquippedItem == null ? 0 : m_inventory.EquippedItem.movementSetID;
            ////m_animatorMonitor.SetItemID(GetItemID(), m_ItemStateID);
            //m_Controller.Aiming = true;
            //m_animator.SetBool(HashID.Aiming, m_Controller.Aiming);

            //m_animatorMonitor.SetActionID(m_ActionID);
            //m_animatorMonitor.SetMovementSetID(movementsetID);

            Debug.LogFormat("<b>Aiming with {0}</b>.", m_inventory.EquippedItem);


            m_animatorMonitor.SetAiming(true);

            EventHandler.ExecuteEvent(m_gameObject, EventIDs.OnAimActionStart, m_Controller.Aiming);

            CameraController.Instance.SetCameraState("AIM");
        }


        protected override void ActionStopped()
        {
            CameraController.Instance.SetCameraState("DEFAULT");

            m_animatorMonitor.SetAiming(false);
            //m_animatorMonitor.SetItemID(GetItemID(), 0);
            m_Controller.Aiming = false;
            m_animator.SetBool(HashID.Aiming, m_Controller.Aiming);
            EventHandler.ExecuteEvent(m_gameObject, EventIDs.OnAimActionStart, m_Controller.Aiming);
        }





        public override bool IsConcurrentAction()
        {
            return true;
        }





    }

}

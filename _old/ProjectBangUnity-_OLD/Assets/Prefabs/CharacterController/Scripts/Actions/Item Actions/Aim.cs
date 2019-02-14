namespace CharacterController
{
    using UnityEngine;


    public class Aim : CharacterAction
    {

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
            EventHandler.ExecuteEvent(m_GameObject, "OnAimActionStart", m_Aiming);

            //Debug.LogFormat("DefaultState {0}", m_Inventory.GetCurrentItem(m_Inventory.EquippedItemType).DefaultStates);
            //Debug.LogFormat("Aim {0}", m_Inventory.GetCurrentItem(m_Inventory.EquippedItemType).AimStates);
            //Debug.LogFormat("Equip {0}", m_Inventory.GetCurrentItem(m_Inventory.EquippedItemType).EquipStates);
            //Debug.LogFormat("Unequip {0}", m_Inventory.GetCurrentItem(m_Inventory.EquippedItemType).UnequipStates);
        }


        protected override void ActionStopped()
        {
            m_Aiming = false;
            EventHandler.ExecuteEvent(m_GameObject, "OnAimActionStart", m_Aiming);
        }


        public override string GetDestinationState(int layer)
        {
            return "";
        }


        public override bool UpdateAnimator()
        {
            m_Animator.SetBool(HashID.Aiming, m_Aiming);

            return true;
        }

        public override bool IsConcurrentAction()
        {
            return true;
        }
    }

}

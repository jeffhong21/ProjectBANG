namespace CharacterController
{
    using UnityEngine;


    public class Shoot : CharacterAction
    {
        [SerializeField]
        protected ItemObject m_EquipedItem;


        //
        // Methods
        //

        private void Reset()
        {
            m_InputName = "Fire1";
            m_StartType = ActionStartType.ButtonDown;
            m_StopType = ActionStopType.Automatic;
        }


        //  Checks if action can be started.
        public override bool CanStartAction()
        {
            
            if (m_StartType == ActionStartType.Manual){
                if(m_Inventory != null && m_Controller.Aiming){
                    if(m_Inventory.EquippedItemType != null){
                        return true;
                    }
                }
            }
            else if (m_StartType == ActionStartType.ButtonDown){
                if (Input.GetButtonDown(m_InputName)){
                    if (m_Inventory != null && m_Controller.Aiming){
                        if (m_EquipedItem == null) 
                            m_EquipedItem = m_Inventory.GetCurrentItem(m_Inventory.EquippedItemType);
                        if (m_Inventory.EquippedItemType != null){
                            return true;
                        }
                    }
                }
            }



            return false;
        }



        protected override void ActionStarted()
        {
            m_Inventory.UseItem(m_Inventory.EquippedItemType, 1);
            //Debug.LogFormat("{0} action.  Shooting a {1} | Iventory: {2} |", GetType().Name, m_Inventory.EquippedItemType, m_Inventory);
        }

        protected override void ActionStopped()
        {

        }


        public override string GetDestinationState(int layer)
        {
            //return "Rifle.Shoot";
            return "";
        }



        private void OnAimActionStart(bool aim)
        {
            Debug.LogFormat("{0} Recieving Aim event", GetType().Name);
        }


        public override bool IsConcurrentAction()
        {
            return true;
        }


        private void OnDestroy()
        {
            //EventHandler.UnregisterEvent<bool>(this, "OnAimActionStart", OnAimActionStart);
        }
    }

}
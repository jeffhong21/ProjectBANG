namespace CharacterController
{
    using UnityEngine;


    public class Shoot : CharacterAction
    {



		//
		// Methods
		//
		public override bool CanStartAction()
		{
            if(m_Controller.Aiming)
                return base.CanStartAction();
            return false;
		}


		protected override void ActionStarted()
        {
            m_Inventory.UseItem(m_Inventory.EquippedItemType, 1);
            //Debug.Log("Shooting action started");
        }

        protected override void ActionStopped()
        {
            //Debug.Log("Shooting action done");
        }
    }

}
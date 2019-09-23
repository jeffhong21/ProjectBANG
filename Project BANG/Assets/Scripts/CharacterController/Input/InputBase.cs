using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace CharacterController.CharacterInput
{
    public abstract class InputBase
    {
        protected CharacterLocomotion m_controller;
        protected Inventory m_inventory;


        public InputBase() { }

        public InputBase(CharacterLocomotion controller) => OnInitialize(controller);

        public InputBase(CharacterLocomotion controller, Inventory inventory) => OnInitialize(controller, inventory);



        private void OnInitialize(CharacterLocomotion controller)
        {
            m_controller = controller;
        }


        private void OnInitialize(CharacterLocomotion controller, Inventory inventory)
        {
            m_inventory = inventory;
            OnInitialize(controller);
        }

        public abstract void Initialize(CharacterLocomotion controller);


        //public abstract bool HandleInput();
    }







    public class InventoryHandler : InputBase
    {

        public ItemAction useAction { get; private set; }
        public ItemAction aimAction { get; private set; }
        public ItemAction equipUnequipAction { get; private set; }




        public override void Initialize(CharacterLocomotion controller)
        {
            useAction = controller.GetAction<Use>();
            aimAction = controller.GetAction<Aim>();
            equipUnequipAction = controller.GetAction<EquipUnequip>();
        }


    }


    public class ControllerHandler : InputBase
    {




        public override void Initialize(CharacterLocomotion controller)
        {

        }


    }


}




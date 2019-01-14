namespace CharacterController
{
    using UnityEngine;


    public class Weapon : Item
    {
        //
        // Fields
        //
        protected string m_UseInputName = "Fire1";



        //
        // Methods
        //
        public override void Initialize(Inventory inventory)
        {
            m_Inventory = inventory;
        }

        protected override void ItemDeactivated()
        {

        }

        protected override void OnAim(bool aim)
        {

        }



        public virtual bool TryUse()
        {

            return true;
        }


        public virtual bool CanUse()
        {
            return true;
        }


        public virtual bool InUse()
        {
            return false;
        }


        public virtual void TryStopUse()
        {

        }

    }

}
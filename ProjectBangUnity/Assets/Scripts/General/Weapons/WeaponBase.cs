namespace Bang
{
    using UnityEngine;
    using System;

    public abstract class WeaponBase : MonoBehaviour
    {
        protected GameObject owner;



        public void SetOwner(GameObject owner)
        {
            this.owner = owner;
        }


        public abstract void ExecuteAttack(Vector3 target);

        public abstract void SecondaryFunction();
    }
}



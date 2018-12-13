namespace Bang
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;


    public class PlayerController : ActorController
    {
        //
        //  Fields
        //
        public PlayerStats stats;

        private PlayerInputHandler input;


        public event Action<int, int> CurrentAmmoEvent;

        public event Action<float> DamageEvent;

        public event Action<string> EquipWeaponEvent;




        //
        //  Methods
        //
        protected override void Awake()
        {
            base.Awake();
            input = GetComponent<PlayerInputHandler>();

        }


        protected override void ExecuteUpdate(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.T)){
                Debug.Break();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                EquipWeapon(WeaponNameIDs.Revolver_01);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                EquipWeapon(WeaponNameIDs.Rifle_01);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                EquipWeapon(WeaponNameIDs.Shotgun);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                EquipWeapon(WeaponNameIDs.Revolver_02);
            }
        }


        protected override void ExecuteFixedUpdate(float deltaTime)
        {
            
        }



        protected override void OnEquipWeapon(ShootableWeapon weapon)
        {
            CurrentAmmoEvent?.Invoke(weapon.CurrentAmmo, weapon.MaxAmmo);
            EquipWeaponEvent?.Invoke(weapon.NameID);
        }


        protected override void OnShootWeapon()
        {
            CurrentAmmoEvent(weapon.CurrentAmmo, weapon.MaxAmmo);
        }


        protected override void OnReload()
        {
            CurrentAmmoEvent(weapon.CurrentAmmo, weapon.MaxAmmo);
        }


        protected override void OnTakeDamage(Vector3 hitDirection)
        {
            //AnimHandler.PlayTakeDamage(hitDirection);
            DamageEvent(Health.CurrentHealth);
        }


        protected override void OnDeath()
        {
            DisableControls();
        }


        public override void EnableControls()
        {
            input.enabled = true;
        }

        public override void DisableControls()
        {
            input.enabled = false;
        }





        //public void OnDrawGizmosSelected(){
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawLine(AimOrigin, _shootTarget);
        //}






    }
}



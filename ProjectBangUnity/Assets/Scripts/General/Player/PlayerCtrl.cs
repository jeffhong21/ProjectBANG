namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections;



    public class PlayerCtrl : ActorCtrl, IHasFirearm
    {




        protected override void Awake()
        {
            base.Awake();
        }

        protected virtual void Start()
        {
            if (_equippedFirearm == null)
            {
                if (actorBody.RightHand != null)
                    EquipGun(gm.defaultWeapon, actorBody.RightHand);
                else if(actorBody.LeftHand != null)
                    EquipGun(gm.defaultWeapon, actorBody.LeftHand);
                else
                    Debug.Log("No weapon holder");
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }


        protected override void OnDisable()
        {
            base.OnDisable();
        }






        public override void EquipGun(FirearmBase fireArm, Transform location)
        {
            base.EquipGun(fireArm, location);

            HUDState.UpdateWeapon(fireArm.GetType().Name);
            HUDState.UpdateAmmo(equippedFirearm.currentAmmo, equippedFirearm.maxAmmo);
        }


        public override void FireWeapon(Vector3 target)
        {
            base.FireWeapon(target);

            HUDState.UpdateAmmo(equippedFirearm.currentAmmo, equippedFirearm.maxAmmo);
        }


        public override void Reload()
        {
            canShoot = false;
            equippedFirearm.Reload();
            canShoot = true;
            HUDState.UpdateAmmo(equippedFirearm.currentAmmo, equippedFirearm.maxAmmo);
        }


        public virtual void LookAt(Vector3 target)
        {
            Vector3 heightCorrectedPoint = new Vector3(target.x, transform.position.y, target.z);
            transform.LookAt(heightCorrectedPoint);
        }

    }
}



namespace Bang
{
    using UnityEngine;
    using System;

    public class InventoryHandler : MonoBehaviour
    {
        [SerializeField]
        protected WeaponController _equippedFirearm;
        [SerializeField]
        protected ActorBody actorBody = new ActorBody();


        public WeaponController equippedFirearm
        {
            get { return _equippedFirearm; }
            set { _equippedFirearm = value; }
        }




        protected virtual void Awake()
        {
            //gm = GameManager.instance;
            //if (gm == null)
            //{
            //    gm = FindObjectOfType<GameManager>();
            //    if (gm == null)
            //        Debug.LogWarning("Still can't find GameManagerInstance.");
            //}
        }


        protected virtual void Start()
        {
            //if (_equippedFirearm == null)
            //{
            //    if (actorBody.RightHand != null)
            //        EquipGun(gm.defaultWeapon, actorBody.RightHand);
            //    else if (actorBody.LeftHand != null)
            //        EquipGun(gm.defaultWeapon, actorBody.LeftHand);
            //    else
            //        Debug.Log("No weapon holder");
            //}

            EquipGun(equippedFirearm, actorBody.RightHand);
        }



        public virtual void EquipGun(WeaponController fireArm, Transform location)
        {
            equippedFirearm = Instantiate(fireArm, location.position, location.rotation) as WeaponController;
            equippedFirearm.transform.parent = location;

            //HUDState.UpdateWeapon(fireArm.GetType().Name);
            //HUDState.UpdateAmmo(equippedFirearm.currentAmmo, equippedFirearm.maxAmmo);
        }




        [Serializable]
        public class ActorBody
        {
            [SerializeField]
            private Transform _leftHand;
            [SerializeField]
            private Transform _rightHand;

            public Transform LeftHand
            {
                get { return _leftHand; }
            }

            public Transform RightHand
            {
                get { return _rightHand; }
            }
        }

    }
}



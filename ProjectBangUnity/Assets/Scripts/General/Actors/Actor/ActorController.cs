namespace Bang
{
    using UnityEngine;

    /// <summary>
    /// Abstract class for Actors.
    /// </summary>
    public abstract class ActorController : EntityBase, IActorController
    {
        private ActorHealth health;
        private AnimationHandler animHandler;

        public float delta;
        public Transform lookTransform;                 //  Optionally specify a transform to determine where to check the line of sight from
        public Gun weapon;
        public Transform weaponHolder;


        private float aimHeight = 1.25f;
        private Vector3 aimPosition;



        public AnimationHandler AnimHandler{
            get { return animHandler; }
        }

        public ActorHealth Health{
            get { return health; }
        }

        public Vector3 AimOrigin{
            get{
                if(lookTransform != null){
                    return lookTransform.position;
                }
                Vector3 origin = transform.position;// + transform.forward;
                origin.y = aimHeight;
                return origin;
            }
        }

        public Vector3 AimPosition{
            get{
                //aimPosition = transform.position + transform.forward * 15;
                aimPosition.y = aimHeight;
                return aimPosition;
            }
            set{
                aimPosition = value;
            }
        }



        //
        //  Methods
        //
        protected virtual void Awake()
        {
            animHandler = GetComponent<AnimationHandler>();
            health = GetComponent<ActorHealth>();

        }


		public void OnEnable()
		{
            AimPosition = transform.position + transform.forward * 15;
		}



		public void Init(ActorManager manager)
        {
            EquipWeapon(WeaponNameIDs.Rifle_01);
        }


        public virtual void EquipWeapon(string weaponID)
        {
            
            Weapon w = GameManager.instance.WeaponManager.GetWeapon(weaponID);
            //weapon = Instantiate(w.prefab, weaponHolder.position, weaponHolder.rotation, weaponHolder);
            weapon = Instantiate(w.prefab);
            weapon.transform.parent = weaponHolder.parent;
            weapon.transform.position = weaponHolder.position;
            weapon.transform.rotation = weaponHolder.rotation;

            Debug.LogFormat("{0} quipping weapon.\n Position: {1}\n Rotation: {2}", transform.name, weapon.transform.localPosition, weapon.transform.localRotation);

            animHandler.EquidWeapon(weapon.MainHandIK, weapon.OffhandIK);
            weapon.Init(this, w.ammo, w.maxAmmo);
        }



        //
        //  Abstract
        //

        public abstract void Death();

        public abstract void DisableControls();

        public abstract void EnableControls();

    }
}



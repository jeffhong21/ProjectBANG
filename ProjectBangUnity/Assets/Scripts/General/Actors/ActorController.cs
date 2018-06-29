namespace Bang
{
    using UnityEngine;


    public abstract class ActorController : MonoBehaviour, IActorController
    {
        protected GameManager gm = GameManager.instance;

        //
        //  Fields
        //
        public float moveSpeed = 6f;
        public float rollSpeed = 10f;
        public float rollDistance = 10f;
        public float idleTimeout = 5f;
        public bool canAttack;
        public bool isMoving;

        public FirearmBase weapon;
        public FirearmBase defaultWeapon;
        public Transform weaponHolder;
        public float aimHeight = 0.75f;



        protected Camera playerCamera;
        protected ActorHealth health;
        protected InventoryHandler inventory;
        protected AnimationHandler animHandler;
        protected Renderer[] renderers;                 // References used to make sure Renderers are reset properly. 
        protected float idleTimer;                      // Used to count up to player considering a random idle.


        //
        //  Properties
        //
        public Vector3 position
        {
            get { return this.transform.position; }
            set { this.transform.position = value; }
        }

        public Quaternion rotation
        {
            get { return this.transform.rotation; }
            set { this.transform.rotation = value; }
        }


        //
        //  Methods
        //
        protected virtual void Awake()
        {
            animHandler = GetComponent<AnimationHandler>();
            inventory = GetComponent<InventoryHandler>();
        }


        protected virtual void OnEnable()
        {
            renderers = GetComponentsInChildren<Renderer>();

            EquipWeapon(defaultWeapon, weaponHolder);
        }


        protected virtual void OnDisable()
        {
            //for (int i = 0; i < renderers.Length; ++i)
            //{
            //    renderers[i].enabled = true;
            //}
        }


        public virtual void EquipWeapon(FirearmBase weapon, Transform location)
        {
            this.weapon = Instantiate(weapon, location.position, location.rotation, location) as FirearmBase;
        }


        public virtual void FireWeapon(Vector3 target)
        {
            weapon.Shoot(target);
        }


        public virtual void Reload()
        {
            //canShoot = false;
            //equippedFirearm.Reload();
            //canShoot = true;
            //HUDState.UpdateAmmo(equippedFirearm.currentAmmo, equippedFirearm.maxAmmo);
        }


        //
        //  Abstract
        //

        public abstract void DisableControls();

        public abstract void EnableControls();
    }
}



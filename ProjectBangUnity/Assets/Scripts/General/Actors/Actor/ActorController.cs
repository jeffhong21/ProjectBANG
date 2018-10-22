namespace Bang
{
    using UnityEngine;

    /// <summary>
    /// Abstract class for Actors.
    /// </summary>
    public abstract class ActorController : EntityBase, IActorController
    {
        public float delta;
        public Transform lookTransform;                 //  Optionally specify a transform to determine where to check the line of sight from
        public WeaponController weapon;
        public WeaponController defaultWeapon;
        public Transform weaponHolder;


        private ActorHealth health;
        private AnimationHandler animHandler;


        public AnimationHandler AnimHandler{
            get { return animHandler; }
        }

        public ActorHealth Health{
            get { return health; }
        }

        //
        //  Methods
        //
        protected virtual void Awake()
        {
            animHandler = GetComponent<AnimationHandler>();
            health = GetComponent<ActorHealth>();
        }

        protected virtual void OnEnable()
        {
            EquipWeapon(defaultWeapon);
        }


        protected virtual void OnDisable()
        {
            //for (int i = 0; i < renderers.Length; ++i){
            //    renderers[i].enabled = true;
            //}
        }


        public void Init(ActorManager manager)
        {
            
        }


        public virtual void EquipWeapon(WeaponController weapon)
        {
            this.weapon = Instantiate(weapon, weaponHolder.position, weaponHolder.rotation, weaponHolder) as WeaponController;
        }



        //
        //  Abstract
        //

        public abstract void Death();

        public abstract void DisableControls();

        public abstract void EnableControls();
    }
}



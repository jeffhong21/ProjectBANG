namespace Bang
{
    using System;
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Abstract class for Actors.
    /// </summary>
    public abstract class ActorController : EntityBase, IActorController
    {
        //
        //  Fields
        //
        public Gun weapon;
        public Transform weaponHolder;
        [SerializeField]
        private ActorStates states;


        private ActorHealth health;
        private AnimationHandler animHandler;
        private Rigidbody myRigidbody;
        private CapsuleCollider controllerCollider;

        private float delta;
        private float aimHeight = 1.25f;
        private Vector3 aimPosition;
        protected float shootingCooldown;
        protected float reloadCooldown;


        public event Action<GameObject> OnDeathEvent;


        //
        //  Properties
        //
        public AnimationHandler AnimHandler{
            get { return animHandler; }
        }

        public ActorHealth Health{
            get { return health; }
        }


        public ActorStates States{
            get { return states; }
        }

        public Vector3 AimOrigin{
            get{
                Vector3 origin = transform.position;// + transform.forward;
                origin.y = aimHeight;
                return origin;
            }
        }

        public Vector3 AimPosition{
            get{
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
            myRigidbody = GetComponent<Rigidbody>();
            controllerCollider = GetComponent<CapsuleCollider>();
            states = new ActorStates();

            InitializeCoverMarkers();
        }


        protected void OnEnable()
		{
            AimPosition = transform.position + transform.forward * 15;
            States.CanShoot = true;

            myRigidbody.isKinematic = false;
            controllerCollider.enabled = true;
		}



		public void InitializeActor(ActorManager manager)
        {
            EquipWeapon(WeaponNameIDs.Rifle_01);
        }



		private void Update()
		{
            delta = Time.deltaTime;

            HandleShootingCooldown(delta);
            HandleReloadCooldown(delta);


            ExecuteUpdate(delta);
		}


		private void FixedUpdate()
		{
            delta = Time.fixedDeltaTime;

            if (weapon != null){
                if(States.IsReloading == false){
                    weapon.transform.LookAt(AimPosition);
                }
            }


            ExecuteFixedUpdate(delta);
		}


        private void HandleShootingCooldown(float time)
        {
            if (shootingCooldown > 0){
                shootingCooldown -= time;
                if (shootingCooldown <= 0){
                    States.CanShoot = true;
                }
            }
        }

        private void HandleReloadCooldown(float time)
        {
            if (reloadCooldown > 0){
                reloadCooldown -= time;
                if (reloadCooldown <= 0){
                    States.IsReloading = false;
                    AnimHandler.PlayReload(States.IsReloading);
                }
            }
        }





		public virtual void EquipWeapon(string weaponID)
        {
            
            Weapon w = GameManager.instance.WeaponManager.GetWeapon(weaponID);
            //weapon = Instantiate(w.prefab, weaponHolder.position, weaponHolder.rotation, weaponHolder);

            //weapon = Instantiate(w.prefab);
            //weapon.transform.parent = AnimHandler.rightHandState.hand;
            //weapon.transform.position = AnimHandler.rightHandState.hand.position;
            //weapon.transform.rotation = AnimHandler.rightHandState.hand.rotation;

            weapon = Instantiate(w.prefab);
            weapon.transform.parent = weaponHolder.transform;
            weapon.transform.position = weaponHolder.position;
            weapon.transform.rotation = weaponHolder.rotation;

            AnimHandler.EquidWeapon(weapon.MainHandIK, weapon.OffhandIK);
            weapon.Init(this, weaponID, w.ammo, w.maxAmmo);

            OnEquipWeapon(weapon);
        }


        public void ShootWeapon(Vector3 target)
        {
            if(States.CanShoot)
            {
                AimPosition = target;
                //Debug.DrawLine(weapon.ProjectileSpawn.position, weapon.ProjectileSpawn.position + (weapon.ProjectileSpawn.forward * 10), Color.red, 1f);
                //Debug.DrawLine(weapon.ProjectileSpawn.position, AimPosition, Color.magenta, 1f);
                OnShootWeapon();
                shootingCooldown = weapon.Cooldown;
                States.CanShoot = false;
            }
        }


        public void Reload()
        {
            if (weapon == null) return;

            //  Get the amount of ammo needed to reload.
            int ammoToReload = weapon.MaxAmmo - weapon.CurrentAmmo;
            //  Subtract that ammo amount from inventory.

            //  If there's enough ammo, tan reload.

            reloadCooldown = 2f;
            States.IsReloading = true;
            AnimHandler.PlayReload(States.IsReloading);

            weapon.Reload(2f);
            //  Add it to the weapon current ammo.
            weapon.CurrentAmmo += ammoToReload;

            OnReload();
        }


        public void TakeDamage(Vector3 hitLocation, Vector3 hitDirection, GameObject attacker)
        {
            myRigidbody.AddForce(hitDirection, ForceMode.Impulse);
            OnTakeDamage(hitDirection);
        }


        public void Death(Vector3 position, Vector3 force, GameObject attacker)
        {
            if (OnDeathEvent != null){
                OnDeathEvent?.Invoke(attacker);
            }
            else{
                Debug.Log("Can't trigger OnDeathEvent");
            }

            myRigidbody.isKinematic = true;
            controllerCollider.enabled = false;
            //states.IsDead = true;

            OnDeath();
        }


        //
        //  Abstract
        //

        protected abstract void ExecuteUpdate(float deltaTime);

        protected abstract void ExecuteFixedUpdate(float deltaTime);

        protected abstract void OnEquipWeapon(Gun weapon);

        protected abstract void OnShootWeapon();

        protected abstract void OnReload();

        protected abstract void OnTakeDamage(Vector3 hitDirection);

        protected abstract void OnDeath();

        public abstract void DisableControls();

        public abstract void EnableControls();


        # region Cover 

        protected Transform leftHelper;
        protected Transform rightHelper;

        private void InitializeCoverMarkers()
        {
            leftHelper = new GameObject().transform;
            leftHelper.name = "Left cover Helper";
            leftHelper.parent = transform;
            leftHelper.localPosition = Vector3.zero;
            leftHelper.localEulerAngles = Vector3.zero;

            rightHelper = new GameObject().transform;
            rightHelper.name = "Right cover Helper";
            rightHelper.parent = transform;
            rightHelper.localPosition = Vector3.zero;
            rightHelper.localEulerAngles = Vector3.zero;
        }


        public void EnterCover(CoverObject cover)
        {
            Color debugColor = Color.red;
            if (cover == null) return;

            float minCoverHeight = AimOrigin.y * 0.75f;
            float maxDistance = 1f;     //  Max distance away from cover.

            Vector3 origin = AimOrigin;
            Vector3 directionToCover = -(transform.position - cover.transform.position);
            //directionToCover.y = minCoverHeight;
            RaycastHit hit;



            if (Physics.Raycast(origin, directionToCover, out hit, maxDistance, Layers.cover))
            {
                //  We hit a box collider
                if (hit.transform.GetComponent<BoxCollider>())
                {
                    Quaternion targetRot = Quaternion.FromToRotation(transform.forward, hit.normal) * transform.rotation;
                    float angel = Vector3.Angle(hit.normal, -transform.forward);
                    //Debug.Log(angel);
                    transform.rotation = targetRot;

                    States.CanShoot = true;
                    States.InCover = true;

                    AnimHandler.EnterCover();
                    Debug.Log("Agent is taking cover");
                    debugColor = Color.green;
                }
            }

            Debug.DrawRay(origin, directionToCover, debugColor, 0.5f);
            //Debug.Break();
        }



		public void ExitCover()
        {
            States.InCover = false;
            States.CanShoot = true;
            AnimHandler.ExitCover();
            Debug.Log("Agent is leaving cover");
        }



        #endregion





        [Serializable]
        public class ActorStates
        {
            [SerializeField]
            private bool canShoot;
            [SerializeField]
            private bool isAiming;
            [SerializeField]
            private bool isReloading;
            [SerializeField]
            private bool inCover;
            [SerializeField]
            private bool isDead;


            public bool CanShoot{
                get{
                    return canShoot;
                }
                set{
                    canShoot = value;
                }
            }

            public bool IsAiming
            {
                get{
                    return isAiming;
                }
                set{
                    isAiming = value;
                }
            }

            public bool IsReloading{
                get{
                    return isReloading;
                }
                set{
                    isAiming = !value;
                    canShoot = !value;
                    isReloading = value;
                }
            }

            public bool InCover{
                get{
                    return inCover;
                }
                set{
                    inCover = value;
                }
            }

            public bool IsDead{
                get{
                    return isDead;
                }
                set{
                    canShoot = !value;
                    isDead = value;
                }
            }
        }
    }
}



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
        public int teamId;
        public ShootableWeapon weapon;
        public LayerMask targetLayerMask;               //  Specifies the layers that the targets are in
        public LayerMask ignoreLayerMask;               //  Specifies any layers that the sight check should ignore
        public Transform lookTransform;                 //  Optionally specify a transform to determine where to check the line of sight from

        protected ActorManager aManager;
        protected Rigidbody myRigidbody;
        protected CapsuleCollider controllerCollider;
        protected ActorHealth health;
        protected AnimationHandler animHandler;
        protected CharcterIK characterIK;
        protected CharacterRagdoll ragdoll;

        private float delta;
        private float aimHeight = 1.25f;
        private Vector3 aimPosition;
        private float reloadTime;


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
            characterIK = GetComponent<CharcterIK>();
            ragdoll = GetComponent<CharacterRagdoll>();

        }


        protected void OnEnable()
		{
            AimPosition = transform.position + transform.forward * 15;

            myRigidbody.isKinematic = false;
            controllerCollider.enabled = true;
		}



		public void InitializeActor(ActorManager manager)
        {
            aManager = manager;
            teamId = manager.teamId;
            EquipWeapon(WeaponNameIDs.Revolver_01);
            GetComponent<ActorSkins.ActorSkinComponent>().LoadActorSkin();
        }



		private void Update()
		{
            delta = Time.deltaTime;

            //  Execute abstract method.
            ExecuteUpdate(delta);
		}


		private void FixedUpdate()
		{
            delta = Time.fixedDeltaTime;

            //  Execute abstract method.
            ExecuteFixedUpdate(delta);
		}




		public virtual void EquipWeapon(string weaponID)
        {
            UnequipWeapon();

            WeaponObject w = GameManager.instance.WeaponManager.GetWeapon(weaponID);
            if(w != null)
            {
                weapon = Instantiate(w.prefab);
                weapon.transform.parent = AnimHandler.Anim.GetBoneTransform(HumanBodyBones.RightHand);
                weapon.transform.localPosition = w.mainHandIK.position;
                weapon.transform.localEulerAngles = w.mainHandIK.rotation;
                //  Initialize Shootable weapon stats.  Do this before setting up animations and IK.
                weapon.EquipWeapon(w, this);
                //  Set IK.
                //characterIK.EquidWeapon(weapon.MainHandIK, 1f, weapon.OffhandIK, 1f);
                characterIK.EquidWeapon(weapon.MainHandIK, weapon.OffhandIK);
                //  Set animtor weapon index.
                AnimHandler.EquipWeapon((int)weapon.WeaponType);
                //  Execute abstract method.
                OnEquipWeapon(weapon); 
            }
            else{
                Debug.Log("Weapon Manager has no weapon to equip.");
            }
        }


        public virtual void UnequipWeapon()
        {
            if(weapon != null){
                Destroy(weapon.gameObject);
            }
        }


        public void ShootWeapon(Vector3 target)
        {
            if(weapon.CurrentAmmo > 0 && weapon.CanShoot)
            {
                AimPosition = target;
                //Debug.DrawLine(weapon.ProjectileSpawn.position, weapon.ProjectileSpawn.position + (weapon.ProjectileSpawn.forward * 10), Color.red, 1f);
                //Debug.DrawLine(weapon.ProjectileSpawn.position, AimPosition, Color.magenta, 1f);

                //  Aim Weapon.
                weapon.transform.LookAt(AimPosition);
                // Shoot weapon.
                weapon.Shoot();
                //  Play Animation.
                AnimHandler.PlayShootAnim();

                //  Execute abstract method.
                OnShootWeapon();
            }
        }


        public void Reload()
        {
            if (weapon == null || weapon.IsReloading) return;


            //  Get the amount of ammo needed to reload.
            int ammoToReload = weapon.MaxAmmo - weapon.CurrentAmmo;
            //  Subtract that ammo amount from inventory.

            //  If there's enough ammo, tan reload.


            reloadTime = animHandler.AnimationLength("rifle_reload_still");
            var speedModifier = 2f;
            //Debug.Log("Rifle Reload Animation Length: " + reloadTime);


            //  Play Weapon Reload animation.  The weapon will add the correct amount of ammo.
            weapon.Reload(reloadTime / speedModifier, ammoToReload);
            //  Play Character Reload animation.
            AnimHandler.PlayReload(weapon.IsReloading);
            //  Set animation Reload paremeter.
            StartCoroutine(ReloadDelay(reloadTime / speedModifier));

            //  Execute abstract method.
            OnReload();
        }


        public void TakeDamage(Vector3 hitLocation, Vector3 force, GameObject attacker)
        {
            //myRigidbody.AddForce(hitDirection, ForceMode.Impulse);
            myRigidbody.AddForceAtPosition(force, hitLocation, ForceMode.Impulse);
            //  Execute abstract method.
            OnTakeDamage(force);

            AnimHandler.PlayTakeDamage(hitLocation);
        }


        public void Death(Vector3 hitLocation, Vector3 force, GameObject attacker)
        {
            myRigidbody.AddForceAtPosition(force, hitLocation, ForceMode.Impulse);

            if(ragdoll){
                ragdoll.EnableRagdoll(0.6f);
            }

            if (OnDeathEvent != null){
                //  Calls the Event if OnDeathEvent is not null.
                OnDeathEvent?.Invoke(attacker);
            }
            else{
                Debug.Log("Can't trigger OnDeathEvent");
            }

            myRigidbody.isKinematic = true;
            controllerCollider.enabled = false;

            AnimHandler.Death(hitLocation);
            //  Execute abstract method.
            OnDeath();
        }



        private IEnumerator ReloadDelay(float time)
        {
            yield return new WaitForSeconds(time);
            AnimHandler.PlayReload(false);
        }

        //
        //  Abstract
        //

        protected abstract void ExecuteUpdate(float deltaTime);

        protected abstract void ExecuteFixedUpdate(float deltaTime);

        protected abstract void OnEquipWeapon(ShootableWeapon weapon);

        protected abstract void OnShootWeapon();

        protected abstract void OnReload();

        protected abstract void OnTakeDamage(Vector3 hitDirection);

        protected abstract void OnDeath();

        public abstract void DisableControls();

        public abstract void EnableControls();





    }
}



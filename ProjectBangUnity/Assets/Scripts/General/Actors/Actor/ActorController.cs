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
        
        [SerializeField]
        private float delta;
        public Transform lookTransform;                 //  Optionally specify a transform to determine where to check the line of sight from
        public Gun weapon;
        public Transform weaponHolder;


        private ActorHealth _health;
        private AnimationHandler _animHandler;
        [SerializeField]
        private ActorStates _states;
        private float aimHeight = 1.25f;
        private Vector3 aimPosition;

        protected float shootingCooldown;
        protected float reloadCooldown;


        public AnimationHandler AnimHandler{
            get { return _animHandler; }
        }

        public ActorHealth Health{
            get { return _health; }
        }

        public ActorStates States{
            get { return _states; }
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
            _animHandler = GetComponent<AnimationHandler>();
            _health = GetComponent<ActorHealth>();
            _states = new ActorStates();

            InitializeCoverMarkers();
        }


        protected void OnEnable()
		{
            AimPosition = transform.position + transform.forward * 15;
            States.CanShoot = true;
		}



		public void Init(ActorManager manager)
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
            weapon.Init(this, w.ammo, w.maxAmmo);
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
        }


        public void TakeDamage(Vector3 hitDirection)
        {
            OnTakeDamage(hitDirection);
        }



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


        protected IEnumerator TransitionCoverStates()
        {
            yield return new WaitForSeconds(1f);
            States.CanShoot = true;
            States.InCover = true;
        }


        protected void HandleCoverState()
        {
            //Vector3 lookRotation = (transform.position - hitNormal);
            ////  Errors out when roation method is called towards a vector zero.
            //if (lookRotation != Vector3.zero)
            //{
            //    // Create a quaternion (rotation) based on looking down the vector from the player to the target.
            //    Quaternion newRotatation = Quaternion.LookRotation(lookRotation);
            //    transform.rotation = Quaternion.Slerp(transform.rotation, newRotatation, Time.fixedDeltaTime * 8);
            //}
        }


        public bool CanEmergeFromCover(Transform helper, bool right)
        {
            float entitySize = 0.5f;
            float distOffset = entitySize * 0.5f;
            Vector3 origin = transform.position;
            Vector3 side = (right == true) ? transform.right : -transform.right;
            //side.y = origin.y;
            Vector3 direction = side - origin;
            Vector3 helpPosition = side + (direction.normalized * 0.025f);
            helpPosition.y = 1f;
            helper.localPosition = helpPosition;
            Vector3 outDir = (-helper.transform.forward) + helper.position;


            float scanDistance = (outDir - helper.position).magnitude;
            RaycastHit hit;

            if(Physics.Raycast(helper.position, outDir, out hit, scanDistance, Layers.cover))
            {
                Debug.DrawLine(helper.position, outDir, Color.red, 1f);
                Debug.Log(helper.name + " hit " + hit.transform.name);
                return false;
            }

            Debug.DrawLine(helper.position, outDir, Color.green, 1f);
            return true;
        }


		protected void OnDrawGizmosSelected()
		{
            if(leftHelper != null && rightHelper != null)
            {
                Vector3 size = new Vector3(0.2f, 0.2f, 0.2f);
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(leftHelper.position, size);
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(rightHelper.position, size);
            }
		}


		//public bool CanEmergeFromCover(bool left = false)
		//{
		//    float entitySize = 0.5f;
		//    float distOffset = entitySize * 0.5f;
		//    Vector3 origin = AimOrigin;
		//    Vector3 side = left == false ? transform.right : -transform.right;

		//    Vector3 direction = side - origin;
		//    Vector3 scanPosition = side + (direction.normalized * distOffset);

		//    //Vector3 outDirection = scanPosition - Vector3.back;
		//    //Vector3 scanPosition = origin + offset * entitySize;
		//    //Vector3 scanDirection = scanPosition + (outDirection.normalized * 1);
		//    Vector3 scanDirection = new Vector3(scanPosition.x, scanPosition.y, scanPosition.z - 1);

		//    RaycastHit hit;

		//    Debug.DrawRay(origin, scanPosition, Color.magenta, 0.25f);
		//    Debug.Log(scanPosition);
		//    //Debug.DrawRay(scanPosition, scanDirection, Color.blue, 0.25f);


		//    if (Physics.Raycast(scanPosition, scanDirection, out hit, entitySize, Layers.cover))
		//    {
		//        Debug.DrawRay(scanPosition, scanDirection, Color.red, 0.25f);
		//        return false;
		//    }
		//    //Debug.DrawRay(scanPosition, scanDirection, Color.green, 0.25f);
		//    Debug.DrawRay(scanPosition, scanDirection, Color.blue, 0.25f);
		//    return true;
		//}


		public void ExitCover()
        {
            States.InCover = false;
            States.CanShoot = true;
            AnimHandler.ExitCover();
            Debug.Log("Agent is leaving cover");
        }



        #endregion


        public void Death()
        {
            OnDeath();
        }


        //
        //  Abstract
        //

        protected abstract void ExecuteUpdate(float deltaTime);

        protected abstract void ExecuteFixedUpdate(float deltaTime);

        protected abstract void OnShootWeapon();

        protected abstract void OnTakeDamage(Vector3 hitDirection);

        public abstract void OnDeath();

        public abstract void DisableControls();

        public abstract void EnableControls();



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



namespace Bang
{
    using UnityEngine;
    using System;

    public abstract class WeaponController : MonoBehaviour, IWeapon
    {
        protected ActorController owner;

        [SerializeField, Tooltip("Location of the projectile")]
        protected Transform _projectileSpawn;
        [SerializeField, Tooltip("What kind of projectile")]
        protected ProjectileBase _projectile;
        [SerializeField, Tooltip("Max ammo of the firearm")]
        protected int _maxAmmo;
        [SerializeField, Tooltip("Current ammo of the firearm")]
        protected int _currentAmmo;
        [SerializeField, Tooltip("Time in milliseconds between shots. The smaller the value, the quicker the shots")]
        protected float _timeBetweenShots = 50f;
        [SerializeField, Tooltip("Is the firearm reloading")]
        protected bool _isReloading;


        protected float nextShotTime;
        protected Vector3 dirToTarget = Vector3.forward;


        public Transform projectileSpawn
        {
            get{
                return _projectileSpawn;
            }
            set{
                _projectileSpawn = value;
            }
        }


        public ProjectileBase projectile
        {
            get{
                return _projectile;
            }
            set{
                _projectile = value;
            }
        }


        public int maxAmmo
       {
            get { return _maxAmmo; }
        }


        public int currentAmmo
        {
            get {
                return _currentAmmo > 0 ? _currentAmmo : 0; 
            }
            set{
                _currentAmmo = value;
            }
        }


        public float timeBetweenShots
        {
            get { return _timeBetweenShots; }
        }



        public bool isReloading
        {
            get { return _isReloading; }
        }



        protected virtual void Awake()
        {
            if (_projectileSpawn == null) throw new ArgumentNullException(this.GetType().Name + " has no projectile spawn location");
            if (_projectile == null) throw new ArgumentNullException(this.GetType().Name + " has no projectile");

            currentAmmo = maxAmmo;
        }


        protected virtual void Start()
		{
            //currentAmmo = maxAmmo;
		}


		protected virtual void OnEnable()
        {

        }


        protected virtual void OnDisable()
        {

        }

        public virtual void Init(ActorController actorCtrl)
        {
            owner = actorCtrl;
        }



        public virtual void Reload()
        {
            currentAmmo = maxAmmo;
        }


        public virtual void Shoot()
        {
            if (currentAmmo > 0 && Time.time > nextShotTime){
                currentAmmo--;
                nextShotTime = Time.time + timeBetweenShots / 100;

                //Debug.LogFormat("Shot Fired:  {0}", Time.time);

                var _pooledProjectile = PoolManager.instance.Spawn(PoolTypes.Projectile, projectileSpawn.position, projectileSpawn.rotation);
                ProjectileBase pooledProjectile = _pooledProjectile.gameObject.GetComponent<ProjectileBase>();
                pooledProjectile.Init(owner);
            }
        }


        public virtual void Shoot(Vector3 target)
        {
            dirToTarget.x = target.x;
            dirToTarget.y = projectileSpawn.position.y;
            dirToTarget.z = target.z;

            //  Have projectileSpawn Look at the target.
            projectileSpawn.transform.LookAt(dirToTarget, Vector3.up);

            //  Shoot.
            Shoot();
            //  Reset projectileSpawn and dirToTarget.
            projectileSpawn.localRotation = Quaternion.identity;
            dirToTarget = Vector3.forward;
        }




    }
}



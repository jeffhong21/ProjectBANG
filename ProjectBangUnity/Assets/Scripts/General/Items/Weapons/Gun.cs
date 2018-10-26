namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections;

    public abstract class Gun : WeaponBase
    {

        [SerializeField]
        protected int currentAmmo;
        [SerializeField]
        protected int maxAmmo;
        [SerializeField]
        protected float fireRate = 0.5f;
        [SerializeField]
        protected bool isReloading;
        [SerializeField]
        protected Transform offhandIK;
        [SerializeField]
        protected Transform projectileSpawn;
        [SerializeField]
        protected ProjectileBase projectile;


        protected IEnumerator reloadCoroutine;
        protected float nextShotTime;
        protected Vector3 dirToTarget; // = Vector3.forward;


        public int CurrentAmmo{
            get{
                if (currentAmmo > maxAmmo)
                    currentAmmo = maxAmmo;
                return currentAmmo > 0 ? currentAmmo : 0;
            }
            set{
                currentAmmo = value;
            }
        }

        public int MaxAmmo{
            get { return maxAmmo; }
        }


        public bool IsReloading{
            get { return isReloading; }
        }

        public Transform MainHandIK{
            get { return this.transform; }
        }

        public Transform OffhandIK{
            get { return offhandIK; }
        }

        public Transform ProjectileSpawn
        {
            get { return projectileSpawn; }
        }

        protected virtual void Awake()
        {
            if (projectileSpawn == null) throw new ArgumentNullException(this.GetType().Name + " has no projectile spawn location");
            if (projectile == null) throw new ArgumentNullException(this.GetType().Name + " has no projectile");
        }




        public void Init(ActorController owner, int currentAmmo, int maxAmmo)
        {
            this.owner = owner;
            this.currentAmmo = currentAmmo;
            this.maxAmmo = maxAmmo;
        }



        public virtual void Shoot()
        {
            if (currentAmmo > 0 && Time.time > nextShotTime)
            {
                currentAmmo--;
                nextShotTime = Time.time + fireRate;

                //Debug.LogFormat("Shot Fired:  {0}", Time.time);

                var _pooledProjectile = PoolManager.instance.Spawn(PoolTypes.Projectile, projectileSpawn.position, projectileSpawn.rotation);
                ProjectileBase pooledProjectile = _pooledProjectile.gameObject.GetComponent<ProjectileBase>();
                pooledProjectile.Init(owner);
            }
        }


        public virtual void Reload(float reloadTime)
        {
            reloadCoroutine = ReloadAnim(reloadTime);

            if (reloadCoroutine != null)
            {
                isReloading = true;
                StartCoroutine(reloadCoroutine);
                isReloading = false;
                currentAmmo = maxAmmo;
            }
        }


        protected abstract IEnumerator ReloadAnim(float time);


        protected abstract IEnumerator Shoot(float time);
    }
}



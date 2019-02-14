namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections;

    public abstract class ShootableWeapon : MonoBehaviour
    {
        [Header("Weapon Properties")]
        [SerializeField]
        protected Transform offhandIK;
        [SerializeField]
        protected Transform projectileSpawn;


        protected ActorController owner;
        protected string nameID;
        protected WeaponTypes weaponType;
        protected bool autoReload;

        protected int maxAmmo;
        protected float damage;
        protected float firerate;
        protected float power;
        protected float range;
        protected float spread;
        protected Projectile projectile;

        [Header("Weapon Runtime Stats")]
        [SerializeField, ReadOnly]
        protected int currentAmmo;
        [SerializeField, ReadOnly]
        protected bool canShoot;
        [SerializeField, ReadOnly]
        protected bool isReloading;
        [SerializeField, ReadOnly]
        protected float lastShootTime;

        protected ParticleSystem[] particles;

        protected IEnumerator reloadCoroutine;




        public string NameID{
            get { return nameID; }
        }

        public WeaponTypes WeaponType{
            get { return weaponType; }
        }

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

        public float Damage{
            get { return damage; }
        }

        public float Firerate{
            get { return firerate; }
        }

        public float Power{
            get { return power; }
        }

        public float Range{
            get { return range; }
        }

        public float Spread{
            get { return spread; }
        }

        public bool CanShoot{
            get { return canShoot; }
        }

        public bool IsReloading{
            get { return isReloading; }
        }

        public Transform MainHandIK{
            get { return transform; }
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
            if (projectileSpawn == null){
                throw new ArgumentNullException(GetType().Name + " has no projectile spawn location");
            }
            particles = GetComponentsInChildren<ParticleSystem>();

            canShoot = true;
        }


        public void EquipWeapon(WeaponObject data, ActorController actor)
        {
            owner = actor;
            nameID = data.nameID;
            weaponType = data.weaponType;
            autoReload = data.autoReload;

            currentAmmo = data.maxAmmo;
            maxAmmo = data.maxAmmo;
            damage = data.damage;
            firerate = data.firerate;
            power = data.power;
            range = data.spread;
            spread = data.spread;
            projectile = data.projectile;


            if (projectile == null){
                throw new ArgumentNullException("** " + this.GetType().Name + " has no projectile");
            }
        }


        public virtual void Shoot()
        {
            if(Time.timeSinceLevelLoad > lastShootTime)
            {
                if (currentAmmo > 0 && canShoot)
                {
                    //  Spawn Projectile from the PooManager.
                    IPooled p = PoolManager.instance.Spawn(PoolTypes.Projectile, projectileSpawn.position, projectileSpawn.rotation);
                    Projectile pooledProjectile = p.gameObject.GetComponent<Projectile>();
                    //  Initialize the projectile.
                    pooledProjectile.Initialize(owner, damage, power, range);


                    //  Play Particle Shoot Effects
                    if (particles != null){
                        for (int i = 0; i < particles.Length; i++){
                            particles[i].Play();
                        }
                    }


                    //  Update Current Ammo;
                    currentAmmo--;
                    //  Stamp last shot time.
                    lastShootTime = Time.timeSinceLevelLoad + firerate;
                }

            }
        }




        public virtual void Reload(float reloadTime, int reloadAmmount)
        {
            reloadCoroutine = ReloadAnim(reloadTime, reloadAmmount);

            if (reloadCoroutine != null)
            {
                isReloading = true;
                canShoot = false;
                //Debug.LogFormat("Time {0} | CanShoot: {1} | IsReloading {2}", Time.timeSinceLevelLoad, canShoot, isReloading);
                StartCoroutine(reloadCoroutine);

            }
        }


        protected Projectile SpawnProjectile()
        {
            var p = PoolManager.instance.Spawn(PoolTypes.Projectile, projectileSpawn.position, projectileSpawn.rotation);
            Projectile pooledProjectile = p.gameObject.GetComponent<Projectile>();
            //  Initialize the projectile.
            pooledProjectile.Initialize(owner, damage, power, range);

            return pooledProjectile;
        }


        private IEnumerator ReloadAnim(float time, int reloadAmmount)
        {
            yield return PlayReloadAnim(time);
            canShoot = true;
            isReloading = false;
            currentAmmo += reloadAmmount;
        }


       
        protected abstract IEnumerator PlayReloadAnim(float time);


        protected abstract IEnumerator PlayShootAnim(float time);
    }
}



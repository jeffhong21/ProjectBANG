namespace Bang
{
    using UnityEngine;

    public interface IFirearm : IPooled
    {
        /// <summary>
        /// Gets or sets the projectile spawn.
        /// </summary>
        /// <value>The projectile spawn.</value>
        Transform projectileSpawn{
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the projectile.
        /// </summary>
        /// <value>The projectile.</value>
        ProjectileBase projectile{
            get;
            set;
        }

        /// <summary>
        /// Gets the max ammo.
        /// </summary>
        /// <value>The max ammo.</value>
        int maxAmmo{
            get;
        }

        /// <summary>
        /// Gets the current ammo.
        /// </summary>
        /// <value>The current ammo.</value>
        int currentAmmo{
            get;
            set;
        }

        /// <summary>
        /// Gets the time between shots.
        /// </summary>
        /// <value>The time between shots.</value>
        float timeBetweenShots { 
            get; 
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Bang.IFirearm"/> can shoot.
        /// </summary>
        /// <value><c>true</c> if can shoot; otherwise, <c>false</c>.</value>
        bool canShoot{
            get;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Bang.IFirearm"/> is reloading.
        /// </summary>
        /// <value><c>true</c> if is reloading; otherwise, <c>false</c>.</value>
        bool isReloading{
            get;
        }

        /// <summary>
        /// Shoot this instance.
        /// </summary>
        void Shoot();

        /// <summary>
        /// Shoot the specified target.
        /// </summary>
        /// <returns>The shoot.</returns>
        /// <param name="target">Target.</param>
        void Shoot(Vector3 target);

        /// <summary>
        /// Reload this instance.
        /// </summary>
        void Reload();

        /// <summary>
        /// Cocks the gun.
        /// </summary>
        void CockFirearm();


    }

}
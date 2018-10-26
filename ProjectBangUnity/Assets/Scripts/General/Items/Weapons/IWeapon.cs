namespace Bang
{
    using UnityEngine;

    public interface IWeapon : IPooled
    {


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
        /// Gets a value indicating whether this <see cref="T:Bang.IWeapon"/> is reloading.
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



    }

}
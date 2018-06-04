namespace Bang
{
    using UnityEngine;

    public interface IHasFirearm
    {

        /// <summary>
        /// Gets or sets the gun.
        /// </summary>
        /// <value>The gun.</value>
        FirearmBase equippedFirearm{
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Bang.IHasGun"/> can shoot.
        /// </summary>
        /// <value><c>true</c> if can shoot; otherwise, <c>false</c>.</value>
        bool canShoot{
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the reload speed.
        /// </summary>
        /// <value>The reload speed.</value>
        float reloadSpeed{
            get;
            set;
        }


        /// <summary>
        /// Shoot the specified target.
        /// </summary>
        /// <returns>The shoot.</returns>
        /// <param name="target">Target.</param>
        void FireWeapon(Vector3 target);


        /// <summary>
        /// Reload this instance.
        /// </summary>
        void Reload();


        /// <summary>
        /// Equips the gun.
        /// </summary>
        /// <param name="gun">Gun.</param>
        void EquipGun(FirearmBase fireArm, Transform location);
    }

}
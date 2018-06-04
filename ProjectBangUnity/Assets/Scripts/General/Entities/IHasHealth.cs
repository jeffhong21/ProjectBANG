namespace Bang
{
    using UnityEngine;

    public interface IHasHealth : IEntity, IPooled
    {
        //
        //  Properties
        //


        /// <summary>
        /// Max health for entity.
        /// </summary>
        /// <value>The max health.</value>
        float maxHealth
        {
            get;
        }

        /// <summary>
        /// Current health for Actor.
        /// </summary>
        /// <value>The current health.</value>
        float currentHealth
        {
            get;
            set;
        }

        /// <summary>
        /// Is the Actor dead.
        /// </summary>
        /// <value><c>true</c> if is dead; otherwise, <c>false</c>.</value>
        bool isDead
        {
            get;
            set;
        }

        //
        //  Methods
        //

        /// <summary>
        /// The amount of damage taken.
        /// </summary>
        /// <param name="damage">Damage.</param>
        void TakeDamage(float damage);


        /// <summary>
        /// The amount of damage taken plus a hit location for particles.
        /// </summary>
        /// <param name="damage">Damage.</param>
        /// <param name="hitLocation">Hit location.</param>
        void TakeDamage(float damage, Vector3 hitLocation);


        /// <summary>
        /// The amount of damage taken plus a hit location for particles and a direction for rotation.
        /// </summary>
        /// <param name="damage">Damage.</param>
        /// <param name="hitLocation">Hit location.</param>
        /// <param name="hitDirection">Hit direction.</param>
        void TakeDamage(float damage, Vector3 hitLocation, Vector3 hitDirection);


        /// <summary>
        /// Death this instance.
        /// </summary>
        void Death();
    }

}
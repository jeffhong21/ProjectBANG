namespace Bang
{
    using UnityEngine;

    public interface IParticleSystem : IPooled
    {
        /// <summary>
        /// Gets the duration of the particle system, as set on the particlce system.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        float duration { get; }

        /// <summary>
        /// Play the particle system.
        /// </summary>
        void Play();
    }

}
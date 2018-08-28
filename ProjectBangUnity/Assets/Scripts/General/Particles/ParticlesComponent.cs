namespace Bang
{
    using UnityEngine;


    public class ParticlesComponent : PooledBase, IParticleSystem
    {
        private ParticleSystem _system;

        /// <summary>
        /// Gets the duration of the particle system, as set on the particlce system.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public float duration
        {
            get { return _system.main.duration; }
        }

        /// <summary>
        /// Called by Unity when [enabled].
        /// </summary>
        protected virtual void OnEnable()
        {

            _system = this.GetComponent<ParticleSystem>();
        }

        /// <summary>
        /// Play the particle system.
        /// </summary>
        public void Play()
        {
            _system.Play();
        }

    }
}



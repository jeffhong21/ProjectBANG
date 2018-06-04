namespace Bang
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    public class ParticlePoolManager : SingletonMonoBehaviour<ParticlePoolManager>
    {
        //  Gets the length of the enum.
        private static readonly int poolTypeLength = Enum.GetNames(typeof(ParticlesType)).Length;

        //  Name of the gameObject the pooled objects are parented too.
        public static string managerHostName = "ParticlePools";
        public static string poolHostName = "{0}Pool";

        [SerializeField]
        private ParticleSetup[] _poolSetup = new ParticleSetup[poolTypeLength];
        // Key is the tag or type of the pool, Value is the Pool of objects.
        private Dictionary<ParticlesType, PoolBase<IParticleSystem>> _pools;




        protected override void Awake()
        {
            base.Awake();

            _pools = new Dictionary<ParticlesType, PoolBase<IParticleSystem>>(poolTypeLength);


            //note -> can be reduced to one-level transform hierarchy for performance
            GameObject managerHost = new GameObject(managerHostName);
            managerHost.transform.SetParent(this.transform);

            for (int i = 0; i < _poolSetup.Length; i++)
            {
                ParticleSetup setup = _poolSetup[i];
                //  Creating a container to hold separate pool types.
                GameObject host = new GameObject(string.Format(poolHostName, setup.type.ToString()));
                host.transform.SetParent(managerHost.transform);

                //_pools.Add(setup.tag, new PoolBase<IPooled>(setup.prefab, host, setup.initialInstanceCount));
                _pools.Add(setup.type, new PoolBase<IParticleSystem>(setup.prefab, host, setup.initialInstanceCount));
            }
        }


        /// <summary>
        /// Spawns a particle system of the given type at the specified position, with default rotation.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="position">The position.</param>
        public void SpawnParticleSystem(ParticlesType type, Vector3 position)
        {
            SpawnParticleSystem(type, position, Quaternion.identity);
        }

        /// <summary>
        /// Spawns a particle system of the given type at the specified position, with the desired rotation.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        public void SpawnParticleSystem(ParticlesType type, Vector3 position, Quaternion rotation)
        {
            var particles = _pools[type].Get(position, rotation);
            particles.Play();

            if (this.gameObject.activeSelf)
            {
                // only return systems if this manager is still active, otherwise the game is shutting down anyway
                StartCoroutine(ReturnSystem(type, particles));
            }
        }

        /// <summary>
        /// Returns the given particle system to the pool from whence it came.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="system">The system.</param>
        /// <returns></returns>
        private IEnumerator ReturnSystem(ParticlesType type, IParticleSystem system)
        {
            yield return new WaitForSeconds(system.duration);
            _pools[type].Return(system);
        }






    }













}



namespace Bang
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;


    public class PoolManager : SingletonMonoBehaviour<PoolManager>
    {
        //  Gets the length of the enum.
        private static readonly int poolTypeLength = Enum.GetNames(typeof(PoolTypes)).Length;

        //  Name of the gameObject the pooled objects are parented too.
        public static string managerHostName = "EntityPool";
        public static string poolHostName = "{0}Pool";

        [SerializeField]
        private PoolSetup[] _poolSetup = new PoolSetup[poolTypeLength];
        // Key is the tag or type of the pool, Value is the Pool of objects.
        //private Dictionary<string, PoolBase<IPooled>> _pools;
        private Dictionary<PoolTypes, PoolBase<IPooled>> _pools;

        public static int playerCount = 1;
        private List<IEntity> _players;
        public IList<IEntity> players{
            get { return _players; }
        }


        protected override void Awake()
		{
            base.Awake();

            _pools = new Dictionary<PoolTypes, PoolBase<IPooled>>(poolTypeLength);
            //_pools = new Dictionary<string, PoolBase<IPooled>>(_poolSetup.Length);
            _players = new List<IEntity>(playerCount);


            //note -> can be reduced to one-level transform hierarchy for performance
            GameObject managerHost = new GameObject(managerHostName);
            managerHost.transform.SetParent(this.transform);

            for (int i = 0; i < _poolSetup.Length; i++)
            {
                PoolSetup setup = _poolSetup[i];
                //  Creating a container to hold separate pool types.
                GameObject host = new GameObject(string.Format(poolHostName, setup.type.ToString()));
                host.transform.SetParent(managerHost.transform);

                //_pools.Add(setup.tag, new PoolBase<IPooled>(setup.prefab, host, setup.initialInstanceCount));
                _pools.Add(setup.type, new PoolBase<IPooled>(setup.prefab, host, setup.initialInstanceCount));
            }
		}


        public IPooled Spawn(PoolTypes type, Vector3 position, Quaternion rotation)
        {
            if (_pools.ContainsKey(type) == false)
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
                return null;
            }

            PoolBase<IPooled> pool = _pools[type];
            IPooled entity = pool.Get(position, rotation);

            return entity;
        }

        public void Return(PoolTypes type, IPooled entity)
        {
            if (_pools.ContainsKey(type) == false)
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
                return;
            }

            PoolBase<IPooled> pool = _pools[type];
            pool.Return(entity);

            // TODO: If the entity is a player, test for game over condition.
        }



        /// <summary>
        /// Gets the player closest to the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The closest player, or null if no more players exist.</returns>
        public HasHealthBase GetClosestPlayer(Vector3 position)
        {
            HasHealthBase closest = null;
            float closestDistance = Mathf.Infinity;

            var count = _players.Count;
            for (int i = 0; i < count; i++)
            {
                var e = _players[i] as HasHealthBase;
                if (e.currentHealth <= 0)
                {
                    continue;
                }

                var dist = (position - e.position).sqrMagnitude;

                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closest = e;
                }
            }

            return closest;
        }


	}













}



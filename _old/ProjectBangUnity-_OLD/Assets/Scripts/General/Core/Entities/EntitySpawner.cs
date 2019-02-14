namespace Bang
{
    using UnityEngine;

    /// <summary>
    /// Spawns entities at an interval (or just once).
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class EntitySpawner : MonoBehaviour
    {
        /// <summary>
        /// The type of object to be spawned -> must correspond to a list in the spawning manager
        /// </summary>
        public ActorTypes entityType;

        /// <summary>
        /// The frequency by which objects are spawned
        /// </summary>
        public float frequency = 3f;

        /// <summary>
        /// Time delay before the first object is spawned
        /// </summary>
        public float delay = 4f;

        /// <summary>
        /// Instruct the spawner to only spawn one
        /// </summary>
        public bool spawnOnce;

        private float _lastSpawnTime;

        private void Start()
        {
            _lastSpawnTime = delay - frequency;
        }

        private void Update()
        {
            if (Time.time > _lastSpawnTime + frequency)
            {
                Spawn();
                if (spawnOnce)
                {
                    this.enabled = false;
                }
            }
        }

        private void Spawn()
        {
            _lastSpawnTime = Time.time;
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            ActorPoolManager.instance.Spawn(entityType, transform.position, rotation);
        }
    }
}

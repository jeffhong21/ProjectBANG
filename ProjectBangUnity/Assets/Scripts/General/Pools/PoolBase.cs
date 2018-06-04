namespace Bang
{
    using UnityEngine;
    using System.Collections.Generic;

    public class PoolBase<T> where T: IPooled
    {

        private Queue<T> _pool;
        private GameObject _prefab;
        private GameObject _host;


        /// <summary>
        /// Get the current size of the queueGets the count.
        /// </summary>
        /// <value>The count.</value>
        public int count
        {
            get { return _pool.Count; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EntityPool"/> class.
        /// </summary>
        /// <param name="prefab">The prefab from which to create the entity.</param>
        /// <param name="host">The host that will be the parent of entity instances.</param>
        /// <param name="initialInstanceCount">The initial instance count.</param>
        public PoolBase(GameObject prefab, GameObject host, int initialInstanceCount)
        {
            _pool = new Queue<T>(initialInstanceCount);
            _prefab = prefab;
            _host = host;

            //Instantiate and queue up the initial number of entities
            for (int i = 0; i < initialInstanceCount; i++)
            {
                _pool.Enqueue(CreateInstance());
            }
        }


        /// <summary>
        /// Gets an entity from the pool and places it at the specified position and rotation.
        /// If the pool is empty a new instance is created.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <returns>The entity</returns>
        public T Get(Vector3 position, Quaternion rotation)
        {
            //  If pool count is greater than 0, take on from the pool.  If not, create a new instance of it.
            T entity;
            if (_pool.Count > 0)
            {
                entity = _pool.Dequeue();
            }
            else
            {
                entity = CreateInstance();
            }

            Transform t = entity.transform;

            t.position = position;
            t.rotation = rotation;

            entity.gameObject.SetActive(true);
            return entity;
        }

        /// <summary>
        /// Returns the specified entity to the pool.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Return(T entity)
        {
            entity.transform.SetParent(_host.transform);  //  Parent the prefab to the object holder.
            entity.gameObject.SetActive(false);
            _pool.Enqueue(entity);
        }


        private T CreateInstance()
        {
            GameObject go = GameObject.Instantiate(_prefab);
            go.transform.SetParent(_host.transform);  //  Parent the prefab to the object holder.
            go.SetActive(false);
            return go.GetComponent<T>();
        }


    }
}



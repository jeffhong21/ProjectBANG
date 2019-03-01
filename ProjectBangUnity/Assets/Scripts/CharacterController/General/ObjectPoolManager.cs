using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [SerializeField]
    private List<ObjectPool> m_Pools = new List<ObjectPool>();

    private Dictionary<string, Pool> m_ObjectPool;

    private Transform m_Host;




    protected void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning(this + " another instance has already been registered for this scene, destroying this one");
            Destroy(this);
            return;
        }
        Instance = this;

        m_ObjectPool = new Dictionary<string, Pool>();
        m_Host = transform;
    }


    protected void Start()
    {
        for (int index = 0; index < m_Pools.Count; index++)
        {
            ObjectPool pool = m_Pools[index];
            Pool objectPool = new Pool(pool.Prefab, m_Host, pool.Count);
            m_ObjectPool.Add(pool.ID, objectPool);
        }

    }


    public GameObject Get(string id, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (m_ObjectPool.ContainsKey(id))
        {
            Pool pool = m_ObjectPool[id];
            GameObject obj = pool.Get(position, rotation, parent);
            return obj;
        }
        return null;
    }


    public void Return(string id, GameObject obj)
    {
        if (m_ObjectPool.ContainsKey(id))
        {
            Pool pool = m_ObjectPool[id];
            pool.Return(obj);
        }
    }







    [Serializable]
    public class Pool
    {

        private Queue<GameObject> m_Pool;
        private GameObject m_Prefab;        //  The object to instantiate.
        private Transform m_Host;           //  The parent object of all the objects.


        public int Count { get { return m_Pool.Count; } }



        public Pool(GameObject prefab, Transform host, int initialInstanceCount)
        {
            m_Pool = new Queue<GameObject>(initialInstanceCount);
            m_Prefab = prefab;
            m_Host = host;

            //Instantiate and queue up the initial number of entities
            for (int i = 0; i < initialInstanceCount; i++)
            {
                m_Pool.Enqueue(CreateInstance(m_Host));
            }
        }


        protected GameObject CreateInstance(Transform host)
        {
            GameObject obj = GameObject.Instantiate(m_Prefab);
            obj.transform.SetParent(host);
            obj.SetActive(false);
            return obj;
        }



        public GameObject Get(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            GameObject obj;
            if (m_Pool.Count > 0)
            {
                obj = m_Pool.Dequeue();
            }
            else
            {
                obj = CreateInstance(parent);
            }
            obj.gameObject.SetActive(true);
            obj.transform.position = position;
            obj.transform.rotation = rotation;

            return null;
        }


        public void Return(GameObject obj)
        {
            obj.transform.SetParent(m_Host.transform);

            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.gameObject.SetActive(false);

            m_Pool.Enqueue(obj.gameObject);
        }

    }
}

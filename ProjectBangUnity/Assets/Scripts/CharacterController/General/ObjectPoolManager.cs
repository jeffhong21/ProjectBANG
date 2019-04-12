using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [SerializeField]
    private List<Pool> m_Pools = new List<Pool>();

    private static Dictionary<GameObject, Queue<GameObject>> m_ObjectPool;

    private static Transform m_Host;




    protected void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        m_ObjectPool = new Dictionary<GameObject, Queue<GameObject>>();
        m_Host = transform;


        for (int index = 0; index < m_Pools.Count; index++)
        {
            m_ObjectPool.Add(m_Pools[index].Prefab, new Queue<GameObject>());
            for (int i = 0; i < m_Pools[index].Count; i++)
            {
                Instantiate(m_Pools[index].Prefab, Vector3.zero, Quaternion.identity, m_Host);
            }
        }
    }






    public static GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        //if (!m_ObjectPool.ContainsKey(original))
        //m_ObjectPool.Add(original, new Queue<GameObject>());

        GameObject instantiatedObject = Instantiate(original);
        m_ObjectPool[original].Enqueue(instantiatedObject);


        instantiatedObject.transform.position = position;
        instantiatedObject.transform.rotation = rotation;
        instantiatedObject.transform.SetParent(parent);
        instantiatedObject.SetActive(false);

        return instantiatedObject;
    }


    public static GameObject Spawn(GameObject original, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject instantiatedObject = null;
        if (m_ObjectPool.ContainsKey(original))
        {
            if (m_ObjectPool[original].Count > 0)
            {
                instantiatedObject = m_ObjectPool[original].Dequeue();
            }
            else
            {
                instantiatedObject = Instantiate(original, Vector3.zero, Quaternion.identity, parent);
            }


            instantiatedObject.transform.position = position;
            instantiatedObject.transform.rotation = rotation;
            instantiatedObject.transform.SetParent(parent);
            instantiatedObject.SetActive(true);
        }
        //else{
        //    m_ObjectPool.Add(original, new Queue<GameObject>());
        //    instantiatedObject = Instantiate(original, Vector3.zero, Quaternion.identity, parent);
        //}



        return instantiatedObject;
    }


    public static int OriginalInstanceID(GameObject instantiatedObject)
    {
        return 0;
    }


    public static void Destroy(GameObject instantiatedObject)
    {
        if (m_ObjectPool.ContainsKey(instantiatedObject))
        {
            Debug.LogFormat("Returning {0} to pool", instantiatedObject);
            instantiatedObject.transform.SetParent(m_Host.transform);

            instantiatedObject.transform.localPosition = Vector3.zero;
            instantiatedObject.transform.localEulerAngles = Vector3.zero;
            instantiatedObject.transform.localScale = Vector3.one;
            instantiatedObject.gameObject.SetActive(false);

            m_ObjectPool[instantiatedObject].Enqueue(instantiatedObject.gameObject);
        }
        else
        {
            Debug.LogFormat("Object pool does not contain {0}", instantiatedObject);
        }
    }





    [Serializable]
    public class Pool
    {
        //public string ID;
        public GameObject Prefab;

        public int Count;
    }
}

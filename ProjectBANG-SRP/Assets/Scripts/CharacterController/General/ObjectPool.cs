﻿using System;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }


    [Serializable]
    public class Pool
    {
        [SerializeField, DisplayOnly]
        public int InstanceID;
        public GameObject Prefab;
        public int Count;
    }


    public class ObjectPooler<T> where T : MonoBehaviour
    {
        
    }

    [SerializeField]
    private List<Pool> m_Pools = new List<Pool>();

    private static Dictionary<GameObject, Queue<GameObject>> m_GameObjectPool;
    private static Dictionary<int, GameObject> m_InstanceIdMap;
    private static Dictionary<int, int> m_InstanceIdLookup;



    private static Transform m_Host;




    protected void Awake()
    {
        if (Instance != null){
            Destroy(this);
            return;
        }
        Instance = this;


        InitializePools();
    }


	private void OnValidate()
	{
        foreach (var pool in m_Pools){
            if (pool.Prefab != null) pool.InstanceID = pool.Prefab.GetInstanceID();
        }
    }


	private void InitializePools()
    {
        m_GameObjectPool = new Dictionary<GameObject, Queue<GameObject>>();
        m_InstanceIdMap = new Dictionary<int, GameObject>();
        m_InstanceIdLookup = new Dictionary<int, int>();
        m_Host = transform;


        for (int index = 0; index < m_Pools.Count; index++)
        {
            var prefab = m_Pools[index].Prefab;
            m_GameObjectPool.Add(prefab, new Queue<GameObject>());
            for (int i = 0; i < m_Pools[index].Count; i++)
            {
                //GameObject instance = ObjectPool.Instantiate(prefab, Vector3.zero, Quaternion.identity, m_Host);

                //  Instantiate a new gameObject.
                var instance = UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, m_Host);
                //  Add to the pool.
                m_GameObjectPool[prefab].Enqueue(instance);
                //  Register the new instantiatedObject ID.
                m_InstanceIdLookup.Add(instance.GetInstanceID(), prefab.GetInstanceID());


                //  Register each instance's instanceID to the original prefab it is derived from.
                if(m_InstanceIdLookup.ContainsKey(instance.GetInstanceID()) == false){
                    m_InstanceIdLookup.Add(instance.GetInstanceID(), prefab.GetInstanceID());
                }
                else{
                    //Debug.LogFormat("Updating {0} instanceID value to {1}", instance.name, prefab.GetInstanceID());
                    m_InstanceIdLookup[instance.GetInstanceID()] = prefab.GetInstanceID();
                }

                //  Register the original prefabs instanceID so we know which prefab is which when we return the instanceObject.
                if (m_InstanceIdMap.ContainsKey(prefab.GetInstanceID()) == false)
                    m_InstanceIdMap.Add(prefab.GetInstanceID(), prefab);


                instance.SetActive(false);
                //instance.hideFlags = HideFlags.HideInHierarchy;
            }
        }
    }






    public static GameObject Get(GameObject original, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject instantiatedObject = null;
        if (m_GameObjectPool.ContainsKey(original)){
            if (m_GameObjectPool[original].Count > 0)
            {
                instantiatedObject = m_GameObjectPool[original].Dequeue();
            }
            else{
                //  Instantiate a new gameObject.
                instantiatedObject = Instantiate(original);
                //  Add to the pool.
                m_GameObjectPool[original].Enqueue(instantiatedObject);
                //  Register the new instantiatedObject ID.
                m_InstanceIdLookup.Add(instantiatedObject.GetInstanceID(), original.GetInstanceID());
            }
            //Debug.LogFormat("Total instances of {0} remaining: {1}", original.name, m_GameObjectPool[original].Count);
        }
        else{
            //  Instantiate a new gameObject.
            instantiatedObject = Instantiate(original);
            //  Create a new ObjectPool and add the new instance to the pool.
            m_GameObjectPool.Add(original, new Queue<GameObject>());
            m_GameObjectPool[original].Enqueue(instantiatedObject);
            //  Register the new instantiatedObject ID.
            m_InstanceIdLookup.Add(instantiatedObject.GetInstanceID(), original.GetInstanceID());
            Debug.LogFormat("Creating a new object pool for {0} (InstanceID: {1})", original.name, original.GetInstanceID());
        }

        instantiatedObject.transform.position = position;
        instantiatedObject.transform.rotation = rotation;
        instantiatedObject.transform.SetParent(parent);
        instantiatedObject.SetActive(true);


        //Debug.LogFormat("Original InstanceID: {0} | Instance InstanceID {1}",GetOriginalInstanceID(instantiatedObject), instantiatedObject.GetInstanceID());
        return instantiatedObject;
    }


    public static int GetOriginalInstanceID(GameObject instantiatedObject)
    {
        if(m_InstanceIdLookup.ContainsKey(instantiatedObject.GetInstanceID())){
            return m_InstanceIdLookup[instantiatedObject.GetInstanceID()];
        }
        return -1;
    }


    public static void Return(GameObject instantiatedObject)
    {
        var originalInstanceID = GetOriginalInstanceID(instantiatedObject);
        var originalPrefab = m_InstanceIdMap[originalInstanceID];
        if(originalInstanceID == -1){
            originalPrefab = null;
        }


        if (originalPrefab != null && m_GameObjectPool.ContainsKey(originalPrefab))
        {
            //Debug.LogFormat("Returning {0} to pool", instantiatedObject);
            instantiatedObject.transform.SetParent(m_Host.transform);

            instantiatedObject.transform.localPosition = Vector3.zero;
            instantiatedObject.transform.localEulerAngles = Vector3.zero;
            instantiatedObject.transform.localScale = Vector3.one;
            instantiatedObject.gameObject.SetActive(false);

            m_GameObjectPool[originalPrefab].Enqueue(instantiatedObject);
        }
        else
        {
            Debug.LogFormat("Object pool does not contain {0}.  Destroying it.", instantiatedObject);
            Destroy(instantiatedObject);

        }
    }



    //public static T Get<T>() where T : MonoBehaviour, IPooled<T>
    //{
        
    //}

    //public static void Return<T>(T obj) where T : MonoBehaviour, IPooled<T>
    //{
        
    //}







}




//public class ObjectPooler<T> where T : MonoBehaviour, IPooled<T>
//{
//    public T[] instances;

//    protected Stack<int> m_FreeIdx;

//    public void InitializePools(int count, T prefab)
//    {
//        instances = new T[count];
//        m_FreeIdx = new Stack<int>(count);

//        for (int i = 0; i < count; ++i)
//        {
//            instances[i] = Object.Instantiate(prefab);
//            instances[i].gameObject.SetActive(false);
//            instances[i].poolID = i;
//            instances[i].pool = this;

//            m_FreeIdx.Push(i);
//        }
//    }

//    public T GetNew()
//    {
//        int idx = m_FreeIdx.Pop();
//        instances[idx].gameObject.SetActive(true);

//        return instances[idx];
//    }

//    public void Free(T obj)
//    {
//        m_FreeIdx.Push(obj.poolID);
//        instances[obj.poolID].gameObject.SetActive(false);
//    }
//}

//public interface IPooled<T> where T : MonoBehaviour, IPooled<T>
//{
//    int poolID { get; set; }
//    ObjectPooler<T> pool { get; set; }
//}
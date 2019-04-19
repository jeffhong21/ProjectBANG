//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace ObjectPool
//{
//    public class ObjectPool
//    {

//        private Queue<GameObject> m_Pool;
//        private GameObject m_Prefab;        //  The object to instantiate.
//        private Transform m_Host;           //  The parent object of all the objects.


//        public int Count{
//            get { return m_Pool.Count; }
//        }



//        public ObjectPool(GameObject prefab, Transform host, int initialInstanceCount)
//        {
//            m_Pool = new Queue<GameObject>(initialInstanceCount);
//            m_Prefab = prefab;
//            m_Host = host;

//            //Instantiate and queue up the initial number of entities
//            for (int i = 0; i < initialInstanceCount; i++){
//                m_Pool.Enqueue(CreateInstance(m_Host));
//            }
//        }


//        public GameObject CreateInstance(Transform host)
//        {
//            GameObject obj = GameObject.Instantiate(m_Prefab);
//            obj.transform.SetParent(host);
//            obj.SetActive(false);
//            return obj;
//        }



//        public GameObject Get(Vector3 position, Quaternion rotation, Transform parent = null)
//        {
//            GameObject obj;
//            if (m_Pool.Count > 0){
//                obj = m_Pool.Dequeue();
//            } else{
//                obj = CreateInstance(parent);
//            }
//            obj.gameObject.SetActive(true);
//            obj.transform.position = position;
//            obj.transform.rotation = rotation;

//            return null;
//        }


//        public void Return(GameObject obj)
//        {
//            obj.transform.SetParent(m_Host.transform);

//            obj.transform.localPosition = Vector3.zero;
//            obj.transform.localEulerAngles = Vector3.zero;
//            obj.transform.localScale = Vector3.one;
//            obj.gameObject.SetActive(false);

//            m_Pool.Enqueue(obj.gameObject);
//        }

//    }
//}


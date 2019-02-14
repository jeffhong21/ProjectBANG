namespace CharacterController
{
    using UnityEngine;
    using System.Collections.Generic;

    public class ItemEquipSlot : MonoBehaviour
    {
        [SerializeField]
        protected int m_ID;
        [SerializeField]
        protected Transform[] m_CustomEquipPoints;
        protected Dictionary<string, Transform> m_EquipPoints;

        public int ID{
            get { return m_ID; }
        }



		protected void Awake()
		{
            m_EquipPoints = new Dictionary<string, Transform>();
		}


		protected void OnEnable()
		{
            GetEquipPoints();
		}


		protected void OnValidate()
		{
            int count = transform.childCount;
            m_CustomEquipPoints = new Transform[count];
            for (int i = 0; i < count; i++)
                m_CustomEquipPoints[i] = transform.GetChild(i);
		}


        protected void GetEquipPoints()
        {
            foreach(Transform child in transform){
                if(!m_EquipPoints.ContainsKey(child.name))
                    m_EquipPoints.Add(child.name, child);
            }
        }


        public Transform CreateEquipPoint(string name)
        {
            var equipPoint = new GameObject(name).transform;
            equipPoint.parent = transform.parent;
            equipPoint.localPosition = Vector3.zero;
            equipPoint.localEulerAngles = Vector3.zero;

            GetEquipPoints();

            return equipPoint;
        }

        public Transform GetEquipPoint(string name)
        {
            if (m_EquipPoints.ContainsKey(name)){
                return m_EquipPoints[name];
            }

            return null;
        }
	}
}
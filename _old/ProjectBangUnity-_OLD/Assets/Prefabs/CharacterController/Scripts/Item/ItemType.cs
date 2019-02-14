namespace CharacterController
{
    using JH_Utils;
    using UnityEngine;


    public abstract class ItemType : ScriptableObject
    {
        protected string m_Name;
        [SerializeField, DisplayOnly, Tooltip("Which hand this item should be assigned too.  -1 means not assigned to a hand")]
        protected int m_ID = -1;
        [SerializeField, Tooltip("Max amount inventory can hold.")]
        protected int m_Capacity;
        [SerializeField]
        protected bool m_Stackable;





        public string Name{
            get { return m_Name; }
            set { m_Name = value; }
        }

        public int ID{
            get { return m_ID; }
            set { m_ID = value; }
        }

        public bool Stackable{
            get { return m_Stackable; }
        }


        public abstract int GetCapacity();

    }

}

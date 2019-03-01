namespace CharacterController
{
    using UnityEngine;

    /// <summary>
    /// Default class for Items.
    /// </summary>
    public abstract class Item : ScriptableObject
    {
        [SerializeField]
        protected int m_ID;
        [SerializeField]
        protected string m_ItemName;
        [SerializeField]
        protected string m_Description;
        [SerializeField]
        protected Sprite m_Icon;

        [SerializeField, Tooltip("Max amount inventory can hold.")]
        protected int m_Capacity = 1;



        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        public string ItemName{
            get { return m_ItemName; }
            set { m_ItemName = value; }
        }

        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        public Sprite Icon
        {
            get { return m_Icon; }
            set { m_Icon = value; }
        }



        public int Capacity
        {
            get { return m_Capacity; }
            set { m_Capacity = Mathf.Clamp(value, 0, int.MaxValue); }
        }




        public abstract int GetCapacity();

    }

}

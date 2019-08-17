namespace CharacterController
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ItemType", menuName = "Character Controller/Item Type", order = 1000)]
    public class ItemType : ScriptableObject
    {
        

        [SerializeField, DisplayOnly]
        protected int m_ID = -1;
        [SerializeField, DisplayOnly]
        protected string m_Description;
        [SerializeField, Tooltip("Max amount inventory can hold."), Min(0)]
        protected int m_Capacity = int.MaxValue;
        [SerializeField, Tooltip("Does the item take an additional item slot?")]
        protected bool m_Stackable;

        [SerializeField]
        protected UseableConsumableItem m_ConsumableItem;


        //
        //  Properties
        //
        public int ID => m_ID;

        public int Capacity{
            get { return m_Capacity; }
            set { m_Capacity = Mathf.Clamp(value, 0, int.MaxValue); }
        }

        public UseableConsumableItem ConsumableItem{
            get { return m_ConsumableItem; }
            set { m_ConsumableItem = value; }
        }






        [System.Serializable]
        public class UseableConsumableItem
        {
            [SerializeField, Tooltip("The type of consumable item the primary item uses.")]
            protected ItemType m_ItemType;
            [SerializeField, Tooltip("The max amount of consumable item type the primary item can hold.")]
            protected int m_Capacity = int.MaxValue;


            public ItemType ItemType
            {
                get { return m_ItemType; }
                set { m_ItemType = value; }
            }

            public int Capacity
            {
                get { return m_Capacity; }
                set { m_Capacity = value; }
            }



            public UseableConsumableItem(ItemType itemType, int capacity)
            {
                m_ItemType = itemType;
                m_Capacity = capacity;
            }
        }
    }

}

namespace CharacterController
{
    using UnityEngine;
    using System;


    public class PrimaryItemType : ItemType
    {
        [SerializeField]
        protected Item m_ItemObject;        //  variable for the game object.
        protected Item m_Item;              //  variabble for the instamce.
        [SerializeField]
        protected UseableConsumableItem m_ConsumableItem;



        public Item Item
        {
            get { return m_Item; }
        }

        public UseableConsumableItem ConsumableItem
        {
            get { return m_ConsumableItem; }
            set { m_ConsumableItem = value; }
        }


        //
        //  Methods
        //
        public override int GetCapacity()
        {
            if (m_Stackable) 
                return 1;
            return m_Capacity;
        }


        public Item CreateItem(Transform parent)
        {
            if (m_ItemObject == null){
                Debug.LogErrorFormat("{0} requires a Item prefab.", GetType());
                return null;
            }

            m_Item = Instantiate(m_ItemObject).GetComponent<Item>();
            m_Item.transform.parent = parent;
            m_Item.transform.localPosition = m_Item.ItemPosition.position;
            m_Item.transform.localEulerAngles = m_Item.ItemPosition.rotation;

            return m_Item;
        }




        [Serializable]
        public class UseableConsumableItem
        {
            [SerializeField, Tooltip("The type of consumable item the primary item uses.")]
            protected ConsumableItemType m_ItemType;
            [SerializeField]
            protected int m_CurrentAmount;
            [SerializeField, Tooltip("The max amount of consumable item type the primary item can hold.")]
            protected int m_Capacity;


            public ConsumableItemType ItemType
            {
                get { return m_ItemType; }
                set { m_ItemType = value; }
            }

            public int CurrentAmount
            {
                get { return m_CurrentAmount; }
                set { m_CurrentAmount = Mathf.Clamp(value, 0, m_Capacity); }
            }

            public int Capacity
            {
                get { return m_Capacity; }
                set { m_Capacity = value; }
            }



            public UseableConsumableItem(ConsumableItemType itemType, int capacity)
            {
                m_ItemType = itemType;
                m_Capacity = capacity;
            }
        }
    }

}

namespace CharacterController
{
    using UnityEngine;
    using System;


    public class PrimaryItem : ItemType
    {
        [SerializeField]
        protected GameObject m_OriginalObject;
        [SerializeField]
        protected string m_CustomEquipPoint;
        [SerializeField]
        protected UseableConsumableItem m_ConsumableItem;




        public GameObject OriginalObject
        {
            get { return m_OriginalObject; }
            set { m_OriginalObject = value; }
        }


        public string CustomEquipPoint
        {
            get { return m_CustomEquipPoint; }
            set { m_CustomEquipPoint = value; }
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
            return 1;
        }









        [Serializable]
        public class UseableConsumableItem
        {
            [SerializeField, Tooltip("The type of consumable item the primary item uses.")]
            protected ConsumableItem m_ItemType;
            [SerializeField, Tooltip("The max amount of consumable item type the primary item can hold.")]
            protected int m_Capacity;


            public ConsumableItem ItemType
            {
                get { return m_ItemType; }
                set { m_ItemType = value; }
            }

            public int Capacity
            {
                get { return m_Capacity; }
                set { m_Capacity = value; }
            }



            public UseableConsumableItem(ConsumableItem itemType, int capacity)
            {
                m_ItemType = itemType;
                m_Capacity = capacity;
            }
        }
    }

}

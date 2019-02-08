namespace CharacterController
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;


    [CreateAssetMenu(menuName = "Character Locomotion/Item Manager")]
    public class ItemManager : ScriptableObject
    {
        public static ItemManager Instance;

        public static string SaveDirectory = "Assets/Prefabs/CharacterController/Resources/ItemTypes/";
        public static string StorageFolder = "ItemTypes";

        [SerializeField]
        private ItemType[] m_ItemTypes;

        private Dictionary<string, ItemType> m_ItemLookup;







		private void OnEnable()
		{
            Initialize();
		}


		public void Initialize()
        {
            if (m_ItemLookup == null) m_ItemLookup = new Dictionary<string, ItemType>();

            m_ItemTypes = Resources.LoadAll<ItemType>(StorageFolder);

            for (int i = 0; i < m_ItemTypes.Length; i++)
            {
                if(!m_ItemLookup.ContainsKey(m_ItemTypes[i].name)){
                    m_ItemLookup.Add(m_ItemTypes[i].name, m_ItemTypes[i]);
                }
            }
        }








        //[MenuItem("Assets/Create/Character Locomotion/Item Manager", false, 140)]
        //public static void CreateItemManager()
        //{
        //    ItemManager itemManager = ScriptableObject.CreateInstance<ItemManager>();
        //    string directory = AssetDatabase.GenerateUniqueAssetPath(SaveDirectory + "ItemManager" + ".asset");

        //    AssetDatabase.CreateAsset(itemManager, directory);
        //    AssetDatabase.SaveAssets();
        //}


        [MenuItem("Assets/Create/Character Locomotion/Item Types/Primary Item", false, 150)]
        public static void CreatePrimaryItemType()
        {
            PrimaryItemType item = CreateItemType<PrimaryItemType>("PrimaryItemType");
            item.ConsumableItem.Capacity = 1;
        }


        [MenuItem("Assets/Create/Character Locomotion/Item Types/Consumable Item", false, 160)]
        public static void CreateConsumableItemType()
        {
            ConsumableItemType item = CreateItemType<ConsumableItemType>("ConsumableItemType");
        }



        public static T CreateItemType<T>(string itemName)  where T : ItemType
        {
            T item = ScriptableObject.CreateInstance<T>();
            string directory = AssetDatabase.GenerateUniqueAssetPath(SaveDirectory + itemName + ".asset");


            AssetDatabase.CreateAsset(item, directory);
            AssetDatabase.SaveAssets();

            //item.ID

            return item;
        }
    }

}

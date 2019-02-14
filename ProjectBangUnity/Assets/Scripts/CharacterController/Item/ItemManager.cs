namespace CharacterController
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;
    using System.IO;


    public class ItemManager : ScriptableObject
    {
        private static ItemManager m_Instance;
        private static string m_StorageFolder = "Assets/Prefabs/Items/";


        public static ItemManager Instance{
            get { return m_Instance; }
            private set{ if(m_Instance == null) m_Instance = value; }
        }

        public static string StorageFolder{
            get{ return m_StorageFolder; }
            set{ if (Directory.Exists(value)) m_StorageFolder = value; }
        }



        [SerializeField]
        private Item[] m_Items;
        private Dictionary<string, Item> m_ItemLookup;







		private void OnEnable()
		{
            Initialize();
		}


		public void Initialize()
        {
            if (m_ItemLookup == null) m_ItemLookup = new Dictionary<string, Item>();

            m_Items = Resources.LoadAll<Item>(StorageFolder);

            for (int i = 0; i < m_Items.Length; i++)
            {
                if(!m_ItemLookup.ContainsKey(m_Items[i].name)){
                    m_ItemLookup.Add(m_Items[i].name, m_Items[i]);
                }
            }
        }







        public static T CreateItemType<T>(string itemName) where T : Item
        {
            if (Directory.Exists(StorageFolder)){
                T item = ScriptableObject.CreateInstance<T>();
                var directory = AssetDatabase.GenerateUniqueAssetPath(StorageFolder + itemName + ".asset");
                AssetDatabase.CreateAsset(item, directory);
                AssetDatabase.SaveAssets();

                item.ItemName = itemName;


                return item;
            }
            else{
                Debug.LogFormat("**  Storage Folder \"{0}\" does not exist", StorageFolder);
                return null;
            }
        }



        [MenuItem("Assets/Create/Character Controller/Items/Item Manager", false, 140)]
        public static void CreateItemManager()
        {
            if (Directory.Exists(StorageFolder)){
                if(ItemManager.Instance == null){
                    ItemManager itemManager = ScriptableObject.CreateInstance<ItemManager>();
                    string directory = AssetDatabase.GenerateUniqueAssetPath(StorageFolder + "Item Manager" + ".asset");
                    AssetDatabase.CreateAsset(itemManager, directory);
                    AssetDatabase.SaveAssets();
                    ItemManager.Instance = itemManager;
                } else{
                    Debug.LogFormat("**  Item Manager exists already", StorageFolder);
                }
            } else {
                Debug.LogFormat("**  Cannot Create Item Manager.  Storage Folder \"{0}\" does not exist", StorageFolder);
            }
        }


        [MenuItem("Assets/Create/Character Controller/Items/Primary Item", false, 0)]
        public static void CreatePrimaryItem()
        {
            PrimaryItem item = CreateItemType<PrimaryItem>("PrimaryItem");
            item.ConsumableItem.Capacity = 1;
        }


        [MenuItem("Assets/Create/Character Controller/Items/Consumable Item", false, 0)]
        public static void CreateConsumableItem()
        {
            ConsumableItem item = CreateItemType<ConsumableItem>("ConsumableItem");
        }




    }

}

namespace CharacterController.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using TMPro;

    public class ItemHUD : MonoBehaviour
    {
        [SerializeField]
        protected Inventory m_Inventory;
        [SerializeField]
        private TextMeshProUGUI m_ItemName;
        [SerializeField]
        private TextMeshProUGUI m_ItemInfo;





		private void Awake()
		{
            if (m_Inventory == null){
                m_Inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
            }
		}


		private void OnEnable()
		{
            if (m_Inventory == null)
                m_Inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
            

            if(m_Inventory != null){
                m_Inventory.InventoryUseItem += UseItem;
                m_Inventory.InventoryEquipItem += EquipItem;
            }
		}

		private void OnDisable()
		{
            if (m_Inventory != null)
            {
                m_Inventory.InventoryUseItem -= UseItem;
                m_Inventory.InventoryEquipItem -= EquipItem;
            }
		}

		private void Start()
		{
			
		}

		public void SetInventory(Inventory inventory){
            m_Inventory = inventory;
            m_Inventory.InventoryUseItem += UseItem;
            m_Inventory.InventoryEquipItem += EquipItem;
        }



        private void UseItem(ItemType itemType, float remaining)
        {
            PrimaryItem item = (PrimaryItem)itemType;
            m_ItemInfo.text = remaining + " / " + item.ConsumableItem.Capacity.ToString();
        }

        private void EquipItem(Item item)
        {
            m_ItemInfo.text = item.name;
        }

	}
}
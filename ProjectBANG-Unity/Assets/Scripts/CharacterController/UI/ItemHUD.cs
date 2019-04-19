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
        private TextMeshProUGUI m_CurrentAmmo;
        [SerializeField]
        private TextMeshProUGUI m_TotalAmmo;



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
            //PrimaryItem primaryItem = (PrimaryItem)itemType;
            ////Debug.LogFormat("{0} was used.  {1} out of {2} remain loaded.", itemType.name, remaining, primaryItem.ConsumableItem.Capacity);

            ////m_CurrentAmmo.text = remaining + " | " + primaryItem.ConsumableItem.Capacity.ToString();
            //m_CurrentAmmo.text = remaining.ToString();
        }

        private void EquipItem(Item item)
        {
            //if(item == null){
            //    m_CurrentAmmo.text = "";
            //    m_ItemName.text = "Unarmed";
            //} else {
            //    //var shootable = (ShootableWeapon)item;
            //    m_ItemName.text = item.ItemType.name;
            //    var primaryWeapon = (PrimaryItem)item.ItemType;
            //    m_TotalAmmo.text = primaryWeapon.ConsumableItem.Capacity.ToString();
            //    //m_CurrentAmmo.text = string.Format("{0} | {1}", shootable.CurrentAmmo, item.ItemType.ConsumableItem.Capacity);
            //}

        }

	}
}
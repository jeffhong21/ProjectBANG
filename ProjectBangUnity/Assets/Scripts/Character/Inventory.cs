namespace CharacterController
{
    using UnityEngine;


    public class Inventory : MonoBehaviour
    {
        //
        // Fields
        //
        [SerializeField]
        protected ShootableWeapon m_CurrentItem;
        protected Animator m_Animator;



        //
        // Methods
        //
		private void Awake()
		{
            m_Animator = GetComponent<Animator>();
		}


        public void PickupItem()
        {
            
        }

        public void UseItem()
        {
            
        }


        public void ReloadItem(int amount)
        {
            
        }

        public void SwitchItem(bool primary, bool next)
        {
            
        }


        public void EquipItem()
        {
            
        }


        public void UnequipCurrentItem()
        {
            
        }



    }

}
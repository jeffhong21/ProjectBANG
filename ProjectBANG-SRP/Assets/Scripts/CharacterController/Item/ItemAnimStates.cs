namespace CharacterController
{
    using System;
    using UnityEngine;

    [Serializable]
    public class ItemAnimStates
    {
        [SerializeField]
        protected string m_DefaultStateName = "Idle";
        [SerializeField]
        protected string m_AimStateName = "Aim";
        [SerializeField]
        protected string m_EquipStateName = "Equip";
        [SerializeField]
        protected string m_UnequipStateName = "Unequip";


        public string DefaultStateName
        {
            get { return m_DefaultStateName; }
        }

        public string AimStateName
        {
            get { return m_AimStateName; }
        }

        public string EquipStateName
        {
            get { return m_EquipStateName; }
        }

        public string UnequipStateName
        {
            get { return m_UnequipStateName; }
        }


        public ItemAnimStates()
        {
            m_DefaultStateName = "Idle";
            m_AimStateName = "Aim";
            m_EquipStateName = "Equip";
            m_UnequipStateName = "Unequip";
        }


    }

}

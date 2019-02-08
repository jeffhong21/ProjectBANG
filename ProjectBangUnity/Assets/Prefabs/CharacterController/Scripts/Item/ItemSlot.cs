namespace CharacterController
{
    using UnityEngine;
    using System.Collections;

    public class ItemSlot : MonoBehaviour
    {
        [SerializeField]
        protected int m_ID;


        public int ID{
            get { return m_ID; }
        }


    }
}
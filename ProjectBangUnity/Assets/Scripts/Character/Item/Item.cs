namespace CharacterController
{
    using UnityEngine;


    public class Item : MonoBehaviour
    {
        //
        // Fields
        //
        protected string m_ItemName;
        protected GameObject m_CrosshairsSprite;
        protected Transform m_NonDominantHandPosition;
        protected Transform m_HolsterTarget;
        protected GameObject m_ItemPickup;

        protected GameObject m_GameObject;
        protected Transform m_Transform;
        protected Animator m_Animator;
        protected GameObject m_Character;
        protected CharacterLocomotion m_Controller;
        protected Inventory m_Inventory;


        public string ItemName
        {
            get { return m_ItemName; }
            set { m_ItemName = value; }
        }

        public Transform HandTransform
        {
            get;
        }

        public Transform NonDominantHandPosition
        {
            get { return m_NonDominantHandPosition; }
        }

        public GameObject ItemPickup
        {
            get { return m_ItemPickup; }
        }

        public GameObject CrosshairsSprite
        {
            get { return m_CrosshairsSprite; }
        }



        //
        // Methods
        //
        public virtual void Awake()
        {
            m_GameObject = gameObject;
            m_Transform = transform;
            m_Character = gameObject;
            m_Controller = GetComponent<CharacterLocomotion>();
            m_Animator = GetComponent<Animator>();
        }

        public virtual void Initialize(Inventory inventory)
        {
            m_Inventory = inventory;
        }


        protected virtual void ItemActivated()
        {

        }

        protected virtual void ItemDeactivated()
        {

        }

        /// <summary>
        /// Callback from the controller when the item starts to aim.
        /// </summary>
        protected virtual void OnStartAim()
        {
            
        }


        /// <summary>
        /// Callback from the controller when the item is aimed or no longer aimed.
        /// </summary>
        /// <param name="aim">If set to <c>true</c> aim.</param>
        protected virtual void OnAim(bool aim)
        {
            
        }

    }

}
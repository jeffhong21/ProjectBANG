namespace CharacterController
{
    using UnityEngine;


    public class Reload : CharacterAction
    {
        [Header("--  Reload Action Settings --")]
        [SerializeField]
        protected const int m_ItemStateID = 2;
        [SerializeField]
        protected Item m_Item;
        [SerializeField]
        protected string m_ItemName;
        [SerializeField]
        protected int m_LayerIndex;
        [Header("--  States --")]
        [SerializeField]
        protected bool m_IsReloading;





		//
		// Methods
		//
        protected virtual void Start()
        {
            m_LayerIndex = m_AnimatorMonitor.UpperBodyLayerIndex;
        }

        public override void StartAction()
        {
            m_IsActive = true;

            ActionStarted();
            EventHandler.ExecuteEvent(m_GameObject, "OnCharacterActionActive", this, true);

            m_Animator.CrossFade(Animator.StringToHash(GetDestinationState(m_LayerIndex)), m_TransitionDuration, m_LayerIndex);
        }

        public override void StopAction()
        {
            m_IsActive = false;
            ActionStopped();
            EventHandler.ExecuteEvent(m_GameObject, "OnCharacterActionActive", this, false);
        }




		public override bool CanStartAction()
		{
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.LogFormat("Reloading {0}", m_Inventory.EquippedItemType);
                if (!m_IsActive && m_Inventory.GetCurrentItem() != null && !m_IsReloading)
                {
                    return true;
                }
            }
            return false;
		}


		protected override void ActionStarted()
        {
            Debug.LogFormat("Reloading {0}", m_Inventory.EquippedItemType);

            m_ItemName = GetItemName();
            m_IsReloading = true;
            m_AnimatorMonitor.SetItemStateIndex(m_ItemStateID);
        }



		public override bool CanStopAction()
        {
            if (m_IsActive && m_IsReloading){
                if (m_Animator.GetCurrentAnimatorStateInfo(m_LayerIndex).shortNameHash == m_StateHash){
                    //Debug.LogFormat("Current Hash: {0} | {1} Hash: {2}", m_Animator.GetCurrentAnimatorStateInfo(m_AnimatorMonitor.UpperBodyLayerIndex).shortNameHash, GetType().Name, m_StateHash);
                    if (GetNormalizedTime() >= 1 - m_TransitionDuration)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected override void ActionStopped()
        {
            //m_ItemName = "<Empty>";
            m_IsReloading = false;
            m_AnimatorMonitor.SetItemID(0);

        }








        public override string GetDestinationState(int layer)
        {
            string fullStateName = string.Format("{0}.{1}.{2}", m_AnimatorMonitor.UpperBodyLayerName, GetItemName(), "Reload");
            return fullStateName;
        }


        public override float GetNormalizedTime()
        {
            return m_Animator.GetCurrentAnimatorStateInfo(m_LayerIndex).normalizedTime % 1; ;
        }


        protected int GetItemID()
        {
            var itemObject = m_Inventory.GetCurrentItem();
            if (itemObject == null)
                return 0;

            var itemID = itemObject.ItemID;
            return itemID;
        }


        protected string GetItemName()
        {
            var itemObject = m_Inventory.GetCurrentItem();
            if (itemObject == null)
                return null;

            var itemName = itemObject.ItemAnimName;
            return itemName;
        }
    }

}
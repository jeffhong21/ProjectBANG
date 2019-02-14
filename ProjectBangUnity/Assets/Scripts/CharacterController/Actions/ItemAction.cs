namespace CharacterController
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;


    public class ItemAction : MonoBehaviour
    {
        [SerializeField]
        protected Item m_Item;
        [SerializeField]
        protected int m_LayerIndex;
        [SerializeField]
        protected string m_StateName;
        [SerializeField]
        protected float m_TransitionDuration = 0.2f;
        protected AnimatorTransitionInfo m_TransitionInfo;

        protected Inventory m_Inventory;
        protected Animator m_Animator;
        protected AnimatorMonitor m_AnimatorMonitor;
        protected CharacterLocomotion m_Controller;
        protected LayerManager m_LayerManager;



        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_AnimatorMonitor = GetComponent<AnimatorMonitor>();
            m_Controller = GetComponent<CharacterLocomotion>();
            m_LayerManager = GetComponent<LayerManager>();
            m_Inventory = GetComponent<Inventory>();

        }


        //public virtual bool CanStartAction()
        //{
        //    if (Input.GetKeyDown(KeyCode.Mouse1))
        //    {
        //        return true;
        //    }
        //    else if (Input.GetKeyDown(KeyCode.Mouse0))
        //    {
        //        return true;
        //    }
        //    else if (Input.GetKeyDown(KeyCode.R))
        //    {
        //        return true;
        //    }
        //    else if (Input.GetKeyDown(KeyCode.Q))
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        public void StartAction()
        {
            
        }

        public virtual bool CanStopAction()
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(m_LayerIndex).fullPathHash == Animator.StringToHash(""))
            {
                m_TransitionInfo = m_Animator.GetAnimatorTransitionInfo(m_LayerIndex);

                if (GetNormalizedTime(m_LayerIndex) > 1 - m_TransitionInfo.duration)
                {
                    return true;
                }
            }


            return false;
        }

        public void StopAction()
        {
            
        }

        public virtual bool UpdateAnimator()
        {
            return true;
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

            var itemName = itemObject.ItemName;
            return itemName;
        }

        public float GetNormalizedTime(int layer)
        {
            //float normalizedTime = m_Animator.GetCurrentAnimatorStateInfo(layer).normalizedTime % 1;
            return m_Animator.GetCurrentAnimatorStateInfo(layer).normalizedTime % 1;
        }


        public void UseItem()
        {
            
        }

        public void Reload()
        {
            string stateName = string.Format("{0}.{1}.{2}", m_AnimatorMonitor.UpperBodyLayerName, GetItemName(), "Reload");
            m_Animator.CrossFade(Animator.StringToHash(stateName), m_TransitionDuration, m_LayerIndex);
        }

        public void Equip()
        {
            string stateName = string.Format("{0}.{1}.{2}", m_AnimatorMonitor.UpperBodyLayerName, GetItemName(), "Equip");
            m_Animator.CrossFade(Animator.StringToHash(stateName), m_TransitionDuration, m_LayerIndex);
        }

        public void Unequip()
        {
            string stateName = string.Format("{0}.{1}.{2}", m_AnimatorMonitor.UpperBodyLayerName, GetItemName(), "Unequip");
            m_Animator.CrossFade(Animator.StringToHash(stateName), m_TransitionDuration, m_LayerIndex);
        }


    }

}
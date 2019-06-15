//namespace CharacterController
//{
//    using UnityEngine;
//    using UnityEngine.Events;
//    using System;
//    using System.Collections.Generic;


//    public class ItemAction_OLD : MonoBehaviour
//    {
//        public enum AnimationLayers { BaseLayer = 0, UpperbodyLayer = 1, AdditiveLayer = 2, FullbodyLayer = 3 };
//        [SerializeField]
//        protected ItemType m_Item;
//        protected int m_LayerIndex;
//        [Header("-- ItemType Action Input --")]
//        [SerializeField]
//        protected KeyCode m_ShootInput = KeyCode.Mouse0;
//        [SerializeField]
//        protected KeyCode m_ReloadInput = KeyCode.R;
//        [SerializeField]
//        protected KeyCode m_SwitchItemInput = KeyCode.Q;
//        [Header("-- Generic Action Input --")]
//        [SerializeField]
//        protected string m_StateName;
//        [SerializeField]
//        protected AnimationLayers m_AnimationLayer;
//        [SerializeField]
//        protected KeyCode m_GenericActionInput = KeyCode.G;
//        [Header("-- Action States --")]
//        [SerializeField]
//        protected bool m_Reloading;
//        [SerializeField]
//        protected bool m_Switching;
//        [SerializeField]
//        protected bool m_Unequipping;
//        [SerializeField]
//        protected bool m_Equipping;

//        protected float m_TransitionDuration = 0.2f;
//        protected AnimatorTransitionInfo m_TransitionInfo;

//        protected Dictionary<string, int> m_ItemActionIDs = new Dictionary<string, int>()
//        {
//            {"Idle",    0 },
//            {"Aim",     1 },
//            {"Use",     2 },
//            {"Reload",  3 },
//            {"Equip",   4 },
//            {"Unequip", 5 },
//            {"Melee",   6 }
//        };


//        protected Inventory m_Inventory;
//        protected Animator m_Animator;
//        protected AnimatorMonitor m_AnimatorMonitor;
//        protected CharacterLocomotion m_Controller;
//        protected LayerManager m_LayerManager;


//        private void Awake()
//        {
//            m_Animator = GetComponent<Animator>();
//            m_AnimatorMonitor = GetComponent<AnimatorMonitor>();
//            m_Controller = GetComponent<CharacterLocomotion>();
//            m_LayerManager = GetComponent<LayerManager>();
//            m_Inventory = GetComponent<Inventory>();

//        }


//        private void Start()
//        {
//            m_LayerIndex = m_AnimatorMonitor.UpperBodyLayerIndex;

//            EventHandler.RegisterEvent(gameObject, "OnAnimatorItemEquip", OnAnimatorItemEquip);
//            EventHandler.RegisterEvent(gameObject, "OnAnimatorItemEquipComplete", OnAnimatorItemEquipComplete);
//        }




//        private void Update()
//        {
//            CanStartAction();

//            IsDoneReloading();

//            CanStopSwitching();
//            //IsDoneSwitching();
//        }

//        protected void CanStartAction()
//        {
//            if (Input.GetKeyDown(m_SwitchItemInput) && !m_Switching && !m_Reloading)
//            {
//                //StartEquipUnequip();
//                SwitchItem();
//            }
//            else if (Input.GetKeyDown(m_ReloadInput) && !m_Switching && !m_Reloading)
//            {
//                ReloadWeapon();
//            }
//            else if (Input.GetKeyDown(m_ShootInput) && !m_Switching && !m_Reloading)
//            {
//                Shoot();
//            }
//            else if (Input.GetKeyDown(m_GenericActionInput) && !m_Switching && !m_Reloading)
//            {
//                PlayGenericAnimation(m_StateName);
//            }
//            else if (Input.GetKeyDown(KeyCode.E))
//            {
//                DebugHashStates();
//            }
//        }


//        public void ReloadWeapon()
//        {
//            //Debug.LogFormat("ReloadWeapon");
//            m_Reloading = true;
//            //m_AnimatorMonitor.SetItemID(GetItemID(), m_ItemActionIDs["Reload"]);

//            var stateName = string.Format("{0}.{1}.{2}.{3}", m_AnimatorMonitor.UpperBodyLayerName, GetItemName(), "Reload", "Reload 1");
//            //var itemName = string.Format("{0}.{1}.{2}", m_AnimatorMonitor.UpperBodyLayerName, GetItemName(), "Reload");
//            //var itemName = string.Format("{0}.{1}", "Reload", "Reload 1");
//            m_Animator.CrossFade(Animator.StringToHash(stateName), m_TransitionDuration, m_LayerIndex);

//            Debug.LogFormat("Reloading: <color=blue> {0} </color> HashID(<color=red> {1} </color>)| Current time: <color=blue> {2} </color>   HashID(<color=red> {3} </color>)",
//                            GetItemName(), Animator.StringToHash(stateName), GetNormalizedTime(m_LayerIndex), m_Animator.GetCurrentAnimatorStateInfo(m_LayerIndex).fullPathHash);

//            m_Inventory.ReloadItem(m_Inventory.EquippedItemType, 1);

//        }


//        public void Shoot()
//        {
//            //var itemName = string.Format("{0}.{1}.{2}", m_AnimatorMonitor.UpperBodyLayerName, GetItemName(), "Shoot");
//            //m_Animator.CrossFade(Animator.StringToHash(itemName), m_TransitionDuration, m_LayerIndex);

//            //Debug.LogFormat("Shooting");
//            m_Inventory.UseItem(m_Inventory.EquippedItemType, 1);
//            //m_AnimatorMonitor.SetItemTrigger();
//            //m_AnimatorMonitor.SetItemID(GetItemID(), m_ItemActionIDs["Shoot"]);
//        }



//        public void SwitchItem()
//        {
//            //Debug.Log(GetItemEnterStateName(m_Inventory.EquippedItemType));
//            //  Get the current ItemType HASH first.  Than switch
//            m_Switching = true;
//            m_Inventory.SwitchItem(true);
//            //m_AnimatorMonitor.SetItemID(GetItemID(), 0);


//        }


//        protected void StartEquipUnequip()
//        {
//            m_Switching = true;
//            //  Get the unequip switch time.
//            m_Inventory.SwitchItem(true);
//            Unequip();

//            //Debug.LogFormat("Switched ItemType: <color=red> {0} </color> | Current Hash: <color=red> {1} </color> | Hash:<color=red> {2} </color>",
//            //GetUnequipStateName(), m_Animator.GetCurrentAnimatorStateInfo(m_LayerIndex).fullPathHash, Animator.StringToHash(GetUnequipStateName()));
//        }


//        protected void Unequip()
//        {
//            var itemObject = m_Inventory.EquippedItemType;
//            var stateName = string.Format("{0}.{1}.{2}", m_AnimatorMonitor.UpperBodyLayerName, GetItemName(), "Unequip");

//            //if(itemObject == null) stateName = string.Format("{0}.{1}.{2}", m_AnimatorMonitor.UpperBodyLayerName, GetItemName(), "Idle");

//            m_Animator.CrossFade(Animator.StringToHash(stateName), m_TransitionDuration, m_LayerIndex);
//            m_Unequipping = true;

//            //Debug.LogFormat("StateName: <color=red> {0} </color> ", stateName);
//        }

//        protected void Equip()
//        {
//            //  Get item to equip id.
//            //m_AnimatorMonitor.SetItemID(GetItemID(), 0);

//            var stateName = string.Format("{0}.{1}.{2}", m_AnimatorMonitor.UpperBodyLayerName, GetItemName(), "Equip");
//            m_Animator.CrossFade(Animator.StringToHash(stateName), m_TransitionDuration, m_LayerIndex);
//            m_Equipping = true;

//            //Debug.LogFormat("StateName: <color=red> {0} </color> ", stateName);
//        }


//        private void OnAnimatorItemEquip()
//        {
//            //m_Switching = true;
//            Debug.Log(m_Inventory.EquippedItemType + " OnAnimatorItemEquip");
//        }
//        private void OnAnimatorItemEquipComplete()
//        {
//            //m_Switching = false;
//            Debug.Log(m_Inventory.EquippedItemType + " OnAnimatorItemEquipComplete");
//        }

//        private void OnAnimatorItemUnequip()
//        {
//            //m_Switching = true;
//            Debug.Log(m_Inventory.EquippedItemType + " OnAnimatorItemUnequip");
//        }
//        private void OnAnimatorItemUnequipComplete()
//        {
//            //m_Switching = false;
//            Debug.Log(m_Inventory.EquippedItemType + " OnAnimatorItemUnequipComplete");
//        }


//        protected void CanStopSwitching()
//        {
//            if (m_Switching)
//            {

//                if (m_Animator.GetCurrentAnimatorStateInfo(m_LayerIndex).fullPathHash == Animator.StringToHash(GetUnequipStateName()))
//                {
//                    m_TransitionInfo = m_Animator.GetAnimatorTransitionInfo(m_LayerIndex);

//                    //Debug.LogFormat("Switching transition duration: {0}", duration);
//                    if (GetNormalizedTime(m_LayerIndex) > 1 - m_TransitionInfo.duration)
//                    //if (GetNormalizedTime(m_LayerIndex) > 0.5f)
//                    {
//                        OnAnimatorItemUnequipComplete();
//                        if (GetItemID() == 0)
//                        {
//                            m_Switching = false;
//                        }
//                    }
//                }

//                else if (m_Animator.GetCurrentAnimatorStateInfo(m_LayerIndex).fullPathHash == Animator.StringToHash(GetEquipStateName()))
//                {
//                    m_TransitionInfo = m_Animator.GetAnimatorTransitionInfo(m_LayerIndex);

//                    //Debug.LogFormat("Switching transition duration: {0}", duration);
//                    if (GetNormalizedTime(m_LayerIndex) > 1 - m_TransitionInfo.duration)
//                    {
//                        m_Switching = false;
//                        OnAnimatorItemEquipComplete();
//                    }
//                }


//            }
//        }




//        private void OnGUI()
//        {
//            //GUILayout.BeginArea(new Rect(5, 5, Screen.width * 0.5f, Screen.height * 0.6f), GUI.skin.box);


//            //var playerInputContent = new GUIContent(string.Format(" Switched ItemType: <color=red> {0} </color> |\n Current Hash: <color=red> {1} </color> |\n Hash:<color=red> {2} </color>",
//            //                                                      GetUnequipStateName(),
//            //                                                      m_Animator.GetCurrentAnimatorStateInfo(m_LayerIndex).fullPathHash,
//            //                                                      Animator.StringToHash(GetUnequipStateName() ) ) );
//            //GUILayout.Label(playerInputContent);

//            //var Equipped = new GUIContent(string.Format(" Switched ItemType: <color=red> {0} </color> |\n Current Hash: <color=red> {1} </color> |\n Hash:<color=red> {2} </color>",
//            //                                            GetEquipStateName(),
//            //                                            m_Animator.GetCurrentAnimatorStateInfo(m_LayerIndex).fullPathHash,
//            //                                            Animator.StringToHash(GetEquipStateName())));
//            //GUILayout.Label(Equipped);

//            //GUILayout.Label(string.Format("Normalized Time: {0}", GetNormalizedTime(m_LayerIndex)));

//            //GUILayout.EndArea();
//        }






//        protected void IsDoneSwitching()
//        {
//            if (m_Switching)
//            {
//                //  Is Unequipping
//                if (m_Unequipping && !m_Equipping)
//                {
//                    if (m_Animator.GetCurrentAnimatorStateInfo(m_LayerIndex).IsName(GetUnequipStateName()))
//                    {
//                        var transitionInfo = m_Animator.GetAnimatorTransitionInfo(m_LayerIndex);
//                        var duration = transitionInfo.duration;
//                        //Debug.LogFormat("Switching transition duration: {0}", duration);
//                        if (GetNormalizedTime(m_LayerIndex) > 1 - duration)
//                        {
//                            m_Unequipping = false;
//                            m_Animator.ResetTrigger(HashID.ItemStateIndexChange);
//                            //  Not equipping an item.
//                            if (m_Inventory.EquippedItemType == null)
//                            {
//                                //m_AnimatorMonitor.SetItemID(GetItemID(), 0);
//                                m_Switching = false;
//                            }
//                            //  equipped item is not null
//                            else
//                            {
//                                Equip();
//                            }
//                        }
//                    }
//                }
//                //  Is Equipping
//                else if (m_Equipping && !m_Unequipping)
//                {
//                    if (m_Animator.GetCurrentAnimatorStateInfo(m_LayerIndex).IsName(GetEquipStateName()))
//                    {
//                        var transitionInfo = m_Animator.GetAnimatorTransitionInfo(m_LayerIndex);
//                        var duration = transitionInfo.duration;
//                        //Debug.LogFormat("Switching transition duration: {0}", duration);
//                        if (GetNormalizedTime(m_LayerIndex) > 1 - duration)
//                        {
//                            m_Equipping = false;
//                            m_Switching = false;
//                            //m_AnimatorMonitor.SetItemID(GetItemID(), 0);
//                            m_Animator.ResetTrigger(HashID.ItemStateIndexChange);
//                        }
//                    }
//                }
//                else
//                {
//                    Debug.LogFormat("It seems switching is on, but unequip and equip are the same value.");
//                    m_Switching = false;
//                }
//            }
//        }



//        public void PlayGenericAnimation(string stateName)
//        {
//            var fullPathStateName = string.Format("{0}.{1}", m_Animator.GetLayerName((int)m_AnimationLayer), stateName);
//            var stateHashID = Animator.StringToHash(fullPathStateName);

//            m_Animator.CrossFade(stateHashID, m_TransitionDuration, (int)m_AnimationLayer);

//            Debug.LogFormat("StateName: {0} | Layer: {1}", fullPathStateName, (int)m_AnimationLayer);
//            //for (int index = 0; index < m_Animator.layerCount; index++)
//            //{
//            //    if (index == (int)m_AnimationLayer){
//            //        m_Animator.CrossFade(stateHashID, m_TransitionDuration, index);
//            //        Debug.LogFormat("Playing: <color=red> {0} </color> | Current Hash: <color=red> {1} </color> | Hash:<color=red> {2} </color>",
//            //                        stateName, m_Animator.GetCurrentAnimatorStateInfo((int)m_AnimationLayer).fullPathHash, stateHashID);
//            //    }

//            //    m_Animator.CrossFade("Empty", m_TransitionDuration, index);
//            //}
//        }


//        public void DebugHashStates()
//        {
//            var itemName = string.Format("{0}.{1}", GetItemName(), "Equip");
//            Debug.LogFormat("ItemAnimName: <color=red> {0} </color> | Current Hash: <color=red> {1} </color> | Hash:<color=red> {2} </color>",
//                            itemName, m_Animator.GetCurrentAnimatorStateInfo(m_LayerIndex).fullPathHash, Animator.StringToHash(itemName));
//        }



//        protected void IsDoneReloading()
//        {
//            if (m_Reloading)
//            {
//                if (m_Animator.GetCurrentAnimatorStateInfo(m_LayerIndex).IsName(GetReloadStateName()))
//                {
//                    if (GetNormalizedTime(m_LayerIndex) > 0.58f && GetNormalizedTime(m_LayerIndex) < 0.6f)
//                    {
//                        var itemName = string.Format("{0}.{1}.{2}.{3}", m_AnimatorMonitor.UpperBodyLayerName, GetItemName(), "Reload", "Reload 1");
//                        //var itemName = string.Format("{0}.{1}", "Reload", "Reload 1");
//                        Debug.LogFormat("<color=blue> -- IsReloading:  {0} </color> HashID(<color=red> {1} </color>)| Current time: <color=blue> {2} </color>   HashID(<color=red> {3} </color>)",
//                                        GetItemName(), Animator.StringToHash(itemName), GetNormalizedTime(m_LayerIndex), m_Animator.GetCurrentAnimatorStateInfo(m_LayerIndex).fullPathHash);
//                    }

//                    if (GetNormalizedTime(m_LayerIndex) > 1 - 0.25f)
//                    {
//                        m_Reloading = false;
//                        //m_AnimatorMonitor.SetItemID(GetItemID(), 0);
//                    }
//                }
//            }
//        }









//        protected string GetReloadStateName()
//        {
//            return string.Format("{0}.{1}.{2}.{3}", m_AnimatorMonitor.UpperBodyLayerName, GetItemName(), "Reload", "Reload 1");
//        }

//        protected string GetEquipStateName()
//        {
//            return string.Format("{0}.{1}.{2}", m_AnimatorMonitor.UpperBodyLayerName, GetItemName(), "Equip");
//        }

//        protected string GetUnequipStateName()
//        {
//            return string.Format("{0}.{1}.{2}", m_AnimatorMonitor.UpperBodyLayerName, GetItemName(), "Unequip");
//        }

//        protected string GetItemEnterStateName(ItemType item)
//        {
//            var enterStateName = "Equip";
//            if (item == null) enterStateName = "Idle";

//            return string.Format("{0}.{1}.{2}", m_AnimatorMonitor.UpperBodyLayerName, GetItemName(), enterStateName);
//        }

//        public float GetNormalizedTime(int layer)
//        {
//            //float normalizedTime = m_Animator.GetCurrentAnimatorStateInfo(layer).normalizedTime % 1;
//            return m_Animator.GetCurrentAnimatorStateInfo(layer).normalizedTime % 1;
//        }

//        public float GetAnimationLength(int layer)
//        {
//            //float clipLength = m_Animator.GetCurrentAnimatorClipInfo(layer)[0].clip.length;
//            return m_Animator.GetCurrentAnimatorClipInfo(layer)[0].clip.length;
//        }


//        protected int GetItemID()
//        {
//            var itemObject = m_Inventory.GetCurrentItem();
//            if (itemObject == null)
//                return 0;

//            var itemID = itemObject.ItemID;
//            return itemID;
//        }

//        protected string GetItemName()
//        {
//            var itemObject = m_Inventory.GetCurrentItem();
//            if (itemObject == null)
//                return "Unarmed";

//            var itemName = itemObject.ItemAnimName;
//            return itemName;
//        }


//        protected void StartAction()
//        {
//            for (int index = 0; index < m_Animator.layerCount; index++)
//            {
//                m_Animator.CrossFade(GetItemID(), m_TransitionDuration, index);
//            }
//        }


//        //public class OnReloadWeapon : UnityEvent
//        //{

//        //}
//    }

//}
namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public static class HashID
    {
        public static readonly int HorizontalInput = Animator.StringToHash("HorizontalInput");
        public static readonly int ForwardInput = Animator.StringToHash("ForwardInput");
        public static readonly int TurnAmount = Animator.StringToHash("TurnAmount");
        public static readonly int ActionID = Animator.StringToHash("ActionID");
        public static readonly int ActionIntData = Animator.StringToHash("ActionIntData");
        public static readonly int ActionFloatData = Animator.StringToHash("ActionFloatData");
        public static readonly int MovementSetID = Animator.StringToHash("MovementSetID");

        public static readonly int ItemID = Animator.StringToHash("ItemID");
        public static readonly int ItemStateIndex = Animator.StringToHash("ItemStateIndex");
        public static readonly int ItemStateIndexChange = Animator.StringToHash("ItemStateIndexChange");
        public static readonly int ItemSubstateIndex = Animator.StringToHash("ItemSubstateIndex");

        public static readonly int Moving = Animator.StringToHash("Moving");
        public static readonly int Aiming = Animator.StringToHash("Aiming");
        public static readonly int Crouching = Animator.StringToHash("Crouching");

        public static readonly int Height = Animator.StringToHash("Height");
        public static readonly int Speed = Animator.StringToHash("Speed");



        public static readonly int ActionChange = Animator.StringToHash("ActionChange");


    }



    public class AnimatorMonitor : MonoBehaviour
    {
        public readonly string BaseLayerName = "Base Layer";
        public readonly string ItemLayerName = "Item Layer";
        public readonly string UpperBodyLayerName = "Upper Body Layer";
        public readonly string AdditiveLayerName = "Additive Layer";
        public readonly string FullBodyLayer = "Full Body Layer";



        private Dictionary<int, string> m_ActionIDLookup = new Dictionary<int, string>()
        {
            { 1, "Jump"},
            //{ 2, "Fall"},
            { 3, "HeightChange"},
            { 4, "Die"},
            { 9, "DamageVisualization"},
            //{ 10, "Interact"},
            //{ 11, "PickupItem"},
            { 12, "Cover"},
            { 15, "Roll"},
            { 16, "Dodge"}
        };



        //
        // Fields
        //
        [SerializeField]
        protected bool m_DebugStateChanges;
        [SerializeField]
        protected float m_HorizontalInputDampTime = 0.1f;
        [SerializeField]
        protected float m_ForwardInputDampTime = 0.1f;
        [SerializeField]
        protected AnimatorStateData m_BaseState = new AnimatorStateData("Movement", 0.2f);
        [SerializeField]
        protected AnimatorStateData m_UpperBodyState = new AnimatorStateData("Idle", 0.2f);
        [SerializeField]
        protected AnimatorStateData m_AdditiveState = new AnimatorStateData("Idle", 0.2f);
        protected Animator m_Animator;
        [SerializeField]
        private AnimatorStateInfo m_AnimStateInfo;

        private string m_BaseDefaultState;
        private string m_UpperBodyDefaultState;
        private string m_AdditiveDefaultState;


        private AnimatorItemBehavior[] m_AnimatorItemBehaviors;


        private Dictionary<string, int> m_DefaultStates = new Dictionary<string, int>();
        private Dictionary<string, int> m_AllStates = new Dictionary<string, int>();



        //
        // Properties
        //
        public float HorizontalInputValue {
            get { return Input.GetAxis("Horizontal"); }
        }

        public float ForwardInputValue{
            get { return Input.GetAxis("Vertical"); }
        }

        //public float YawValue{
        //    get { return m_Animator.GetFloat("Yaw"); }
        //}

        public int BaseLayerIndex{
            get { return m_Animator.GetLayerIndex(BaseLayerName); }
        }

        public int ItemLayerIndex
        {
            get { return m_Animator.GetLayerIndex(ItemLayerName); }
        }

        public int UpperBodyLayerIndex
        {
            get { return m_Animator.GetLayerIndex(UpperBodyLayerName); }
        }

        public int AdditiveLayerIndex
        {
            get { return m_Animator.GetLayerIndex(AdditiveLayerName); }
        }

        public int FullBodyLayerIndex
        {
            get { return m_Animator.GetLayerIndex(FullBodyLayer); }
        }



		//
		// Methods
		//
		private void Awake()
		{
            m_Animator = GetComponent<Animator>();
		}


		public void Start()
        {
            //m_DefaultStates.Add(m_BaseState.Name, Animator.StringToHash(m_BaseState.Name));
            m_AnimatorItemBehaviors = m_Animator.GetBehaviours<AnimatorItemBehavior>();
            for (int i = 0; i < m_AnimatorItemBehaviors.Length; i++)
            {
                m_AnimatorItemBehaviors[i].AnimMonitor = this;
            }


            m_BaseDefaultState = m_BaseState.Name;
            m_UpperBodyDefaultState = m_UpperBodyState.Name;
            m_AdditiveDefaultState = m_AdditiveState.Name;
        }

        public void PlayDefaultState()
        {
            m_Animator.CrossFade(m_BaseDefaultState, m_BaseState.TransitionDuration, BaseLayerIndex);
            m_Animator.CrossFade(m_UpperBodyDefaultState, m_UpperBodyState.TransitionDuration, UpperBodyLayerIndex);
            m_Animator.CrossFade(m_AdditiveDefaultState, m_AdditiveState.TransitionDuration, AdditiveLayerIndex);
        }


        public void DetermineStates()
        {
            //m_BaseState.Name = 


        }


		//private void Update()
		//{
  //          if(m_Animator.GetCurrentAnimatorClipInfo(BaseLayerIndex).Length > 0){
  //              m_BaseState.clipName = m_Animator.GetCurrentAnimatorClipInfo(BaseLayerIndex)[0].clip.name;
  //              m_BaseState.clipLength = m_Animator.GetCurrentAnimatorClipInfo(BaseLayerIndex)[0].clip.length;
  //          }
  //          m_BaseState.fullHashPath = m_Animator.GetCurrentAnimatorStateInfo(BaseLayerIndex).fullPathHash;
  //          m_BaseState.shortNameHash = m_Animator.GetCurrentAnimatorStateInfo(BaseLayerIndex).shortNameHash;
  //          m_BaseState.length = m_Animator.GetCurrentAnimatorStateInfo(BaseLayerIndex).length;
  //          m_BaseState.normalizedTime = m_Animator.GetCurrentAnimatorStateInfo(BaseLayerIndex).normalizedTime % 1;



  //          if (m_Animator.GetCurrentAnimatorClipInfo(UpperBodyLayerIndex).Length > 0){
  //              m_UpperBodyState.clipName = m_Animator.GetCurrentAnimatorClipInfo(UpperBodyLayerIndex)[0].clip.name;
  //              m_UpperBodyState.clipLength = m_Animator.GetCurrentAnimatorClipInfo(UpperBodyLayerIndex)[0].clip.length;
  //          }
                
  //          m_UpperBodyState.length = m_Animator.GetCurrentAnimatorStateInfo(UpperBodyLayerIndex).length;
  //          m_UpperBodyState.normalizedTime = m_Animator.GetCurrentAnimatorStateInfo(UpperBodyLayerIndex).normalizedTime % 1;
		//}


		public virtual bool DetermineState(int layer, AnimatorStateData defaultState, bool checkAbilities, bool baseStart)
        {
            return true;
        }


        public string FormatStateName(string stateName)
        {
            string layerName = BaseLayerName;

            stateName = string.Format("{0}.{1}", layerName, stateName);
            return stateName;
        }


        public void SetHorizontalInputValue(float value)
        {
            m_Animator.SetFloat(HashID.HorizontalInput, value, m_HorizontalInputDampTime, Time.deltaTime);
        }


        public void SetForwardInputValue(float value)
        {
            m_Animator.SetFloat(HashID.ForwardInput, value, m_ForwardInputDampTime, Time.deltaTime);
        }

        public void SetActionID(int value)
        {
            m_Animator.SetInteger(HashID.ActionID, value);
        }

        public void SetIntDataValue(int value)
        {
            m_Animator.SetInteger(HashID.ActionIntData, value);
        }

        public void SetFloatDataValue(float value)
        {
            m_Animator.SetFloat(HashID.ActionFloatData, value);
        }






        public void SetItemID(int itemID)
        {
            m_Animator.SetInteger(HashID.ItemID, itemID);
        }

        public void SetItemID(int itemID, int stateIndex)
        {
            m_Animator.SetInteger(HashID.ItemID, itemID);
            SetItemStateIndex(stateIndex);
        }

        public void SetItemStateIndex(int stateIndex)
        {
            m_Animator.SetInteger(HashID.ItemStateIndex, stateIndex);
            //m_Animator.SetTrigger(HashID.ItemStateIndexChange);
        }

        public void SetItemTrigger()
        {
            m_Animator.SetTrigger(HashID.ItemStateIndexChange);
        }






        public void ExecuteEvent(string eventName)
        {
            //Debug.Log(eventName);
            EventHandler.ExecuteEvent(gameObject, eventName);
        }


        public void SetMovementSetID(int value)
        {
            m_Animator.SetInteger(HashID.MovementSetID, value);
        }


        public void SetHeightValue(float value)
        {
            m_Animator.SetFloat(HashID.Height, Mathf.Clamp01(value));
        }


        public void SetSpeedValue(float value)
        {
            m_Animator.SetFloat(HashID.Speed, value);
        }

        public void SetActionTrigger(string value)
        {
            m_Animator.SetTrigger(value);
        }

        public void SetActionTrigger(int value)
        {
            m_Animator.SetTrigger(value);
        }








        [Serializable]
        public class AnimatorStateData
        {
            //
            // Fields
            //  
            [SerializeField]
            private string m_Name = "Idle";
            [SerializeField]
            private float m_TransitionDuration = 0.2f;
            [SerializeField]
            private float m_SpeedMultiplier = 1f;

            [Header("AnimatorStateInfo")]
            public int fullHashPath;
            public int shortNameHash;
            public string stateName;
            public float length;
            public float normalizedTime;
            public string clipName;
            public float clipLength;


            //
            // Properties
            //  
            public string Name
            {
                get { return m_Name; }
            }

            public float TransitionDuration
            {
                get { return m_TransitionDuration; }
            }

            public float SpeedMultiplier
            {
                get { return m_SpeedMultiplier; }
            }


            //
            // Constructor
            //  
            public AnimatorStateData(string name, float transitionDuration){
                m_Name = name;
                m_TransitionDuration = transitionDuration;
            }



        }
    }



}


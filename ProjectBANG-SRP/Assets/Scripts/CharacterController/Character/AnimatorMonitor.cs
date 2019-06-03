namespace CharacterController
{
    using UnityEngine;
    using UnityEditor.Animations;
    using System;
    using System.Collections.Generic;

    public static class HashID
    {
        public static readonly int InputMagnitude = Animator.StringToHash("InputMagnitude");
        public static readonly int InputAngle = Animator.StringToHash("InputAngle");
        public static readonly int StartAngle = Animator.StringToHash("StartAngle");
        public static readonly int StopLeftUp = Animator.StringToHash("StopLeftUp");
        public static readonly int StopRightUp = Animator.StringToHash("StopRightUp");
        public static readonly int LegUpIndex = Animator.StringToHash("LegUpIndex");
        //public static readonly int Grounded = Animator.StringToHash("Grounded");


        public static readonly int HorizontalInput = Animator.StringToHash("HorizontalInput");
        public static readonly int ForwardInput = Animator.StringToHash("ForwardInput");
        public static readonly int Rotation = Animator.StringToHash("Rotation");
        public static readonly int InputVector = Animator.StringToHash("InputVector");
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

        public static readonly int ColliderHeight = Animator.StringToHash("ColliderHeight");

        public static readonly int ActionChange = Animator.StringToHash("ActionChange");


    }


    [DisallowMultipleComponent]
    public class AnimatorMonitor : MonoBehaviour
    {
        public readonly string BaseLayerName = "Base Layer";
        public readonly string ItemLayerName = "Item Layer";
        public readonly string UpperBodyLayerName = "Upper Body Layer";
        public readonly string AdditiveLayerName = "Additive Layer";
        public readonly string FullBodyLayer = "Full Body Layer";


        private Dictionary<int, string> m_StateNameHash = new Dictionary<int, string>();
        private Dictionary<int, string> m_FullPathNameHash = new Dictionary<int, string>();
        private Dictionary<int, string> m_ShortPathNameHash = new Dictionary<int, string>();



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
        protected AnimatorController m_AnimatorController;







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
            m_AnimatorController = m_Animator.runtimeAnimatorController as AnimatorController;

        }


        private void Start()
        {
            GetAllStateIDs();
        }


        public void GetAllStateIDs(bool debugMsg = false)
        {
            if(m_Animator == null) m_Animator = GetComponent<Animator>();
            AnimatorController animatorController = m_Animator.runtimeAnimatorController as AnimatorController;


            foreach (AnimatorControllerLayer layer in animatorController.layers){
                RegisterAnimatorStates(layer.stateMachine, layer.name);
            }

            if (debugMsg)
            {
                string debugStateInfo = "";

                debugStateInfo += "<b>Short Name Hash: </b>\n";
                foreach (var stateName in m_ShortPathNameHash){
                    debugStateInfo += "<b>StateName:</b> " + stateName.Value + " | <b>shortNameHash:</b> " + stateName.Key + "\n";
                }

                debugStateInfo += "<b>Full Name Hash: </b>\n";
                foreach (var stateName in m_FullPathNameHash){
                    debugStateInfo += "<b>StateName:</b> " + stateName.Value + " | <b>fullNameHash:</b> " + stateName.Key + "\n";
                }
                debugStateInfo += "\n<b>Total State Count: " + stateCount + " </b>\n";

                stateCount = 0;
                Debug.Log(debugStateInfo);
            }

        }

        int stateCount;
        private void RegisterAnimatorStates(AnimatorStateMachine stateMachine, string parentState)
        {
            foreach (ChildAnimatorState childState in stateMachine.states) //for each state
            {
                string stateName = childState.state.name;
                int shortNameHash = Animator.StringToHash(stateName);

                if(m_ShortPathNameHash.ContainsKey(shortNameHash) == false){
                    m_ShortPathNameHash.Add(shortNameHash, stateName);
                }

                int nameHash = childState.state.nameHash;
                if (m_StateNameHash.ContainsKey(nameHash) == false){
                    m_StateNameHash.Add(nameHash, stateName);
                }

                string fullPathName = parentState + "." + stateName;
                int fullPathHash = Animator.StringToHash(fullPathName);

                if (m_FullPathNameHash.ContainsKey(fullPathHash) == false){
                    m_FullPathNameHash.Add(fullPathHash, fullPathName);
                }

                stateCount++;
            }

            foreach (ChildAnimatorStateMachine sm in stateMachine.stateMachines) //for each state
            {
                string path = parentState + "." + sm.stateMachine.name;
                RegisterAnimatorStates(sm.stateMachine, path);
            }
        }


        public string GetShortPathName(int hash)
        {
            if (m_ShortPathNameHash.ContainsKey(hash)){
                return m_ShortPathNameHash[hash];
            }
            return "";
        }

        public string GetFullPathName(int hash)
        {
            if (m_FullPathNameHash.ContainsKey(hash)){
                return m_FullPathNameHash[hash];
            }
            return "";
        }

        public string GetNameHash(int hash)
        {
            if (m_StateNameHash.ContainsKey(hash)){
                return m_FullPathNameHash[hash];
            }
            return "";
        }


        private void LateUpdate()
		{

            for (int layerIndex = 0; layerIndex < m_Animator.layerCount; layerIndex++)
            {
                var animatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(layerIndex);
                var nextAnimatorStateInfo = m_Animator.GetNextAnimatorStateInfo(layerIndex);
                var transitionStateInfo = m_Animator.GetAnimatorTransitionInfo(layerIndex);
                if(m_Animator.IsInTransition(layerIndex) )
                {
                    if (m_DebugStateChanges)
                        Debug.LogFormat("{0} -> {1}", GetShortPathName(animatorStateInfo.shortNameHash), GetShortPathName(nextAnimatorStateInfo.shortNameHash));

                }

                
            }



        }



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
            m_Animator.SetFloat(HashID.HorizontalInput, value, m_HorizontalInputDampTime,  Time.deltaTime);
        }


        public void SetForwardInputValue(float value)
        {
            m_Animator.SetFloat(HashID.ForwardInput, value, m_ForwardInputDampTime,  Time.deltaTime);
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










        public void ExecuteEvent(string eventName)
        {
            //Debug.Log(eventName);
            EventHandler.ExecuteEvent(gameObject, eventName);
        }















        private string DebugGetCurrentStateInfo(int layerIndex, bool addNewLine = false)
        {
            string stateInfo = "";

            string layerName = m_Animator.GetLayerName(layerIndex);
            string fullPathHash = m_Animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash.ToString();
            string shortPathHash = m_Animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash.ToString();

            if (m_FullPathNameHash.ContainsKey(m_Animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash))
            {
                string stateName = m_FullPathNameHash[m_Animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash];
                stateInfo = layerName + " | State Name: " + stateName + "  | fullPathHash <" + fullPathHash + ">, shortPathHash <" + shortPathHash + ">";
            }
            else
            {
                stateInfo = layerName + " | <b>No HASHID</b>  | fullPathHash <" + fullPathHash + ">, shortPathHash <" + shortPathHash + ">";
            }
                



            if (addNewLine)
                stateInfo += "\n";
            return stateInfo;
        }

        private void OnGUI()
        {
            if (!Application.isPlaying) return;
            if (m_DebugStateChanges == false) return;

            int layerIndex = 0;
            int line = 0;

            GUI.color = CharacterControllerUtility.DebugTextColor;
            Rect rect = CharacterControllerUtility.AnimatorMonitorRect;

            GUI.BeginGroup(rect, GUI.skin.box);
            //GUI.BeginGroup(rect);
            GUI.Label(rect, DebugGetCurrentStateInfo(layerIndex), CharacterControllerUtility.GuiStyle);

            if (m_Animator.IsInTransition(layerIndex))
            {
                line++;
                rect.y = rect.y + UnityEditor.EditorGUIUtility.singleLineHeight * line;
                string label = "Transition Duration: < " + m_Animator.GetAnimatorTransitionInfo(layerIndex).duration.ToString() + " >";
                GUI.Label(rect, label, CharacterControllerUtility.GuiStyle);
            }


            if (m_Animator.isMatchingTarget)
            {
                line++;
                rect.y = rect.y + UnityEditor.EditorGUIUtility.singleLineHeight * line;
                GUI.Label(rect, "Currently matching target.", CharacterControllerUtility.GuiStyle);
            }


            GUI.EndGroup();
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


namespace CharacterController
{
    using UnityEngine;
    using UnityEditor.Animations;
    using System;
    using System.Collections.Generic;
    using System.Linq;



    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class AnimatorMonitor : MonoBehaviour
    {

        private readonly Dictionary<int, string> m_StateNameHash = new Dictionary<int, string>();
        private readonly Dictionary<int, AnimatorState> m_AnimatorStates = new Dictionary<int, AnimatorState>();


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
        protected AnimatorStateData[] m_AnimatorStateData = new AnimatorStateData[0];


        [SerializeField, HideInInspector]
        private AnimatorStateData State_1 = new AnimatorStateData("", 0);
        [SerializeField, HideInInspector]
        private AnimatorStateData State_2 = new AnimatorStateData("", 0);


        protected Animator m_Animator;
        protected AnimatorController m_AnimatorController;
        protected CharacterLocomotion m_Controller;

        private int stateCount;




        //
        // Properties
        //
        public float HorizontalInputValue {
            get { return Input.GetAxis("Horizontal"); }
        }

        public float ForwardInputValue{
            get { return Input.GetAxis("Vertical"); }
        }




		//
		// Methods
		//
		private void Awake()
		{
            m_Animator = GetComponent<Animator>();
            m_AnimatorController = m_Animator.runtimeAnimatorController as AnimatorController;
            m_Controller = GetComponent<CharacterLocomotion>();
        }


        private void Start()
        {
            m_AnimatorStateData = new AnimatorStateData[m_Animator.layerCount];
            for (int i = 0; i < m_AnimatorController.layers.Length; i++)
            {
                //if (m_DebugStateChanges)
                //{
                //    Debug.LogFormat("({2}) Animator Layer name: <b>{0}</b> | AnimatorController layer name: <b>{1}</b>",
                //                    m_Animator.GetLayerName(i),
                //                    m_AnimatorController.layers[i].name,
                //                    i);
                //}

                AnimatorStateMachine stateMachine = m_AnimatorController.layers[i].stateMachine;
                AnimatorState defaultState = stateMachine.defaultState;
                if (defaultState == null)
                {
                    defaultState = stateMachine.AddState("Idle", Vector3.zero);
                }

                //string stateName = stateMachine.name + "." + defaultState.name;
                string stateName = defaultState.name;

                m_AnimatorStateData[i] = new AnimatorStateData(defaultState.nameHash, stateName, 0.2f);
            }



            RegisterAllAnimatorStateIDs();
        }


        public void RegisterAllAnimatorStateIDs(bool debugMsg = false)
        {
            if(m_Animator == null) m_Animator = GetComponent<Animator>();
            AnimatorController animatorController = m_Animator.runtimeAnimatorController as AnimatorController;


            foreach (AnimatorControllerLayer layer in animatorController.layers){
                RegisterAnimatorStates(layer.stateMachine, layer.name);
            }


            if (debugMsg)
            {
                string debugStateInfo = "";

                debugStateInfo += "<b>State Name Hash: </b>\n";
                foreach (var stateName in m_StateNameHash){
                    debugStateInfo += "<b>StateName:</b> " + stateName.Value + " | <b>HashID:</b> " + stateName.Key + "\n";
                }

                debugStateInfo += "\n<b>Total State Count: " + stateCount + " </b>\n";

                stateCount = 0;
                Debug.Log(debugStateInfo);
            }

        }


        private void RegisterAnimatorStates(AnimatorStateMachine stateMachine, string parentState)
        {
            foreach (ChildAnimatorState childState in stateMachine.states) //for each state
            {
                string stateName = childState.state.name;
                string fullPathName = parentState + "." + stateName;
                int shortNameHash = Animator.StringToHash(stateName);
                int fullPathHash = Animator.StringToHash(fullPathName);

                if (m_StateNameHash.ContainsKey(shortNameHash) == false)
                {
                    m_StateNameHash.Add(shortNameHash, stateName);
                }
                if (m_StateNameHash.ContainsKey(fullPathHash) == false)
                {
                    m_StateNameHash.Add(fullPathHash, fullPathName);
                }

                if (m_StateNameHash.ContainsKey(fullPathHash) == false)
                {
                    m_AnimatorStates.Add(fullPathHash, childState.state);
                }


                stateCount++;
            }

            foreach (ChildAnimatorStateMachine sm in stateMachine.stateMachines) //for each state
            {
                string path = parentState + "." + sm.stateMachine.name;
                RegisterAnimatorStates(sm.stateMachine, path);
            }
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
                    //if (m_DebugStateChanges)
                        //Debug.LogFormat("{0} -> {1}", GetShortPathName(animatorStateInfo.shortNameHash), GetShortPathName(nextAnimatorStateInfo.shortNameHash));

                    if(GetStateName(nextAnimatorStateInfo.shortNameHash) == m_AnimatorStateData[layerIndex].StateName)
                    {
                        //if(m_DebugStateChanges)
                            //Debug.LogFormat("<color=yellow> {0} </color> is exiting state", GetStateName(animatorStateInfo.fullPathHash) );
                    }
                }

                
            }



        }



        public string GetStateName(int hash)
        {
            if (m_StateNameHash.ContainsKey(hash)){
                return m_StateNameHash[hash];
            }
            return null;
        }


        public virtual bool DetermineState(int layer, AnimatorStateData defaultState, bool checkAbilities, bool baseStart)
        {
            if (m_Animator.IsInTransition(layer))
            {
                if(m_Animator.GetNextAnimatorStateInfo(layer).fullPathHash == defaultState.NameHash)
                {

                }

                Debug.LogFormat("{1} is exiting. | {0} is the next state.", GetStateName(m_Animator.GetNextAnimatorStateInfo(layer).fullPathHash), this.GetType());
                return true;
            }


            throw new NotImplementedException("<color=yellow> AnimatorMonitor </color> FormatStateName() not implemented yet.");

        }


        public string FormatStateName(string stateName)
        {
            throw new NotImplementedException("<color=yellow> AnimatorMonitor </color> FormatStateName() not implemented yet.");
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



        #region Change Parameters

        string parameterInfo;
        int parameterCount;
        public void ChangeParameter(string parameter)
        {
            if (m_Animator == null) m_Animator = GetComponent<Animator>();
            AnimatorController animatorController = m_Animator.runtimeAnimatorController as AnimatorController;

            parameterInfo = "";
            foreach (AnimatorControllerLayer layer in animatorController.layers)
            {
                LoopThroughStateMachines(layer.stateMachine, parameter);
            }


            Debug.LogFormat("{0} used count: {1}\n{2}", parameter, parameterCount, parameterInfo);
            parameterInfo = "";
        }

        private void LoopThroughStateMachines(AnimatorStateMachine stateMachine, string parameter )
        {
            foreach (ChildAnimatorState childState in stateMachine.states) //for each state
            {
                AnimatorStateTransition[] transitions = childState.state.transitions;
                foreach (AnimatorStateTransition transition in transitions)
                {
                    AnimatorCondition[] conditions = transition.conditions;
                    for (int con = 0; con < conditions.Length; con++)
                    {
                        AnimatorCondition condition = conditions[con];
                        if (condition.parameter == parameter)
                        {
                            parameterInfo += childState.state.name + " | " + condition.mode + " | " + condition.threshold + "\n";
                            parameterCount++;
                        }
                    }
                }
            }

            foreach (ChildAnimatorStateMachine sm in stateMachine.stateMachines) //for each state{
                LoopThroughStateMachines(sm.stateMachine, parameter);

        }

        #endregion









        #region Debug
        GUIStyle guiStyle;
        GUIContent guiContent;
        private void OnGUI()
        {
            if (!Application.isPlaying) return;
            if (m_DebugStateChanges == false) return;

            if (guiStyle == null){
                guiStyle = CharacterControllerUtility.GuiStyle;
                guiStyle.fontSize = 28;
            }

            if (guiContent == null){
                guiContent = new GUIContent();
            }

            int layerIndex = 0;
            int line = 0;

            GUI.color = CharacterControllerUtility.DebugTextColor;
            Rect rect = CharacterControllerUtility.AnimatorMonitorRect;

            GUI.BeginGroup(rect, GUI.skin.box);
            //GUI.BeginGroup(rect);
            GUI.Label(rect, DebugGetCurrentStateInfo(layerIndex), guiStyle);

            if (m_Animator.IsInTransition(layerIndex))
            {
                var transitionStateInfo = m_Animator.GetAnimatorTransitionInfo(layerIndex);
                //string nextState = m_StateNameHash[m_Animator.GetAnimatorTransitionInfo(layerIndex).fullPathHash];
                string nextState = GetStateName(m_Animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash);
                line++;

                //string label = " °State Name: " + nextState + "Transition Duration: " + transitionStateInfo.duration.ToString() + " ";
                guiContent.text = " °State Name: " + nextState + "Transition Duration: " + transitionStateInfo.duration.ToString() + " ";
                float lineHeight = guiStyle.CalcHeight(guiContent, rect.width);

                rect.y = rect.y + lineHeight * line;

                GUI.Label(rect, guiContent, guiStyle);
            }


            if (m_Animator.isMatchingTarget)
            {
                line++;
                rect.y = rect.y + UnityEditor.EditorGUIUtility.singleLineHeight * line;
                GUI.Label(rect, "Currently matching target.", CharacterControllerUtility.GuiStyle);
            }


            GUI.EndGroup();
        }





        private string DebugGetCurrentStateInfo(int layerIndex, bool addNewLine = false)
        {
            string stateInfo = "";

            var animatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(layerIndex);
            var nextAnimatorStateInfo = m_Animator.GetNextAnimatorStateInfo(layerIndex);
            var transitionStateInfo = m_Animator.GetAnimatorTransitionInfo(layerIndex);

            string layerName = m_Animator.GetLayerName(layerIndex);
            string fullPathHash = animatorStateInfo.fullPathHash.ToString();
            string shortPathHash = animatorStateInfo.shortNameHash.ToString();

            if (m_StateNameHash.ContainsKey(animatorStateInfo.fullPathHash))
            {
                string stateName = m_StateNameHash[m_Animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash];
                //stateInfo = layerName + " | State Name: " + stateName + "  | fullPathHash <" + fullPathHash + ">, shortPathHash <" + shortPathHash + ">";
                stateInfo = " °State Name: " + stateName + 
                            "  | Duration: " + animatorStateInfo.length + 
                            ", NormalizeTime: " + animatorStateInfo.normalizedTime + 
                            "    °";

            }
            else
            {
                stateInfo = layerName + " | <b>No HASHID</b>  | fullPathHash <" + fullPathHash + ">, shortPathHash <" + shortPathHash + ">";
            }


            if (addNewLine)
                stateInfo += "\n";
            return stateInfo;
        }





        #endregion





    }
}


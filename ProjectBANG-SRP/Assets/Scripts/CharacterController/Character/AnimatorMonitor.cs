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
        public event Action<Vector3, Quaternion> OnMatchTarget;


        private readonly Dictionary<int, string> allStateNameHash = new Dictionary<int, string>();
        private readonly Dictionary<int, AnimatorState> allAnimatorStateHash = new Dictionary<int, AnimatorState>();





        //
        // Fields
        //

        [SerializeField]
        protected float horizontalInputDampTime = 0.1f;
        [SerializeField]
        protected float forwardInputDampTime = 0.1f;
        [SerializeField]
        protected AnimatorStateData[] animatorStateData = new AnimatorStateData[0];

        [SerializeField] protected bool debugStateChanges;
        [SerializeField] protected bool logEvents;

        [Header("Match Target Attributes")]
        [SerializeField]
        private bool hasMatchTarget;
        private Vector3 matchPosition;
        private Quaternion matchRotation;
        private bool applyRootMotion;

        protected Animator m_Animator;
        protected AnimatorController m_AnimatorController;
        protected CharacterLocomotion m_Controller;

        private int stateCount;
        private float deltaTime;



        //
        // Properties
        //
        public float HorizontalInputValue {
            get { return Input.GetAxis("Horizontal"); }
        }

        public float ForwardInputValue{
            get { return Input.GetAxis("Vertical"); }
        }



        public bool HasMatchTarget {  get { return hasMatchTarget; } }


        //
        // Methods
        //
        private void Awake()
		{
            UnityEditor.AssetDatabase.Refresh();

            EventHandler.RegisterEvent<Vector3, Quaternion>(gameObject, "OnSetMatchTarget", SetMatchTarget);


            m_Animator = GetComponent<Animator>();
            m_AnimatorController = m_Animator.runtimeAnimatorController as AnimatorController;
            m_Controller = GetComponent<CharacterLocomotion>();


            StateBehavior[] stateBehaviors = m_Animator.GetBehaviours<StateBehavior>();
            if(stateBehaviors.Length > 0) {
                for (int i = 0; i < stateBehaviors.Length; i++) {
                    stateBehaviors[i].Initialize(this);
                }
            } else {
                Debug.Log("Animator has no state behaviors.");
            }

            deltaTime = m_Animator.updateMode == AnimatorUpdateMode.AnimatePhysics ? Time.fixedDeltaTime : Time.deltaTime;
        }

        private void OnDestroy()
        {
            EventHandler.UnregisterEvent<Vector3, Quaternion>(gameObject, "OnSetMatchTarget", SetMatchTarget);
        }


        private void Start()
        {
            animatorStateData = new AnimatorStateData[m_Animator.layerCount];
            for (int i = 0; i < m_AnimatorController.layers.Length; i++)
            {
                AnimatorStateMachine stateMachine = m_AnimatorController.layers[i].stateMachine;
                AnimatorState defaultState = stateMachine.defaultState;
                if (defaultState == null)
                {
                    defaultState = stateMachine.AddState("Idle", Vector3.zero);
                }

                //string stateName = stateMachine.name + "." + defaultState.name;
                string stateName = defaultState.name;

                animatorStateData[i] = new AnimatorStateData(defaultState.nameHash, stateName, 0.2f);
            }


            AnimatorController animatorController = m_Animator.runtimeAnimatorController as AnimatorController;
            foreach (AnimatorControllerLayer layer in animatorController.layers) {
                RegisterAnimatorStates(layer.stateMachine, layer.name);
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

                if (allStateNameHash.ContainsKey(shortNameHash) == false)
                {
                    allStateNameHash.Add(shortNameHash, stateName);
                }
                if (allStateNameHash.ContainsKey(fullPathHash) == false)
                {
                    allStateNameHash.Add(fullPathHash, fullPathName);
                }

                if (allStateNameHash.ContainsKey(fullPathHash) == false)
                {
                    allAnimatorStateHash.Add(fullPathHash, childState.state);
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
                    //if (debugStateChanges)
                        //Debug.LogFormat("{0} -> {1}", GetShortPathName(animatorStateInfo.shortNameHash), GetShortPathName(nextAnimatorStateInfo.shortNameHash));

                    if(GetStateName(nextAnimatorStateInfo.shortNameHash) == animatorStateData[layerIndex].StateName)
                    {
                        //if(debugStateChanges)
                            //Debug.LogFormat("<color=yellow> {0} </color> is exiting state", GetStateName(animatorStateInfo.fullPathHash) );
                    }
                }

                
            }



        }



        public string GetStateName(int hash)
        {
            if (allStateNameHash.ContainsKey(hash)){
                return allStateNameHash[hash];
            }
            return null;
        }


        protected void SetMatchTarget( Vector3 position, Quaternion rotation )
        {
            hasMatchTarget = true;
            matchPosition = position;
            matchRotation = rotation;
            applyRootMotion = m_Animator.applyRootMotion;
        }


        public void ResetMatchTarget()
        {
            hasMatchTarget = false;
            m_Animator.applyRootMotion = applyRootMotion;
            matchPosition = Vector3.zero;
            matchRotation = Quaternion.identity;
        }


        public bool MatchTarget(Vector3 position, Quaternion rotation,  AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startTime, float endTime, bool force = false )
        {
            SetMatchTarget(position, rotation);
            return MatchTarget(matchPosition, matchRotation, targetBodyPart, weightMask, startTime, endTime, force);
        }


        public bool MatchTarget( AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startTime, float endTime,  bool force = false)
        {
            if (hasMatchTarget == false) return false;

            if (!m_Animator.isMatchingTarget || force) {
                m_Animator.MatchTarget(matchPosition, matchRotation, targetBodyPart, weightMask, startTime, endTime);
                return true;
            }
            return false;
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




        public void SetHorizontalInputValue(float value){
            m_Animator.SetFloat(HashID.HorizontalInput, value, horizontalInputDampTime,  deltaTime);
        }

        public void SetHorizontalInputValue( float value, float dampTime){
            m_Animator.SetFloat(HashID.HorizontalInput, value, dampTime, deltaTime);
        }

        public void SetForwardInputValue(float value){
            m_Animator.SetFloat(HashID.ForwardInput, value, forwardInputDampTime, deltaTime);
        }

        public void SetForwardInputValue( float value, float dampTime ){
            m_Animator.SetFloat(HashID.ForwardInput, value, dampTime, deltaTime);
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
            if(logEvents) Debug.Log(eventName);
            EventHandler.ExecuteEvent(gameObject, eventName);
        }


        public void ItemUsed(int itemTypeIndex)
        {

        }





        //private void OnActionActive( CharacterAction action, bool activated )
        //{
        //    int index = Array.IndexOf(m_Actions, action);
        //    if (action == m_Actions[index]) {
        //        if (m_Actions[index].enabled) {
        //            if (activated) {
        //                //Debug.LogFormat(" {0} is starting.", action.GetType().Name);
        //                CharacterDebug.Log(action.GetType().Name, action.GetType());

        //            } else {
        //                CharacterDebug.Remove(action.GetType().Name);
        //            }
        //        }
        //    }

        //}



        #region Debug

        /// <summary>
        /// Register all animator state ids and print the names.
        /// </summary>
        /// <param name="debugMsg"></param>
        public void RegisterAllAnimatorStateIDs( bool debugMsg = false )
        {
            if (m_Animator == null) m_Animator = GetComponent<Animator>();
            AnimatorController animatorController = m_Animator.runtimeAnimatorController as AnimatorController;


            foreach (AnimatorControllerLayer layer in animatorController.layers) {
                RegisterAnimatorStates(layer.stateMachine, layer.name);
            }

            if (debugMsg) DebugLogAnimatorStates();

        }

        protected void DebugLogAnimatorStates()
        {
            //allStateNameHash.Keys.OrderBy(k => k).ToDictionary(k =>k, k => allStateNameHash[k]);

            //allStateNameHash.OrderByDescending(r => r.Value).ThenBy(r => r.Key);
            allStateNameHash.OrderByDescending(r => r.Value);

            var sortedList = allStateNameHash.ToList();
            sortedList.Sort(( x, y ) => string.Compare(x.Value, y.Value, StringComparison.CurrentCulture));

            string debugStateInfo = "";
            debugStateInfo += "<b>State Name Hash: </b>\n";

            for (int i = 0; i < sortedList.Count; i++) {
                debugStateInfo += "<b>StateName:</b> " + sortedList[i].Value + " | <b>HashID:</b> " + sortedList[i].Key + "\n";
            }


            debugStateInfo += "\n<b>Total State Count: " + stateCount + " </b>\n";

            stateCount = 0;
            Debug.Log(debugStateInfo);
        }





        
        GUIStyle guiStyle;
        GUIContent guiContent;
        private void OnGUI()
        {
            if (!Application.isPlaying) return;
            if (debugStateChanges == false) return;

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
                //string nextState = allStateNameHash[m_Animator.GetAnimatorTransitionInfo(layerIndex).fullPathHash];
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

            if (allStateNameHash.ContainsKey(animatorStateInfo.fullPathHash))
            {
                string stateName = allStateNameHash[m_Animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash];
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


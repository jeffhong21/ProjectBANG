//namespace CharacterController
//{
//    using UnityEngine;
//    using UnityEditor.Animations;
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;



//    [DisallowMultipleComponent]
//    [RequireComponent(typeof(Animator))]
//    public class AnimatorMonitor1 : MonoBehaviour
//    {
//        public static string OnSetMatchTargetEventID = "OnSetMatchTarget";


//        public event Action<Vector3, Quaternion> OnMatchTarget;


//        private Dictionary<int, AnimatorState> allAnimatorStateHash;
//        private Dictionary<int, string> allStateNameHash;
//        private Dictionary<CharacterAction, int> allCharacterActions;
//        [Header("Active Actions")]
//        [SerializeField] private CharacterAction[] activeActions;
//        private HashSet<ItemAction> activeItemActions;
//        [SerializeField] private bool activeAction;
//        [SerializeField] private int activeActionsCount;
//        // store all transitions and states so we know when they have changed
//        private AnimatorTransitionInfo[] previousTransitions;
//        private AnimatorStateInfo[] previousStates;
//        // store all the current states.
//        private AnimatorStateData[] animatorStateData;



//        //
//        // Fields
//        //

//        //[Foldout("Damp Options", true)]
//        public float horizontalInputDampTime = 0.1f;
//        public float forwardInputDampTime = 0.1f;


//        [Header("Debug Options")]
//        [SerializeField] private bool debugStateChanges;
//        [SerializeField] private bool logEvents;

//        [Header("Match Target Attributes")]
//        [SerializeField]
//        private bool hasMatchTarget;
//        private Vector3 matchPosition;
//        private Quaternion matchRotation;
//        private bool applyRootMotion;

//        private Animator animator;
//        private AnimatorController m_AnimatorController;
//        private CharacterLocomotion controller;

//        private int stateCount;
//        private float deltaTime;



//        //
//        // Properties
//        //
//        public float HorizontalInputValue
//        {
//            get { return Input.GetAxis("Horizontal"); }
//        }

//        public float ForwardInputValue
//        {
//            get { return Input.GetAxis("Vertical"); }
//        }



//        public bool HasMatchTarget { get { return hasMatchTarget; } }


//        //
//        // Methods
//        //
//        private void Awake()
//        {
//            UnityEditor.AssetDatabase.Refresh();



//            animator = GetComponent<Animator>();
//            m_AnimatorController = animator.runtimeAnimatorController as AnimatorController;
//            controller = GetComponent<CharacterLocomotion>();

//            //  Initialize statebehaviors.
//            InitializeStateBehaviors();

//            //  Set deltaTime.
//            deltaTime = animator.updateMode == AnimatorUpdateMode.AnimatePhysics ? Time.fixedDeltaTime : Time.deltaTime;


//            EventHandler.RegisterEvent<Vector3, Quaternion>(gameObject, OnSetMatchTargetEventID, SetMatchTarget);
//            EventHandler.RegisterEvent<CharacterAction, bool>(gameObject, EventIDs.OnCharacterActionActive, OnActionActive);
//            EventHandler.RegisterEvent<ItemAction, bool>(gameObject, EventIDs.OnItemActionActive, OnItemActionActive);
//        }


//        private void OnDestroy()
//        {
//            EventHandler.UnregisterEvent<Vector3, Quaternion>(gameObject, OnSetMatchTargetEventID, SetMatchTarget);
//            EventHandler.UnregisterEvent<ItemAction, bool>(gameObject, EventIDs.OnItemActionActive, OnItemActionActive);
//            EventHandler.UnregisterEvent<CharacterAction, bool>(gameObject, EventIDs.OnCharacterActionActive, OnActionActive);

//        }


//        public void InitializeStateBehaviors()
//        {
//            //  Gather all state machine behaviors.
//            StateBehavior[] stateBehaviors = animator.GetBehaviours<StateBehavior>();
//            if (stateBehaviors.Length > 0)
//            {
//                for (int i = 0; i < stateBehaviors.Length; i++)
//                {
//                    stateBehaviors[i].Initialize(this);
//                }
//            }
//            else
//            {
//                Debug.Log("Animator has no state behaviors.");
//            }
//        }


//        private void Start()
//        {
//            allCharacterActions = new Dictionary<CharacterAction, int>();
//            activeActions = new CharacterAction[controller.CharActions.Length];

//            activeItemActions = new HashSet<ItemAction>();

//            allStateNameHash = new Dictionary<int, string>();
//            allAnimatorStateHash = new Dictionary<int, AnimatorState>();
//            previousTransitions = new AnimatorTransitionInfo[animator.layerCount];
//            previousStates = new AnimatorStateInfo[animator.layerCount];
//            animatorStateData = new AnimatorStateData[animator.layerCount];



//            //
//            //  Map out the character actions 
//            //
//            for (int i = 0; i < controller.CharActions.Length; i++)
//            {
//                var charAction = controller.CharActions[i];
//                allCharacterActions.Add(charAction, i);
//            }

//            //
//            //  Setup all the animatior state data.
//            //
//            AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
//            for (int i = 0; i < animatorController.layers.Length; i++)
//            {
//                AnimatorStateMachine stateMachine = animatorController.layers[i].stateMachine;
//                AnimatorState defaultState = stateMachine.defaultState;
//                if (defaultState == null)
//                {
//                    defaultState = stateMachine.AddState("Empty", Vector3.zero);
//                }
//                animatorStateData[i] = new AnimatorStateData(defaultState.nameHash, defaultState.name, 0.2f);

//            }

//            //
//            //  Register all animator states.
//            //
//            foreach (AnimatorControllerLayer layer in animatorController.layers)
//            {
//                RegisterAnimatorStates(layer.stateMachine, layer.name);
//            }



//        }





//        private void RegisterAnimatorStates(AnimatorStateMachine stateMachine, string parentState)
//        {
//            foreach (ChildAnimatorState childState in stateMachine.states) //for each state
//            {
//                string stateName = childState.state.name;
//                string fullPathName = parentState + "." + stateName;
//                int shortNameHash = Animator.StringToHash(stateName);
//                int fullPathHash = Animator.StringToHash(fullPathName);

//                if (allStateNameHash.ContainsKey(shortNameHash) == false)
//                {
//                    allStateNameHash.Add(shortNameHash, stateName);
//                }
//                if (allStateNameHash.ContainsKey(fullPathHash) == false)
//                {
//                    allStateNameHash.Add(fullPathHash, fullPathName);
//                }

//                if (allStateNameHash.ContainsKey(fullPathHash) == false)
//                {
//                    allAnimatorStateHash.Add(fullPathHash, childState.state);
//                }


//                stateCount++;
//            }

//            foreach (ChildAnimatorStateMachine sm in stateMachine.stateMachines) //for each state
//            {
//                string path = parentState + "." + sm.stateMachine.name;
//                RegisterAnimatorStates(sm.stateMachine, path);
//            }
//        }



//        private void DetermineStates()
//        {
//            for (int i = 0; i < animator.layerCount; i++)
//            {
//                // pull our current transition and state into temporary variables, since we may not need to do anything with them
//                AnimatorTransitionInfo currentTransition = animator.GetAnimatorTransitionInfo(i);
//                AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(i);

//                // if we have a new transition...
//                int previousTransition = previousTransitions[i].fullPathHash;
//                if (currentTransition.fullPathHash != previousTransition)
//                {
//                    // fire off our end callback, if any, for our previous transition...


//                    // fire off our begin call back for our new transition...

//                    //Debug.LogFormat("Transitioning. normalized time: {0}", currentTransition.normalizedTime);
//                    // and remember that we are now in this transition.
//                    previousTransitions[i] = currentTransition;
//                }

//                // if we have a new state, things go similarly.
//                int previousState = previousStates[i].fullPathHash;
//                if (currentState.fullPathHash != previousState)
//                {
//                    //  Current state is ending.

//                    //  Next state is starting.

//                    if (debugStateChanges) Debug.LogFormat("Transitioning from <color=blue>{0}</color> to <color=blue>{1}</color>.", GetStateName(previousState), GetStateName(currentState.fullPathHash));
//                    // recall what state we were in
//                    previousStates[i] = currentState;
//                }
//            }
//        }


//        private void LateUpdate()
//        {
//            DetermineStates();


//        }



//        public string GetStateName(int hash)
//        {
//            if (allStateNameHash.ContainsKey(hash))
//            {
//                return allStateNameHash[hash];
//            }
//            return null;
//        }


//        private void SetMatchTarget(Vector3 position, Quaternion rotation)
//        {
//            hasMatchTarget = true;
//            matchPosition = position;
//            matchRotation = rotation;
//            applyRootMotion = animator.applyRootMotion;
//        }


//        public void ResetMatchTarget()
//        {
//            hasMatchTarget = false;
//            animator.applyRootMotion = applyRootMotion;
//            matchPosition = Vector3.zero;
//            matchRotation = Quaternion.identity;
//        }


//        public bool MatchTarget(Vector3 position, Quaternion rotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startTime, float endTime, bool force = false)
//        {
//            SetMatchTarget(position, rotation);
//            return MatchTarget(matchPosition, matchRotation, targetBodyPart, weightMask, startTime, endTime, force);
//        }


//        public bool MatchTarget(AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startTime, float endTime, bool force = false)
//        {
//            if (hasMatchTarget == false) return false;

//            if (!animator.isMatchingTarget || force)
//            {
//                animator.MatchTarget(matchPosition, matchRotation, targetBodyPart, weightMask, startTime, endTime);
//                return true;
//            }
//            return false;
//        }


//        public void RegisterCallback(int layer, CharacterAction charAction)
//        {

//        }

//        public void UnregisterCallback()
//        {

//        }


//        #region Animation Methods

//        public virtual bool DetermineState(int layer, AnimatorStateData defaultState, bool checkAbilities, bool baseStart)
//        {
//            if (animator.IsInTransition(layer))
//            {
//                if (animator.GetNextAnimatorStateInfo(layer).fullPathHash == defaultState.NameHash)
//                {

//                }

//                Debug.LogFormat("{1} is exiting. | {0} is the next state.", GetStateName(animator.GetNextAnimatorStateInfo(layer).fullPathHash), this.GetType());
//                return true;
//            }


//            throw new NotImplementedException("<color=yellow> AnimatorMonitor </color> FormatStateName() not implemented yet.");

//        }


//        public string FormatStateName(string stateName)
//        {
//            throw new NotImplementedException("<color=yellow> AnimatorMonitor </color> FormatStateName() not implemented yet.");
//        }

//        public void SetHorizontalInputValue(float value)
//        {
//            animator.SetFloat(HashID.HorizontalInput, value, horizontalInputDampTime, deltaTime);
//        }

//        public void SetHorizontalInputValue(float value, float dampTime)
//        {
//            animator.SetFloat(HashID.HorizontalInput, value, dampTime, deltaTime);
//        }

//        public void SetForwardInputValue(float value)
//        {
//            animator.SetFloat(HashID.ForwardInput, value, forwardInputDampTime, deltaTime);
//        }

//        public void SetForwardInputValue(float value, float dampTime)
//        {
//            animator.SetFloat(HashID.ForwardInput, value, dampTime, deltaTime);
//        }

//        public void SetMovementSetID(int value)
//        {
//            animator.SetInteger(HashID.MovementSetID, value);
//        }

//        public void SetActionID(int value)
//        {
//            animator.SetInteger(HashID.ActionID, value);
//        }

//        public void SetActionIntData(int value)
//        {
//            animator.SetInteger(HashID.ActionIntData, value);
//        }

//        public void SetActionFloatData(float value)
//        {
//            animator.SetFloat(HashID.ActionFloatData, value);
//        }

//        public void SetItemID(int value)
//        {
//            animator.SetInteger(HashID.ItemID, value);
//        }

//        public void SetItemStateIndex(int value)
//        {
//            animator.SetInteger(HashID.ItemStateIndex, value);
//        }

//        public void SetItemSubstateIndex(int value)
//        {
//            animator.SetInteger(HashID.ItemSubstateIndex, value);
//        }

//        public void SetItemStateChange()
//        {
//            animator.SetTrigger(HashID.ItemStateIndexChange);
//        }

//        public void ResetActionTrigger() { animator.ResetTrigger(HashID.ActionChange); }
//        public void ResetItemStateTrigger() { animator.ResetTrigger(HashID.ItemStateIndexChange); }


//        public void ExecuteEvent(string eventName)
//        {
//            if (logEvents) Debug.Log(eventName);
//            EventHandler.ExecuteEvent(gameObject, eventName);
//        }

//        public void ItemUsed(int itemTypeIndex)
//        {

//        }

//        #endregion



//        private void OnActionActive(CharacterAction action, bool activated)
//        {
//            activeAction = activated;
//            if (activated) activeActionsCount++;
//            else activeActionsCount--;
//            if (activeActionsCount > 1) Debug.LogFormat("Active Action count: {0}", activeActionsCount);

//            if (allCharacterActions.TryGetValue(action, out int index))
//            {
//                activeActions[index] = activated ? action : null;
//            }
//            else
//            {
//                Debug.LogError("Action <color=blue>" + action.name + "</color> is not registered with AnimatorMonitor.");
//            }
//        }

//        private void OnItemActionActive(ItemAction action, bool activated)
//        {
//            activeAction = activated;
//            if (activated) activeActionsCount++;
//            else activeActionsCount--;
//            if (activeActionsCount > 1) Debug.LogFormat("Active Action count: {0}", activeActionsCount);



//            if (!activeItemActions.Contains(action))
//            {
//                activeItemActions.Add(action);
//            }
//            else
//            {
//                Debug.LogError("ItemAction <color=cyan>" + action.name + "</color> is not registered with AnimatorMonitor.");
//            }
//        }

//        #region Debug

//        /// <summary>
//        /// Register all animator state ids and print the names.
//        /// </summary>
//        /// <param name="debugMsg"></param>
//        public void RegisterAllAnimatorStateIDs(bool debugMsg = false)
//        {
//            if (animator == null) animator = GetComponent<Animator>();
//            AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;


//            foreach (AnimatorControllerLayer layer in animatorController.layers)
//            {
//                RegisterAnimatorStates(layer.stateMachine, layer.name);
//            }

//            if (debugMsg) DebugLogAnimatorStates();

//        }

//        private void DebugLogAnimatorStates()
//        {
//            //allStateNameHash.Keys.OrderBy(k => k).ToDictionary(k =>k, k => allStateNameHash[k]);

//            //allStateNameHash.OrderByDescending(r => r.Value).ThenBy(r => r.Key);
//            allStateNameHash.OrderByDescending(r => r.Value);

//            var sortedList = allStateNameHash.ToList();
//            sortedList.Sort((x, y) => string.Compare(x.Value, y.Value, StringComparison.CurrentCulture));

//            string debugStateInfo = "";
//            debugStateInfo += "<b>State Name Hash: </b>\n";

//            for (int i = 0; i < sortedList.Count; i++)
//            {
//                debugStateInfo += "<b>StateName:</b> " + sortedList[i].Value + " | <b>HashID:</b> " + sortedList[i].Key + "\n";
//            }


//            debugStateInfo += "\n<b>Total State Count: " + stateCount + " </b>\n";

//            stateCount = 0;
//            Debug.Log(debugStateInfo);
//        }






//        GUIStyle guiStyle;
//        GUIContent guiContent;
//        private void OnGUI()
//        {
//            if (!Application.isPlaying) return;
//            if (debugStateChanges == false) return;

//            if (guiStyle == null)
//            {
//                guiStyle = CharacterControllerUtility.GuiStyle;
//                guiStyle.fontSize = 28;
//            }

//            if (guiContent == null)
//            {
//                guiContent = new GUIContent();
//            }

//            int layerIndex = 0;
//            int line = 0;

//            GUI.color = CharacterControllerUtility.DebugTextColor;
//            Rect rect = CharacterControllerUtility.AnimatorMonitorRect;

//            GUI.BeginGroup(rect, GUI.skin.box);
//            //GUI.BeginGroup(rect);
//            GUI.Label(rect, DebugGetCurrentStateInfo(layerIndex), guiStyle);

//            if (animator.IsInTransition(layerIndex))
//            {
//                var transitionStateInfo = animator.GetAnimatorTransitionInfo(layerIndex);
//                //string nextState = allStateNameHash[animator.GetAnimatorTransitionInfo(layerIndex).fullPathHash];
//                string nextState = GetStateName(animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash);
//                line++;

//                //string label = " °State Name: " + nextState + "Transition Duration: " + transitionStateInfo.duration.ToString() + " ";
//                guiContent.text = " °State Name: " + nextState + "Transition Duration: " + transitionStateInfo.duration.ToString() + " ";
//                float lineHeight = guiStyle.CalcHeight(guiContent, rect.width);

//                rect.y = rect.y + lineHeight * line;

//                GUI.Label(rect, guiContent, guiStyle);
//            }


//            if (animator.isMatchingTarget)
//            {
//                line++;
//                rect.y = rect.y + UnityEditor.EditorGUIUtility.singleLineHeight * line;
//                GUI.Label(rect, "Currently matching target.", CharacterControllerUtility.GuiStyle);
//            }


//            GUI.EndGroup();
//        }





//        private string DebugGetCurrentStateInfo(int layerIndex, bool addNewLine = false)
//        {
//            string stateInfo = "";

//            var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
//            var nextAnimatorStateInfo = animator.GetNextAnimatorStateInfo(layerIndex);
//            var transitionStateInfo = animator.GetAnimatorTransitionInfo(layerIndex);

//            string layerName = animator.GetLayerName(layerIndex);
//            string fullPathHash = animatorStateInfo.fullPathHash.ToString();
//            string shortPathHash = animatorStateInfo.shortNameHash.ToString();

//            if (allStateNameHash.ContainsKey(animatorStateInfo.fullPathHash))
//            {
//                string stateName = allStateNameHash[animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash];
//                //stateInfo = layerName + " | State Name: " + stateName + "  | fullPathHash <" + fullPathHash + ">, shortPathHash <" + shortPathHash + ">";
//                stateInfo = " °State Name: " + stateName +
//                            "  | Duration: " + animatorStateInfo.length +
//                            ", NormalizeTime: " + animatorStateInfo.normalizedTime +
//                            "    °";

//            }
//            else
//            {
//                stateInfo = layerName + " | <b>No HASHID</b>  | fullPathHash <" + fullPathHash + ">, shortPathHash <" + shortPathHash + ">";
//            }


//            if (addNewLine)
//                stateInfo += "\n";
//            return stateInfo;
//        }





//        #endregion





//    }
//}




//// if (m_ActiveStateHash != null) {
////     for (int i = 0; i < m_ActiveStateHash.Length; ++i) {
////         m_ActiveStateHash[i] = 0;
////     }

////     PlayDefaultStates(); /// HERE
//// }
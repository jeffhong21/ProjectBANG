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

        private CharacterAction[] m_actions;
        [SerializeField, DisplayOnly]
        private CharacterAction m_activeAction, m_previousAction, m_nextAction;
        private Dictionary<string, int> m_stateNameHash;
        private Dictionary<int, string> m_stateHashName;

        //  Animator layer names.
        private string[] m_animatorLayerNames;

        //  -- Variables for determining which state.
        // store all transitions and states so we know when they have changed
        private AnimatorTransitionInfo[] m_previousTransitions;
        //  store the previous states
        private AnimatorStateInfo[] m_previousStates;
        // store all the current states.
        private AnimatorStateData[] m_animatorStateData;



        private StateBehavior[] m_stateBehaviors;
        private StateBehavior m_activeStateBehavior;
        //
        // Fields
        //

        //[Foldout("Damp Options", true)]
        [SerializeField] private float horizontalInputDampTime = 0.1f;
        [SerializeField] private float forwardInputDampTime = 0.1f;


        [Header("Debug Options")]
        [SerializeField] private bool debugStateChanges;
        [SerializeField] private bool logEvents;



        private Animator m_animator;
        private CharacterLocomotion m_controller;
        private Inventory m_inventory;

        private int m_stateCount;
        private float m_deltaTime;



        //
        // Properties
        //





        //
        // Methods
        //
        private void Awake()
		{
            m_stateNameHash = new Dictionary<string, int>();
            m_stateHashName = new Dictionary<int, string>();

            m_animator = GetComponent<Animator>();
            m_controller = GetComponent<CharacterLocomotion>();
            m_inventory = GetComponent<Inventory>();
            //  Set m_deltaTime.
            m_deltaTime = m_animator.updateMode == AnimatorUpdateMode.AnimatePhysics ? Time.fixedDeltaTime : Time.deltaTime;

            //  Initialize statebehaviors.
            InitializeStateBehaviors();




            EventHandler.RegisterEvent<CharacterAction, bool>(gameObject, EventIDs.OnCharacterActionActive, OnActionActive);
            EventHandler.RegisterEvent<ItemAction, bool>(gameObject, EventIDs.OnItemActionActive, OnItemActionActive);
        }


        private void OnDestroy()
        {
            EventHandler.UnregisterEvent<ItemAction, bool>(gameObject, EventIDs.OnItemActionActive, OnItemActionActive);
            EventHandler.UnregisterEvent<CharacterAction, bool>(gameObject, EventIDs.OnCharacterActionActive, OnActionActive);
        }


        /// <summary>
        /// Initialize state machine behaviors.
        /// </summary>
        public void InitializeStateBehaviors()
        {
            //  Gather all state machine behaviors.
            m_stateBehaviors = m_animator.GetBehaviours<StateBehavior>();
            if (m_stateBehaviors.Length > 0) {
                for (int i = 0; i < m_stateBehaviors.Length; i++) {
                    m_stateBehaviors[i].Initialize(this, m_animator);
                }
            }
            m_activeStateBehavior = null;
        }


        private void Start()
        {
            int layerCount = m_animator.layerCount;
            //  Initialize animator layer names.
            m_animatorLayerNames = new string[layerCount];
            for (int i = 0; i < layerCount; i++)
                m_animatorLayerNames[i] = m_animator.GetLayerName(i);

            m_previousTransitions = new AnimatorTransitionInfo[layerCount];
            m_previousStates = new AnimatorStateInfo[layerCount];
            m_animatorStateData = new AnimatorStateData[layerCount];


            //  Set up actions list.
            int count = m_controller.CharActions.Length;
            m_actions = new CharacterAction[count];
            for (int i = 0; i < count; i++)
                m_actions[i] = m_controller.CharActions[i];
            


            //
            //  Setup all the animatior state data.
            //
            AnimatorController animatorController = m_animator.runtimeAnimatorController as AnimatorController;
            for (int i = 0; i < animatorController.layers.Length; i++)
            {
                AnimatorStateMachine stateMachine = animatorController.layers[i].stateMachine;
                AnimatorState defaultState = stateMachine.defaultState;
                if (defaultState == null){
                    defaultState = stateMachine.AddState("Empty", Vector3.zero);
                }
                m_animatorStateData[i] = new AnimatorStateData(defaultState.nameHash, defaultState.name, 0.2f);
            }


            //
            //  Register all m_animator states.
            //
            foreach (AnimatorControllerLayer layer in animatorController.layers) {
                RegisterAnimatorStates(layer.stateMachine, layer.name);
            }



        }



        private void LateUpdate()
        {
            DetermineStates();

            DebugUI.DebugUI.Log(this, m_activeAction == null ? "null" : m_activeAction.ToString(), "ActiveAction", DebugUI.RichTextColor.Yellow);
            DebugUI.DebugUI.Log(this, m_animator.isMatchingTarget, "MatchingTarget", DebugUI.RichTextColor.Yellow);
        }





        /// <summary>
        /// Register all the state machines
        /// </summary>
        /// <param name="stateMachine"></param>
        /// <param name="parentState"></param>
        private void RegisterAnimatorStates(AnimatorStateMachine stateMachine, string parentState)
        {
            foreach (ChildAnimatorState childState in stateMachine.states) //for each state
            {
                string stateName = childState.state.name;
                string fullPathName = parentState + "." + stateName;
                int shortNameHash = Animator.StringToHash(stateName);
                int fullPathHash = Animator.StringToHash(fullPathName);


                //  Fullpath hash
                if (m_stateNameHash.ContainsKey(fullPathName))m_stateNameHash[fullPathName] = fullPathHash;
                else m_stateNameHash.Add(fullPathName, fullPathHash);
                //  Shortpath hash
                if (m_stateNameHash.ContainsKey(stateName)) m_stateNameHash[stateName] = shortNameHash;
                else m_stateNameHash.Add(stateName, shortNameHash);


                //  Fullpath name
                if (m_stateHashName.ContainsKey(fullPathHash)) m_stateHashName[fullPathHash] = fullPathName;
                else m_stateHashName.Add(fullPathHash, fullPathName);
                //  Shortpath name
                if (m_stateHashName.ContainsKey(shortNameHash)) m_stateHashName[shortNameHash] = stateName;
                else m_stateHashName.Add(shortNameHash, stateName);




                m_stateCount++;
            }

            foreach (ChildAnimatorStateMachine sm in stateMachine.stateMachines) //for each state
            {
                string path = parentState + "." + sm.stateMachine.name;
                RegisterAnimatorStates(sm.stateMachine, path);
            }
        }



        /// <summary>
        /// Determine the current action states.
        /// </summary>
        private void DetermineStates()
        {
            if(m_activeAction)
            {
                for (int layer = 0; layer < m_animator.layerCount; layer++)
                {
                    bool stateChanged = DetermineState(layer, m_animatorStateData[layer], true);
                    if (stateChanged)
                    {

                    }
                }
            }

        }



        public int GetStateHash(string state)
        {
            if (m_stateNameHash.TryGetValue(state, out int hash) == false)
                hash = -1;
            return hash;

        }


        public string GetStateName(int hash)
        {
            if (m_stateHashName.ContainsKey(hash)){
                return m_stateHashName[hash];
            }
            return null;
        }


        public bool ChangeAnimatorState(CharacterAction action, int layer, string destinationState, float transitionDuration, float normalizedTime = 0f)
        {
            if (destinationState.Contains(".") == false) Debug.Log(this.GetType());

            //  Set active action.
            m_previousAction = m_activeAction;
            m_activeAction = action;

            string fullStateName = m_animator.GetLayerName(layer) + "." + destinationState;
            int stateHash = GetStateHash(fullStateName);
            if (stateHash > -1){
                if (m_animator.HasState(layer, stateHash)){
                    m_animator.CrossFade(fullStateName, (transitionDuration > 0) ? transitionDuration : 0.01f, layer, normalizedTime);
                    //m_animator.CrossFadeInFixedTime(fullStateName, (transitionDuration > 0) ? transitionDuration : 0.01f, layer, normalizedTime);
                    Debug.LogFormat("ActiveAction is ({0})", action.GetType().Name);
                    return true;
                }
            }
            Debug.LogFormat("<b><color=blue>[AnimatorMonitor]</color></b> Full State Name: {0} | StateHAsh: {1} | AnimatorStateHash: {2}", fullStateName, stateHash, Animator.StringToHash(fullStateName));


            return false;
        }



        /// <summary>
        /// Determine the state that the specified layer should be in.
        /// </summary>
        /// <param name="layer">The layer to determine the state of.</param>
        /// <param name="defaultState">The default state to be in if no other states should run.</param>
        /// <param name="checkAction">Should the action be checked to determine if they have control?</param>
        /// <param name="baseStart">Is the base layer being set at the same time?</param>
        /// <returns>True if the state was changed.</returns>
        public virtual bool DetermineState(int layer, AnimatorStateData defaultState, bool checkAction, bool baseStart = false)
        {
            bool stateChanged = false;
            // pull our current transition and state into temporary variables, since we may not need to do anything with them
            AnimatorTransitionInfo currentTransition = m_animator.GetAnimatorTransitionInfo(layer);
            AnimatorStateInfo currentState = m_animator.GetCurrentAnimatorStateInfo(layer);

            // if we have a new transition...
            int previousTransition = m_previousTransitions[layer].fullPathHash;
            if (currentTransition.fullPathHash != previousTransition)
            {
                //Debug.LogFormat("Transitioning. normalized time: {0}", currentTransition.normalizedTime);

                // fire off our end callback, if any, for our previous transition...
                //  Next state is starting.


                // fire off our begin call back for our new transition...


                // and remember that we are now in this transition.
                m_previousTransitions[layer] = currentTransition;

            }

            // if we have a new state, things go similarly.
            int previousState = m_previousStates[layer].fullPathHash;
            if (currentState.fullPathHash != previousState)
            {
                if(layer == 0) if (debugStateChanges) Debug.LogFormat("Transitioning from <color=blue>{0}</color> to <color=blue>{1}</color>.", GetStateName(previousState), GetStateName(currentState.fullPathHash));

                //  Current state is ending.

                //  Next state is starting.
                //if(m_activeAction != null)
                //{
                //    //Debug.Break();
                //    m_controller.TryStopAction(m_activeAction, true);
                //    m_activeAction = null;
                //}


                // recall what state we were in
                m_previousStates[layer] = currentState;
                // State has changed.
                stateChanged = true;
            }


            return stateChanged;
        }


        public void SetActiveStateBehavior(StateBehavior stateBehavior)
        {
            m_activeStateBehavior = stateBehavior;
        }


        public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation)
        {
            if (!m_animator.isMatchingTarget && m_activeStateBehavior != null){
                if (m_activeStateBehavior is ActionStateBehavior){
                    var actionStateBehavior = (ActionStateBehavior)m_activeStateBehavior;
                    actionStateBehavior.MatchTarget(matchPosition, matchRotation);

                }

            }

        }



        #region Animation Methods


        



        public void SetHorizontalInputValue(float value){
            m_animator.SetFloat(HashID.HorizontalInput, value, horizontalInputDampTime,  m_deltaTime);
        }

        public void SetHorizontalInputValue( float value, float dampTime){
            m_animator.SetFloat(HashID.HorizontalInput, value, dampTime, m_deltaTime);
        }

        public void SetForwardInputValue(float value){
            m_animator.SetFloat(HashID.ForwardInput, value, forwardInputDampTime, m_deltaTime);
        }

        public void SetForwardInputValue( float value, float dampTime ){
            m_animator.SetFloat(HashID.ForwardInput, value, dampTime, m_deltaTime);
        }

        public void SetMovementSetID( int value ){
            m_animator.SetInteger(HashID.MovementSetID, value);
        }

        public void SetActionID(int value){
            m_animator.SetInteger(HashID.ActionID, value);
        }

        public void SetActionIntData(int value){
            m_animator.SetInteger(HashID.ActionIntData, value);
        }

        public void SetActionFloatData(float value){
            m_animator.SetFloat(HashID.ActionFloatData, value);
        }

        public void SetItemID( int value ){
            m_animator.SetInteger(HashID.ItemID, value);
        }

        public void SetItemStateIndex(int value ){
            m_animator.SetInteger(HashID.ItemStateIndex, value);
        }

        public void SetItemSubstateIndex( int value ){
            m_animator.SetInteger(HashID.ItemSubstateIndex, value);
        }

        public void SetItemStateChange(){
            m_animator.SetTrigger(HashID.ItemStateIndexChange);
        }

        public void ActionChanged() { m_animator.SetTrigger(HashID.ActionChange); }

        public void ItemStateChanged() { m_animator.SetTrigger(HashID.ItemStateIndexChange); }

        public void ResetActionTrigger() { m_animator.ResetTrigger(HashID.ActionChange); }

        public void ResetItemStateTrigger() { m_animator.ResetTrigger(HashID.ItemStateIndexChange); }

        public void ExecuteEvent(string eventName)
        {
            if(logEvents) Debug.Log(eventName);
            EventHandler.ExecuteEvent(gameObject, eventName);
        }

        public void ItemUsed(int itemTypeIndex)
        {

        }

        #endregion



        private void OnActionActive( CharacterAction action, bool activated )
        {
            if (activated)
            {
                //  There is currently an action running.
                if (m_activeAction != action && m_activeAction != null)
                {
                    //Debug.LogFormat("Stoppiong ActiveAction {0} |Starting Action ({1})", m_activeAction.GetType().Name,  action.GetType().Name);
                    m_controller.TryStopAction(m_activeAction, true);
                    m_activeAction = action;
                }
                //  If active action is the same as the incoming action.  This shoul;dn't happen.
                else if (m_activeAction == action)
                {
                    Debug.LogFormat("<color=yellow><b>[Warning]</color></b> ActiveAction is the same as incoming Action ({0})", action.GetType().Name);
                }
                //  there is no active action.
                else
                {
                    //Debug.LogFormat("ActiveAction is ({0})", action.GetType().Name);
                    m_activeAction = action;
                }

                //  If animator is matching target, stop it.
                if (m_animator.isMatchingTarget)
                    m_animator.InterruptMatchTarget(false);
            }
            else
            {
                if (m_activeAction == action)
                {
                    //Debug.LogFormat("ActiveAction {0} is now null.", m_activeAction.GetType().Name);
                    m_activeAction = null;

                    //  If animator is matching target, stop it.
                    if (m_animator.isMatchingTarget)
                        m_animator.InterruptMatchTarget(true);
                }
            }
        }

        private void OnItemActionActive( ItemAction action, bool activated )
        {


        }






        #region Debug

        /// <summary>
        /// Register all m_animator state ids and print the names.
        /// </summary>
        /// <param name="debugMsg"></param>
        public void RegisterAllAnimatorStateIDs( bool debugMsg = false )
        {
            if (m_animator == null) m_animator = GetComponent<Animator>();
            AnimatorController animatorController = m_animator.runtimeAnimatorController as AnimatorController;


            foreach (AnimatorControllerLayer layer in animatorController.layers) {
                RegisterAnimatorStates(layer.stateMachine, layer.name);
            }

            if (debugMsg) DebugLogAnimatorStates();

        }

        private void DebugLogAnimatorStates()
        {
            //m_stateHashName.Keys.OrderBy(k => k).ToDictionary(k =>k, k => m_stateHashName[k]);

            //m_stateHashName.OrderByDescending(r => r.Value).ThenBy(r => r.Key);
            m_stateHashName.OrderByDescending(r => r.Value);

            var sortedList = m_stateHashName.ToList();
            sortedList.Sort(( x, y ) => string.Compare(x.Value, y.Value, StringComparison.CurrentCulture));

            string debugStateInfo = "";
            debugStateInfo += "<b>State Name Hash: </b>\n";

            for (int i = 0; i < sortedList.Count; i++) {
                debugStateInfo += "<b>StateName:</b> " + sortedList[i].Value + " | <b>HashID:</b> " + sortedList[i].Key + "\n";
            }


            debugStateInfo += "\n<b>Total State Count: " + m_stateCount + " </b>\n";

            m_stateCount = 0;
            Debug.Log(debugStateInfo);
        }





        







        #endregion





    }
}




// if (m_ActiveStateHash != null) {
//     for (int i = 0; i < m_ActiveStateHash.Length; ++i) {
//         m_ActiveStateHash[i] = 0;
//     }
 
//     PlayDefaultStates(); /// HERE
// }
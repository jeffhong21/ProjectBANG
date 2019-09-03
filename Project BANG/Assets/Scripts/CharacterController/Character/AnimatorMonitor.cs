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
        private CharacterAction m_activeAction;
        private Dictionary<int, string> m_stateNameHash = new Dictionary<int, string>();



        //  -- Variables for determining which state.
        // store all transitions and states so we know when they have changed
        private AnimatorTransitionInfo[] previousTransitions;
        //  store the previous states
        private AnimatorStateInfo[] previousStates;
        // store all the current states.
        private AnimatorStateData[] animatorStateData;



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






        //
        // Methods
        //
        private void Awake()
		{
            m_animator = GetComponent<Animator>();
            m_controller = GetComponent<CharacterLocomotion>();
            //  Set deltaTime.
            deltaTime = m_animator.updateMode == AnimatorUpdateMode.AnimatePhysics ? Time.fixedDeltaTime : Time.deltaTime;

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
            StateBehavior[] stateBehaviors = m_animator.GetBehaviours<StateBehavior>();
            if (stateBehaviors.Length > 0) {
                for (int i = 0; i < stateBehaviors.Length; i++) {
                    stateBehaviors[i].Initialize(this, m_animator);
                }
            }
        }


        private void Start()
        {
            previousTransitions = new AnimatorTransitionInfo[m_animator.layerCount];
            previousStates = new AnimatorStateInfo[m_animator.layerCount];
            animatorStateData = new AnimatorStateData[m_animator.layerCount];


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
                animatorStateData[i] = new AnimatorStateData(defaultState.nameHash, defaultState.name, 0.2f);
            }


            //
            //  Register all m_animator states.
            //
            foreach (AnimatorControllerLayer layer in animatorController.layers) {
                RegisterAnimatorStates(layer.stateMachine, layer.name);
            }



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

                if (m_stateNameHash.ContainsKey(shortNameHash) == false)
                {
                    m_stateNameHash.Add(shortNameHash, stateName);
                }
                if (m_stateNameHash.ContainsKey(fullPathHash) == false)
                {
                    m_stateNameHash.Add(fullPathHash, fullPathName);
                }


                stateCount++;
            }

            foreach (ChildAnimatorStateMachine sm in stateMachine.stateMachines) //for each state
            {
                string path = parentState + "." + sm.stateMachine.name;
                RegisterAnimatorStates(sm.stateMachine, path);
            }
        }



        private void DetermineStates()
        {
            for (int i = 0; i < m_animator.layerCount; i++)
            {
                // pull our current transition and state into temporary variables, since we may not need to do anything with them
                AnimatorTransitionInfo currentTransition = m_animator.GetAnimatorTransitionInfo(i);
                AnimatorStateInfo currentState = m_animator.GetCurrentAnimatorStateInfo(i);

                // if we have a new transition...
                int previousTransition = previousTransitions[i].fullPathHash;
                if (currentTransition.fullPathHash != previousTransition)
                {
                    // fire off our end callback, if any, for our previous transition...


                    // fire off our begin call back for our new transition...

                    //Debug.LogFormat("Transitioning. normalized time: {0}", currentTransition.normalizedTime);
                    // and remember that we are now in this transition.
                    previousTransitions[i] = currentTransition;
                }

                // if we have a new state, things go similarly.
                int previousState = previousStates[i].fullPathHash;
                if (currentState.fullPathHash != previousState)
                {
                    //  Current state is ending.

                    //  Next state is starting.

                    if (debugStateChanges) Debug.LogFormat("Transitioning from <color=blue>{0}</color> to <color=blue>{1}</color>.", GetStateName(previousState), GetStateName(currentState.fullPathHash));
                    // recall what state we were in
                    previousStates[i] = currentState;
                }
            }
        }


        private void LateUpdate()
		{
            DetermineStates();


        }



        public string GetStateName(int hash)
        {
            if (m_stateNameHash.ContainsKey(hash)){
                return m_stateNameHash[hash];
            }
            return null;
        }


        public virtual bool DetermineState(int layer, AnimatorStateData defaultState, bool checkAbilities, bool baseStart)
        {
            if (m_animator.IsInTransition(layer))
            {
                if (m_animator.GetNextAnimatorStateInfo(layer).fullPathHash == defaultState.NameHash)
                {

                }

                Debug.LogFormat("{1} is exiting. | {0} is the next state.", GetStateName(m_animator.GetNextAnimatorStateInfo(layer).fullPathHash), this.GetType());
                return true;
            }


            throw new NotImplementedException("<color=yellow> AnimatorMonitor </color> FormatStateName() not implemented yet.");

        }






        #region Animation Methods



        public string FormatStateName(string stateName)
        {
            throw new NotImplementedException("<color=yellow> AnimatorMonitor </color> FormatStateName() not implemented yet.");
        }


        public void SetHorizontalInputValue(float value){
            m_animator.SetFloat(HashID.HorizontalInput, value, horizontalInputDampTime,  deltaTime);
        }

        public void SetHorizontalInputValue( float value, float dampTime){
            m_animator.SetFloat(HashID.HorizontalInput, value, dampTime, deltaTime);
        }

        public void SetForwardInputValue(float value){
            m_animator.SetFloat(HashID.ForwardInput, value, forwardInputDampTime, deltaTime);
        }

        public void SetForwardInputValue( float value, float dampTime ){
            m_animator.SetFloat(HashID.ForwardInput, value, dampTime, deltaTime);
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
            if(activated)
            {


                if(m_activeAction != action && m_activeAction != null)
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
                
            }
            else
            {
                if(m_activeAction == action)
                {
                    //Debug.LogFormat("ActiveAction {0} is now null.", m_activeAction.GetType().Name);
                    m_activeAction = null;
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
            //m_stateNameHash.Keys.OrderBy(k => k).ToDictionary(k =>k, k => m_stateNameHash[k]);

            //m_stateNameHash.OrderByDescending(r => r.Value).ThenBy(r => r.Key);
            m_stateNameHash.OrderByDescending(r => r.Value);

            var sortedList = m_stateNameHash.ToList();
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





        







        #endregion





    }
}




// if (m_ActiveStateHash != null) {
//     for (int i = 0; i < m_ActiveStateHash.Length; ++i) {
//         m_ActiveStateHash[i] = 0;
//     }
 
//     PlayDefaultStates(); /// HERE
// }
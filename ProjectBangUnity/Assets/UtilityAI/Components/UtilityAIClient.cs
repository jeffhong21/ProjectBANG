﻿namespace UtilityAI
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Visualization;

    public enum UtilityAIClientState
    {
        Running,  ///   The associated AI is running.
        Stopped,  ///   The associated AI is not running.
        Pause     ///   The associated AI is paused.
    }

    /// <summary>
    /// This is the decision maker.
    /// </summary>
    [Serializable]
    public class UtilityAIClient : IUtilityAIClient
    {
        //  ContextProvider from BANG.
        [SerializeField, HideInInspector]
        private Bang.AIContextProvider contextProvider;

        [SerializeField]
        private IUtilityAI _ai;
        [SerializeField]
        private UtilityAIClientState _state;

        [SerializeField]
        private float _intervalMin;
        [SerializeField]
        private float _intervalMax;
        [SerializeField]
        private float _startDelayMin;
        [SerializeField]
        private float _startDelayMax;



        public IUtilityAI ai{ 
            get{return _ai;}
            set{ _ai = value;}
        }

        public UtilityAIClientState state{ 
            get{return _state; }
            protected set{_state = value;}
        }

        public float intervalMin { 
            get{
                //if (_intervalMin > _intervalMax){
                //    _intervalMin = _intervalMax;
                //}
                return _intervalMin;
            }
            set { _intervalMin = value; }
        }

        public float intervalMax{
            get{
                //if (_intervalMax < _intervalMin){
                //    _intervalMax = _intervalMin;
                //}
                return _intervalMax;
            }
            set { _intervalMax = value; }
        }

        public float startDelayMin{
            get{
                //if (_startDelayMin > _startDelayMax){
                //    _startDelayMin = _startDelayMax;
                //}
                return _startDelayMax;
            }
            set{_startDelayMin = value;}
        }

        public float startDelayMax{
            get { 
                //if(_startDelayMax < _startDelayMin){
                //    _startDelayMax = _startDelayMin;
                //}
                return _startDelayMax;
            }
            set { _startDelayMax = value; }
        }


        public IAction activeAction{
            get; 
            protected set;
        }



        //  For Debuging.
        public bool _debugClient;
        public bool debugClient
        {
            get { return _debugClient; }
            set { _debugClient = value; }
        }
        public Dictionary<IQualifier, float> selectorResults;



        #region Constructors

        public UtilityAIClient(Guid aiId, IContextProvider contextProvider) 
        {
            this.contextProvider = contextProvider as Bang.AIContextProvider;

            string path = AssetDatabase.GUIDToAssetPath(aiId.ToString("N"));
            AIStorage aiStorage = AssetDatabase.LoadMainAssetAtPath(path) as AIStorage;
            ai = aiStorage.configuration;

            intervalMin = intervalMax = 1f;
            startDelayMin = startDelayMax = 0f;
            state = UtilityAIClientState.Stopped;

            Debug.Log("Constructing UtilityAIClient with aiId - v1");
        }

        public UtilityAIClient(UtilityAI ai, IContextProvider contextProvider)
        {
            this.ai = ai;
            this.contextProvider = contextProvider as Bang.AIContextProvider;

            intervalMin = intervalMax = 1f;
            startDelayMin = startDelayMax = 0f;
            state = UtilityAIClientState.Stopped;

            //Debug.LogFormat("this.IContextProvider:  {0}  |  param IContextProvider:  {1}", this.contextProvider, contextProvider);
        }


        public UtilityAIClient(Guid aiId, IContextProvider contextProvider, float intervalMin, float intervalMax, float startDelayMin, float startDelayMax)
        {
            this.contextProvider = contextProvider as Bang.AIContextProvider;

            string path = AssetDatabase.GUIDToAssetPath(aiId.ToString("N"));
            AIStorage aiStorage = AssetDatabase.LoadMainAssetAtPath(path) as AIStorage;
            ai = aiStorage.configuration;

            this.intervalMin = intervalMin;
            this.intervalMax = intervalMax;
            this.startDelayMin = startDelayMin;
            this.startDelayMax = startDelayMax;
            state = UtilityAIClientState.Stopped;

            //Debug.Log("Constructing UtilityAIClient with aiId - v2");
        }


        public UtilityAIClient(UtilityAI ai, IContextProvider contextProvider, float intervalMin, float intervalMax, float startDelayMin, float startDelayMax)
        {
            this.ai = ai;
            this.contextProvider = contextProvider as Bang.AIContextProvider;

            this.intervalMin = intervalMin;
            this.intervalMax = intervalMax;
            this.startDelayMin = startDelayMin;
            this.startDelayMax = startDelayMax;
            state = UtilityAIClientState.Stopped;

            //Debug.Log("Contructor with all variables");
        }

        #endregion



        //  Part of a IBalanced/Scheduler
        public float? ExecuteUpdate(float deltaTime, float nextInterval)
        {

            //var sw = Stopwatch.StartNew();
            //var ticks = (nextInterval * Stopwatch.Frequency) / 1000;
            //while (sw.ElapsedTicks < ticks) { }


            Execute();

            return null;
        }



        public void Execute()
        {
            if (state != UtilityAIClientState.Running) return;


            //Debug.LogFormat("UtilityAI:  {0}  |  ContextProvider:  {1}", ai, contextProvider);

            IAction newAction = ai.Select(contextProvider.GetContext());
            activeAction = newAction;

            if (activeAction == null)
                return;


            //  For Debug
            //selectorResults = GetSelectorResults(contextProvider.GetContext(), ai.rootSelector.qualifiers, ai.rootSelector.defaultQualifier);
            // --------------
            if(debugClient)
                Debug.Log("Executing " + activeAction.name);

            //  Delay the execution based on the delayMin/max.


            activeAction.Execute(contextProvider.GetContext());


            ////  Call event for GetDataForVisualization(activeAction, contextProvider.GetContext())
            //VisualizerManager.UpdateVisualizer(activeAction, contextProvider.GetContext());
        }



        /// <summary>
        /// Called to initialize AIClient
        /// </summary>
        public void Start(){
            if (state != UtilityAIClientState.Stopped)
                return;

            state = UtilityAIClientState.Running;
            OnStart();
            //Debug.Log(string.Format("Executing action:  {0} | {1}", activeAction.GetType().Name, Time.time));
            AIManager.Register(this);
        }


        public void Stop(){
            if (state == UtilityAIClientState.Stopped)
                return;
            
            state = UtilityAIClientState.Stopped;
            OnStop();
            AIManager.UnRegister(this);
        }


        public void Resume(){
            if (state != UtilityAIClientState.Pause)
                return;
            
            state = UtilityAIClientState.Running;
            OnResume();
        }


        public void Pause(){
            if (state != UtilityAIClientState.Running)
                return;
            
            state = UtilityAIClientState.Pause;
            OnPause();
        }



        protected virtual void OnStart()
        {

        }

        protected virtual void OnStop()
        {

        }

        protected virtual void OnPause()
        {

        }

        protected virtual void OnResume()
        {

        }





        /// <summary>
        /// For Debugging
        /// </summary>
        public Dictionary<IQualifier, float> GetSelectorResults(IAIContext context, IList<IQualifier> qualifiers, IDefaultQualifier defaultQualifer)
        {
            if (selectorResults == null)
                selectorResults = new Dictionary<IQualifier, float>();
            else
                selectorResults.Clear();

            for (int index = 0; index < qualifiers.Count; index++)
            {
                CompositeQualifier qualifier = qualifiers[index] as CompositeQualifier;
                var score = qualifier.Score(context, qualifier.scorers);
                selectorResults.Add(qualifier, score);
            }


            var dq = defaultQualifer as IQualifier;
            selectorResults.Add(dq, defaultQualifer.Score(context));

            return selectorResults;
        }





        //public void Execute2()
        //{
        //    IAction newAction = ai.Select(context);
        //    Debug.Log(string.Format("Current Action:  {0}\nNew Action: {1}", activeAction, newAction));
        //
        //    if (activeAction != newAction){
        //        activeAction = newAction;
        //        //activeAction.utilityAIComponent = taskNetwork;
        //        //if (ActionSelected != null){
        //        //    ActionSelected(context, ai.rootSelector.qualifiers);
        //        //    Debug.Log(ai.name + " just pinged debugger");
        //        //}
        //        selectorResults = GetSelectorResults(context, ai.rootSelector.qualifiers, ai.rootSelector.defaultQualifier);
        //    }
        //    else{
        //        return;
        //    }
        //
        //    if (activeAction == null)
        //        return;
        //   
        //    //  Execute the current action.
        //    activeAction.ExecuteAction(context);
        //    Debug.Log("Executing " + activeAction.GetType().Name);
        //}





        ///// <summary>
        ///// Selects the action.
        ///// </summary>
        ///// <returns><c>true</c>, if action was selected, <c>false</c> otherwise.</returns>
        //public bool SelectAction()
        //{
        //    //  Check if Action is still running.
        //    if (IsActionStillRunning())
        //        return false;

        //    //  Select the action to be executed.
        //    activeAction = ai.Select(context);
        //    activeAction.utilityAIComponent = taskNetwork;
        //    //  For debugging.
        //    selectorResults = GetSelectorResults(context, ai.rootSelector.qualifiers);

        //    return activeAction != null;
        //}


        //private bool IsActionStillRunning(){
        //    if (activeAction == null)
        //        return false;
        //    return activeAction.actionStatus == ActionStatus.Running;
        //}


        ///// <summary>
        /////  This gets the best Action to perform and executes it.
        ///// </summary>
        ///// <returns></returns>
        //public IEnumerator ExecuteAction()
        //{
        //    if (activeAction == null)
        //        yield break;

        //    if(debug) Debug.Log(string.Format("{0} is executing action. |  {1}", ai.name, Time.time));
        //    //  Set the current state to "Running".
        //    Start();

        //    //  Execute the current action.
        //    activeAction.ExecuteAction(context);

        //    while(activeAction.actionStatus == ActionStatus.Running){
        //        if(activeAction.actionStatus != ActionStatus.Running)
        //            break;
        //        yield return null;
        //    }
        //    //Debug.Log("Action Done Executing at:  " + Time.time);
        //    Stop();
        //    yield return null;
        //}


    }
}

namespace AtlasAI.Visualization
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The base class for all custom visualizers.   Provides a single method to override, which is called every time the specified type of AI entity is executed by the AI.
    /// </summary>
    /// <typeparam name="T">T is the type of AI entity to visualize, e.g. a specific or base-class Action.</typeparam>
    /// <typeparam name = "TData" > TData is the type of data to visualize and can be virtually anything. </typeparam>
    public abstract class CustomVisualizerComponent<T, TData> : ContextVisualizerComponent, ICustomVisualizer where T : class
    {
        //
        // Fields
        //
        protected Dictionary<IAIContext, TData> _data = new Dictionary<IAIContext, TData>();

        //
        // Properties
        //
        protected bool registerForDerivedTypes
        {
            get;
            set;
        }

        //
        // Constructors
        //
        protected CustomVisualizerComponent()
        {
            
        }

        //
        // Methods
        //
        protected override void Awake()
        {
            base.Awake();


        }



        public void EntityUpdate(object aiEntity, IAIContext context)
        {
            ////  Call event for GetDataForVisualization(activeAction, contextProvider.GetContext())
            //VisualizerManager.UpdateVisualizer(activeAction, contextProvider.GetContext());
            if(_data.ContainsKey(context)){
                Guid aiId = new Guid();
                TData data = GetDataForVisualization((T)aiEntity, context, aiId);
                _data[context] = data;
            }
            else{
                IAIContext key = GetComponent<IContextProvider>().GetContext();
                TData value = GetDataForVisualization((T)aiEntity, context, new Guid());
                _data.Add(key, value);
            }


        }


        protected abstract TData GetDataForVisualization(T aiEntity, IAIContext context, Guid aiId);




        protected virtual void OnEnable()
        {
            if(_data == null) _data = new Dictionary<IAIContext, TData>();
            VisualizerManager.RegisterVisualizer<T>(this);



            Debug.Log(VisualizerManager.DebugLogRegisteredVisualziers());
        }

        protected virtual void OnDisable()
        {
            _data = null;
            VisualizerManager.UnregisterVisualizer<T>();
            Debug.Log(VisualizerManager.DebugLogRegisteredVisualziers());
        }

    }
}

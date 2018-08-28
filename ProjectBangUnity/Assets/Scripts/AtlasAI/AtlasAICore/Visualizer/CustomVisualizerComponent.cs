namespace AtlasAI.Visualization
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// CustomVisualizerComponent
    /// </summary>
    /// <typeparam name="T">The concrete class type to visualize.</typeparam>
    /// <typeparam name = "TData" > The type of data.</typeparam>
    public abstract class CustomVisualizerComponent<T, TData> : ContextVisualizerComponent, ICustomVisualizer where T : class
    {
        //
        // Fields
        //
        protected Dictionary<IAIContext, TData> _data;

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

            if (_data == null) _data = new Dictionary<IAIContext, TData>();
        }


        public void EntityUpdate(T aiEntity, IAIContext context, Guid aiId)
        {


            GetDataForVisualization(aiEntity, context, aiId);
        }


        public void EntityUpdate(object aiEntity, IAIContext context)
        {
            ////  Call event for GetDataForVisualization(activeAction, contextProvider.GetContext())
            //VisualizerManager.UpdateVisualizer(activeAction, contextProvider.GetContext());
        }


        protected abstract TData GetDataForVisualization(T aiEntity, IAIContext context, Guid aiId);




        protected virtual void OnEnable()
        {
            if(_data == null) _data = new Dictionary<IAIContext, TData>();
            VisualizerManager.RegisterVisualizer<T>(this);
            //Debug.Log(VisualizerManager.DebugLogRegisteredVisualziers());
        }

        protected virtual void OnDisable()
        {
            _data = null;
            VisualizerManager.UnregisterVisualizer<T>();
            //Debug.Log(VisualizerManager.DebugLogRegisteredVisualziers());
        }

    }
}

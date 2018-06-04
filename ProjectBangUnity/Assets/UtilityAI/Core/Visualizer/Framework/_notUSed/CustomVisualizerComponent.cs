namespace UtilityAI.Visualization
{
    using UnityEngine;

    public class CustomVisualizerComponent<T, TData> : ICustomVisualizer
    {
        protected TData _data;
        public T registerForDerivedTypes { get; set; }


        protected virtual void Awake()
        {
            
        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }


        /// <summary>
        /// Called after an entity of the type associated with this visualizer has been executed in the AI, e.g. an <see cref="T:Apex.AI.IAction"/>.
        /// </summary>
        /// <param name="aiEntity">Ai entity.</param>
        /// <param name="context">Context.</param>
        public void EntityUpdate(object aiEntity, IAIContext context)
        {
            GetDataForVisualization((T)aiEntity,context);
        }


        /// <summary>
        /// Called after an entity of the type associated with this visualizer has been executed in the AI, e.g. an <see cref="T:Apex.AI.IAction"/>.
        /// </summary>
        /// <returns>The data for visualization.</returns>
        /// <param name="aiEntity">Ai entity.</param>
        /// <param name="context">Context.</param>
        protected virtual TData GetDataForVisualization(T aiEntity, IAIContext context)
        {
            TData data = default(TData);
            AIContext c = context as AIContext;
            Debug.LogFormat("aiEntity: {0}\nEntity:  {1}", aiEntity, c.entity);


            return data;
            //throw new System.NotImplementedException();
        }

    }
}


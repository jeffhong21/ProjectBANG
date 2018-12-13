namespace AtlasAI.Visualization
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// The final step for custom visualization. This base class covers the expected most common use case for custom visualization: 
    /// Visualizing the scores for each option used by an ActionWithOptions<TOption>. Provides an abstract method for getting the 
    /// desired options through the Context object.
    /// </summary>
    public abstract class ActionWithOptionsVisualizerComponent<T, TOption> : CustomGizmoGUIVisualizerComponent<T, List<ScoredOption<TOption>>> where T : ActionWithOptions<TOption>
    {
        //
        // Fields
        //
        private List<ScoredOption<TOption>> _scoredBuffer = new List<ScoredOption<TOption>>();


        //
        // Methods
        //
        protected override List<ScoredOption<TOption>> GetDataForVisualization(T aiEntity, IAIContext context, Guid aiId)
        {
            _scoredBuffer = Utilities.ListBufferPool.GetBuffer<ScoredOption<TOption>>(GetOptions(context).Count);
            aiEntity.GetAllScores(context, GetOptions(context), _scoredBuffer);
            Utilities.ListBufferPool.ReturnBuffer<ScoredOption<TOption>>(_scoredBuffer);
            //Debug.Log(_scoredBuffer.Count);
            return _scoredBuffer;
        }


        protected abstract List<TOption> GetOptions(IAIContext context);
    }





  //  /// <summary>
  //  /// Action with options visualizer component.
  //  /// </summary>
  //  /// <typeparam name="T">The concrete ActionWithOptions type</typeparam>
  //  /// <typeparam name = "TOption" > The type of the options.</typeparam>
  //  //public class ActionWithOptionsVisualizerComponent<T, TOption> : CustomGizmoGUIVisualizer<List<ScoredOption<TOption>>, TOption>
  //  //    where T : ActionWithOptions<TOption>
  //  //{
  //  public abstract class ActionWithOptionsVisualizerComponent<T, TOption> : MonoBehaviour, ICustomVisualizer
  //      where T : ActionWithOptions<TOption>
  //  {

  //      [SerializeField]
  //      protected bool drawGUI = true;
  //      [SerializeField]
  //      protected bool drawGizmo = true;
  //      protected List<ScoredOption<TOption>> data;


  //      private void OnEnable()
  //      {
  //          data = new List<ScoredOption<TOption>>();
  //          VisualizerManager.RegisterVisualizer<T>(this);
  //      }

		//private void OnDisable()
		//{
  //          data = null;
  //          VisualizerManager.UnregisterVisualizer<T>();
		//}


    //    protected virtual List<TOption> GetOptions(IAIContext context)
    //    {
    //        return new List<TOption>();
    //    }



    //    /// <summary>
    //    /// Called after an entity of the type associated with this visualizer has been executed in the AI, e.g. an <see cref="T:Apex.AI.IAction" />.
    //    /// </summary>
    //    /// <returns>The data for visualization.</returns>
    //    /// <param name="aiEntity">Ai entity.</param>
    //    /// <param name="context">Context.</param>
    //    /// <param name="aiID">Ai identifier.</param>
    //    public void EntityUpdate(object aiEntity, IAIContext context)
    //    {
    //        if(aiEntity.GetType() != typeof(T)){
    //            Debug.LogFormat("AiEntity object <{0}> is not of type ({1})", aiEntity, typeof(T));
    //            return;
    //        }

    //        GetDataForVisualization((T)aiEntity, context);
    //    }


    //    /// <summary>
    //    /// Called after an entity of the type associated with this visualizer has been executed in the AI, e.g. an <see cref="T:Apex.AI.IAction" />.
    //    /// </summary>
    //    /// <returns>The data for visualization.</returns>
    //    /// <param name="aiEntity">Ai entity.</param>
    //    /// <param name="context">Context.</param>
    //    /// <param name="aiID">Ai identifier.</param>
    //    protected List<ScoredOption<TOption>> GetDataForVisualization(T aiEntity, IAIContext context)
    //    {
    //        var scoredOptions = new List<ScoredOption<TOption>>();
    //        scoredOptions = aiEntity.GetAllScorers(context, GetOptions(context), scoredOptions);

    //        if(data != null){
    //            data.Clear();
    //            data = scoredOptions;
    //        }
    //        else{
    //            data = new List<ScoredOption<TOption>>(scoredOptions);
    //        }

    //        //IContext c = context as AIContext;
    //        //Debug.LogFormat("Getting data for visualization of <{0}> for {1}.\n Recieved {2} entires for scoredOptions", aiEntity, c.entity, scoredOptions.Count);

    //        return scoredOptions;
    //    }




    //    /// <summary>
    //    /// Ons the GUI.
    //    /// </summary>
    //    protected virtual void OnGUI()
    //    {
    //        if (data != null && drawGUI == true)
    //        {
    //            if (Camera.current == Camera.main || Camera.current == SceneView.lastActiveSceneView.camera)
    //            {
    //                DrawGUI(data);
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Ons the draw gizmos.
    //    /// </summary>
    //    protected virtual void OnDrawGizmos()
    //    {
    //        if (data != null && drawGUI == true)
    //        {
    //            if (Camera.current == Camera.main || Camera.current == SceneView.lastActiveSceneView.camera)
    //            {
    //                DrawGizmos(data);
    //            }
    //        }
    //    }



    //    protected abstract void DrawGUI(List<ScoredOption<TOption>> data);

    //    protected abstract void DrawGizmos(List<ScoredOption<TOption>> data);


    //}






}

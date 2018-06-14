namespace UtilityAI.Visualization
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Manager for the visualizers to register too.
    /// </summary>
    public static class VisualizerManager
    {
        static bool displayInfoText;

        //  The type to visualize and the visualizer associated with that type.
        private static Dictionary<Type, ICustomVisualizer> visualizers;

        private static List<IContextProvider> _visualizedContextProviders;


		internal static bool isVisualizing {
			get;
		}

		internal static IList<IContextProvider> visualizedContextProviders {
			get;
		}


        internal static bool BeginVisualization ()
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<Type> GetDerived (Type forType)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateSelectedGameObjects (GameObject[] selected)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registers the visualizer.
        /// </summary>
        /// <param name="visualizer">Visualizer.</param>
        /// <typeparam name="TFor">The type visualized by the custom visualizer.</typeparam>
        public static void RegisterVisualizer<TFor>(ICustomVisualizer visualizer)
        {
            if (visualizers == null){
                visualizers = new Dictionary<Type, ICustomVisualizer>();
            }


            if (visualizers.ContainsKey(typeof(TFor)) == false)
            {
                visualizers.Add(typeof(TFor), visualizer);
                //Debug.LogFormat("Registered {0} to a Visualizer", typeof(TFor));
            }
        }


        public static void UnregisterVisualizer<TFor>()
        {

            if (visualizers.ContainsKey(typeof(TFor)))
            {
                visualizers.Remove(typeof(TFor));
                //Debug.LogFormat("Unregistered {0} to a Visualizer", typeof(TFor));
            }
        }


        /// <summary>
        /// Updates the visualizer.
        /// </summary>
        /// <param name="forType">Type for.</param>
        /// <param name="context">Context.</param>
        public static void UpdateVisualizer(object aiEntity, IAIContext context)
        {
            if(visualizers.ContainsKey(aiEntity.GetType()))
            {
                ICustomVisualizer visualizer = null;
                if (visualizers.TryGetValue(aiEntity.GetType(), out visualizer))
                {
                    if (visualizer != null)
                    {
                        visualizer.EntityUpdate(aiEntity, context);
                    }

                    //Debug.LogFormat("Updated the Visualizer for :  {0}", aiEntity.GetType());
                }
            }

        }


    }
}




#region OLD
//using UnityEngine;
//public class VisualizerManager : MonoBehaviour
//{


//    Dictionary<Type, ICustomVisualizer> registeredVisualizers;


//    private static VisualizerManager visualizerManager;

//    Dictionary<string, UnityEvent> visualizerEvents;
//    public static VisualizerManager Instance
//    {
//        get
//        {
//            if (!visualizerManager)
//            {
//                visualizerManager = FindObjectOfType<VisualizerManager>() as VisualizerManager;

//                if (visualizerManager == false)
//                {
//                    Debug.LogError("There needs to be an active Visualizer Manager");
//                }
//                else
//                {
//                    if (visualizerManager.visualizerEvents == null)
//                    {
//                        visualizerManager.visualizerEvents = new Dictionary<string, UnityEvent>();
//                    }

//                }
//            }
//            return visualizerManager;
//        }
//    }


//    public void RegisterVisualizer<TFor>(string eventName, UnityAction listener)
//    {
//        UnityAction someAction;
//        UnityEvent thisEvent = null;
//        if (Instance.visualizerEvents.TryGetValue(eventName, out thisEvent))
//        {
//            thisEvent.AddListener(listener);
//        }
//        else
//        {
//            thisEvent = new UnityEvent();
//            thisEvent.AddListener(listener);
//            Instance.visualizerEvents.Add(eventName, thisEvent);
//        }

//    }

//    public void RegisterVisualizer(Type forType, ICustomVisualizer visualizer, bool registerDerivedTypes)
//    {

//    }



//    public void Unregister<TFor>(string eventName, UnityAction listener)
//    {
//        if (visualizerManager == null) return;
//        UnityEvent thisEvent = null;
//        if (Instance.visualizerEvents.TryGetValue(eventName, out thisEvent))
//        {
//            thisEvent.RemoveListener(listener);
//        }
//    }


//    public static void TriggerEvent(string eventName)
//    {
//        UnityEvent thisEvent = null;
//        if (!Instance.visualizerEvents.TryGetValue(eventName, out thisEvent))
//        {
//            thisEvent.Invoke();
//        }
//    }

//}
#endregion
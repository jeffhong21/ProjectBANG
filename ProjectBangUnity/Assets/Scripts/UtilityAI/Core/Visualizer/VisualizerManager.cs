namespace UtilityAI.Visualization
{
    using UnityEngine;
    using System;
    using System.Reflection;
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


        public static bool isVisualizing {
			get;
		}

        public static List<IContextProvider> visualizedContextProviders
        {
            get
            {
                if(_visualizedContextProviders == null) _visualizedContextProviders = new List<IContextProvider>();

                foreach(TaskNetworkComponent taskNetwork in Utilities.ComponentHelper.FindAllComponentsInScene<TaskNetworkComponent>())
                {
                    _visualizedContextProviders.Add(taskNetwork.GetComponent<IContextProvider>());
                }
                return _visualizedContextProviders;
            }
		}


        public static bool BeginVisualization ()
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<Type> GetDerived (Type forType)
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

            // TODO:  need to search for every agent that contains a class that derives from visualizer. 
        }


        public static void UnregisterVisualizer<TFor>()
        {
            if (visualizers == null)
            {
                return;
            }

            if (visualizers.ContainsKey(typeof(TFor)))
            {
                visualizers.Remove(typeof(TFor));
                //Debug.LogFormat("Unregistered {0} to a Visualizer", typeof(TFor));
            }
        }



        public static bool TryGetVisualizerFor(Type t, out ICustomVisualizer visualizer)  //  internal
        {
            if (visualizers.ContainsKey(t))
            {
                visualizer = visualizers[t];
                return true;
            }

            visualizer = null;
            return false;
        }


        public static void UpdateSelectedGameObjects(GameObject[] selected)  //  internal
        {
            for (int index = 0; index < selected.Length; index++)
            {
                //  Check if selected gameObject contains taskNetwork component.
                if(selected[index].GetComponent<TaskNetworkComponent>() == true)
                {
                    //  Get the TaskNetworkComponent.
                    TaskNetworkComponent go = selected[index].GetComponent<TaskNetworkComponent>();
                    //  Get Context.
                    IAIContext context = go.GetComponent<IContextProvider>().GetContext();
                    //  Loop through each aiConfig.
                    for (int i = 0; i < go.aiConfigs.Length; i++)
                    {
                        //  Get aiId.
                        string aiId = go.aiConfigs[i].aiId;
                        //  Get ai Guid.
                        var field = typeof(AINameMapHelper).GetField(aiId, BindingFlags.Public | BindingFlags.Static);
                        Guid guid = (Guid)field.GetValue(null);

                        ICustomVisualizer visualizer = null;
                        if (TryGetVisualizerFor(go.GetType(), out visualizer))
                        {
                            visualizer.EntityUpdate(go.GetType(), context);
                            //EntityUpdate(object aiEntity, IAIContext context, Guid aiId)
                        }
                    }


                }
                

            }
        }



        /// <summary>
        /// Updates the visualizer.
        /// </summary>
        /// <param name="aiEntity">Type for.</param>
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

        public static string DebugLogRegisteredVisualziers(bool indent = false)
        {
            string registeredVisualziersLog = "";
            string indentString = "|";

            if(indent){
                indentString = "\n    ";
            }

            if(visualizers != null)
            {
                foreach (KeyValuePair<Type, ICustomVisualizer> visualizer in visualizers)
                {
                    registeredVisualziersLog += string.Format("VisualizerType:  {0} {2} ICustomVisualizer:  {1}\n", visualizer.Key.Name, visualizer.Value.GetType(), indentString);
                }
            }


            return registeredVisualziersLog;
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
//namespace Bang //UtilityAI.Visualization
//{
//    using UnityEngine;
//    using UnityEditor;
//    using System.Linq;
//    using System;
//    using System.Collections.Generic;

//    using UtilityAI.Visualization;
//    using UtilityAI;


//    public class TaskNetworkDiagnostics : MonoBehaviour
//    {
//        #region Singleton

//        private static TaskNetworkDiagnostics taskNetworkDiagnostics;
//        public static TaskNetworkDiagnostics instance
//        {
//            get
//            {
//                if (taskNetworkDiagnostics == null)
//                {
//                    taskNetworkDiagnostics = FindObjectOfType<TaskNetworkDiagnostics>() as TaskNetworkDiagnostics;

//                    if (taskNetworkDiagnostics == false){
//                        Debug.LogError("There is no TaskNetworkDiagnostics");
//                    }
//                }
//                return taskNetworkDiagnostics;
//            }
//        }
//        #endregion


//        TaskNetworkComponent taskNetwork;

//        string diagnosticsInfo;

//        bool showDiagnostics;
//        int clientIndex;



//        //  All the Keycode values.
//        int[] values;
//        //  Keys that are pressed.
//        bool[] keys;


//        public bool ShiftIndexInput
//        {
//            get
//            {
//                bool keyInput = false;
//                for (int i = 0; i < values.Length; i++)
//                {
//                    keys[i] = Input.GetKeyUp((KeyCode)values[i]);
//                    if (keys[i])
//                    {
//                        keyInput = keys[i];
//                        break;
//                    }
//                }
//                return keyInput && Input.GetKeyDown(KeyCode.LeftShift);
//            }
//        }



//        int indentSpacing = 12;
//        //  name cache variables.
//        string selectorName, qualifierName, scorerName, actionName, defaultQualifierName;


//        protected void Awake()
//        {
//            taskNetworkDiagnostics = this;
//            taskNetwork = GetComponent<TaskNetworkComponent>();
//        }

//		protected void OnEnable()
//		{
//            values = (int[])System.Enum.GetValues(typeof(KeyCode));
//            keys = new bool[values.Length];

//            if(taskNetwork == null){
//                Debug.Log("Task Network Missing, adding it now.");
//                taskNetwork = GetComponent<TaskNetworkComponent>();
//            }
//		}

//        protected void OnDisable()
//		{
//            values = null;
//            keys = null;
//		}



//        private void Update()
//        {
//            //TaskNetworkComponent[] taskNetworks = Selection.GetFiltered<TaskNetworkComponent>(SelectionMode.TopLevel);

//            for (int i = 0; i < values.Length; i++)
//            {
//                keys[i] = Input.GetKeyUp((KeyCode)values[i]);
//                if (keys[i])
//                {
//                    //  Number keys start at 49.
//                    var index = values[i] - 49;
//                    if (taskNetwork.clients.Count() >= index + 1)
//                    {
//                        if(clientIndex == index){
//                            showDiagnostics = !showDiagnostics;
//                        }
//                        else{
//                            clientIndex = index;
//                        }
//                        ClientDiagnosticsInfo(clientIndex);
//                    }
//                }
//            }


//        }


//        void OnGUI()
//        {
//            if(showDiagnostics && clientIndex < taskNetwork.clients.Count())
//            {
//                GUILayout.BeginArea(new Rect(5f, Screen.height * 0.2f, Screen.width * 0.2f, Screen.height * 0.6f), GUI.skin.box);


//                //GUILayout.Label(diagnosticsInfo, TaskNetworkDiagnosticsStyle.textStyle);

//                GUILayout.Label("AI:  " + taskNetwork.clients[clientIndex].ai.name, TaskNetworkDiagnosticsStyle.textStyle);
//                GUILayout.Label("Selector:  " + taskNetwork.clients[clientIndex].ai.rootSelector.GetType().Name, TaskNetworkDiagnosticsStyle.textStyle);

//                for (int i = 0; i < taskNetwork.clients[clientIndex].ai.rootSelector.qualifiers.Count; i++)
//                {
//                    IQualifier qualifier = taskNetwork.clients[clientIndex].ai.rootSelector.qualifiers[i];
//                    using (new EditorGUILayout.HorizontalScope())
//                    {
//                        GUILayout.Space(indentSpacing * 1);
//                        GUILayout.Label(qualifier.GetType().Name, TaskNetworkDiagnosticsStyle.textStyle);
//                        GUILayout.Label("0", TaskNetworkDiagnosticsStyle.textStyle);
//                    }

//                    if(qualifier is CompositeQualifier)
//                    {
//                        foreach (IScorer scorer in ((CompositeQualifier)qualifier).scorers)
//                        {
//                            using (new EditorGUILayout.HorizontalScope())
//                            {
//                                GUILayout.Space(indentSpacing * 3);
//                                GUILayout.Label(scorer.GetType().Name, TaskNetworkDiagnosticsStyle.textStyle);
//                                GUILayout.Label("25", TaskNetworkDiagnosticsStyle.textStyle);
//                            }
//                        }
//                    }
//                    IAction action = qualifier.action;
//                    using (new EditorGUILayout.HorizontalScope())
//                    {
//                        GUILayout.Space(indentSpacing * 2);
//                        GUILayout.Label(action.GetType().Name, TaskNetworkDiagnosticsStyle.textStyle);
//                    }
//                }

//                using (new EditorGUILayout.HorizontalScope())
//                {
//                    GUILayout.Space(indentSpacing * 1);
//                    var defaultQualifier = taskNetwork.clients[clientIndex].ai.rootSelector.defaultQualifier;
//                    GUILayout.Label(defaultQualifier.GetType().Name, TaskNetworkDiagnosticsStyle.textStyle);
//                    GUILayout.Label("0", TaskNetworkDiagnosticsStyle.textStyle);
//                }





//                GUILayout.EndArea();
//            }


//        }



//        void ClientDiagnosticsInfo(int index)
//        {
//            diagnosticsInfo = "";

//            IUtilityAI utilityAI = taskNetwork.clients[index].ai;
//            Selector rootSelector = utilityAI.rootSelector;
//            bool isSelected = false;

//            diagnosticsInfo += string.Format("<b>AI Name:  {0}</b>\n", utilityAI.name);
//            foreach (IQualifier qualifier in rootSelector.qualifiers)
//            {
//                isSelected = taskNetwork.clients[index].activeAction == qualifier.action;
//                diagnosticsInfo += QualifierNetworkInfo(qualifier, isSelected);
//            }

//            isSelected = taskNetwork.clients[index].activeAction == rootSelector.defaultQualifier.action;
//            diagnosticsInfo += QualifierNetworkInfo((DefaultQualifier)rootSelector.defaultQualifier, isSelected);
//        }

//        string QualifierNetworkInfo(IQualifier qualifier, bool isSelected)
//        {
//            //string networkInfo = " <b>Qualifier:</b> {0} | <b>Score:</b>: <color=lime>{1}</color>\n <b>Action:</b>:  <color=lime>{2}</color>\n";
//            string networkInfo = "";

//            string qualiferName = "";
//            //string qualifierScore;
//            string[] scorers;
//            //string[] scorersScores;
//            string actionName = "";
//            string[] optionScorers;

//            if (qualifier is DefaultQualifier)
//                qualiferName = string.Format("<b>{0}</b>{1}", "DefaultQualifier:  ", qualifier.GetType().Name);
//            else
//                qualiferName = string.Format("<b>{0}</b>{1}", "Qualifier:  ", qualifier.GetType().Name);


//            networkInfo += string.Format(" {0}\n", qualiferName);


//            if (qualifier is CompositeQualifier)
//            {
//                var _scorers = ((CompositeQualifier)qualifier).scorers;
//                int count = _scorers.Count;
//                scorers = new string[count];
//                //scorersScores = new string[count];
//                for (int i = 0; i < count; i++)
//                {
//                    scorers[i] = string.Format("<b>{0}</b>{1} ", "Scorer: ", _scorers[i].GetType().Name);
//                    networkInfo += string.Format(" ---- {0}\n", scorers[i]);
//                }
//            }

//            if (qualifier is DefaultQualifier)
//            {
//                string _actionName = qualifier.action == null ? "<None>" : qualifier.action.name;
//                actionName = string.Format("<b>{0}</b>{1}", "Action: ", _actionName);
//                networkInfo += string.Format(" -- {0}\n", actionName);
//                return networkInfo;
//            }

//            var action = qualifier.action;
//            actionName = string.Format("<b>{0}</b>{1}", "Action: ", action.name);
//            networkInfo += string.Format(" -- {0} <{1}>\n", actionName, action.GetType());

//            //if (action.GetType() == typeof(ActionWithOptions<>))
//            if (action is ActionWithOptions<Vector3>)
//            {
//                var _actionWithOption = action as ActionWithOptions<Vector3>;
//                var _optionScorers = _actionWithOption.scorers;
//                int count = _optionScorers.Count;

//                optionScorers = new string[count];
//                //scorersScores = new string[count];
//                for (int i = 0; i < count; i++)
//                {
//                    optionScorers[i] = string.Format("<b>{0}</b>{1} ", "OptionScorer: ", _optionScorers[i].GetType().Name);
//                    networkInfo += string.Format("   - {0}\n", _optionScorers[i]);
//                }
//                //actionName += string.Format(" <{0}>", optionType);
//            }

//            return networkInfo;
//        }

//        //string QualifierNetworkInfo(IQualifier qualifier, bool isSelected)
//        //{
//        //    //string networkInfo = " <b>Qualifier:</b> {0} | <b>Score:</b>: <color=lime>{1}</color>\n <b>Action:</b>:  <color=lime>{2}</color>\n";
//        //    string networkInfo = "";

//        //    string qualiferName = "";
//        //    string qualifierScore;
//        //    string[] scorers;
//        //    string[] scorersScores;
//        //    string actionName = "";
//        //    IAIContext context = gameObject.GetComponent<IContextProvider>().GetContext();


//        //    qualiferName = string.Format("<b>{0}</b>", qualifier.GetType().Name);
//        //    qualifierScore = isSelected ? string.Format("<color=lime>{0}</color>", qualifier.Score(context)) : qualifier.Score(context).ToString();
//        //    networkInfo += string.Format("{0} : \t\t {1}\n", qualiferName, qualifierScore);


//        //    if(qualifier is CompositeQualifier)
//        //    {
//        //        var _scorers = ((CompositeQualifier)qualifier).scorers;
//        //        int count = _scorers.Count;
//        //        scorers = new string[count];
//        //        scorersScores = new string[count];
//        //        for (int i = 0; i < count; i ++)
//        //        {
//        //            scorers[i] = _scorers[i].GetType().Name;
//        //            scorersScores[i] = isSelected ? string.Format("<color=lime>{0}</color>", qualifier.Score(context)) : qualifier.Score(context).ToString();
//        //            networkInfo += string.Format("\t{0} : \t {1}\n", scorers[i], scorersScores[i]);
//        //        }
//        //    }

//        //    actionName = string.Format("<b>{0}</b>", qualifier.action.name);
//        //    networkInfo += string.Format("        <b>{0}:</b>\n", actionName);


//        //    return networkInfo;
//        //}










//        #region Old


//        public string SelectorInfo(UtilityAI s)
//        {
//            var selector = s.rootSelector;

//            string selectorInfo = "";
//            selectorInfo += string.Format("** taskNetwork Name: :  {0} **\n\n", s.GetType().Name);
//            selectorInfo += string.Format("  Selector Type:  {0}\n", selector.GetType().Name);

//            //  Get Selector Name and Type.
//            for (int i = 0; i < selector.qualifiers.Count(); i++)
//            {
//                var qualifier = selector.qualifiers[i] as CompositeQualifier;
//                string qualifierInfo = "";
//                string scorerInfo = "";
//                string actionInfo = "";

//                qualifierInfo += string.Format("{0}", qualifier.GetType().Name);

//                foreach (IScorer scorer in qualifier.scorers)
//                {
//                    scorerInfo += string.Format("    - {0}\n", scorer.GetType().Name);
//                }
//                actionInfo += string.Format("{0}", qualifier.action.GetType().Name);



//                selectorInfo += string.Format("  Qualifier:    {0}\n", qualifierInfo);
//                selectorInfo += string.Format("  Action:       {0}\n", actionInfo);
//                selectorInfo += string.Format("  Number of Scorers:  {0}\n", qualifier.scorers.Count());
//                selectorInfo += scorerInfo;
//                selectorInfo += "\n";
//            }

//            return selectorInfo;
//        }

//        #endregion

//    }


//    public static class TaskNetworkDiagnosticsStyle
//    {
//        [Range(6f, 24f)]
//        public static int fontSize = 12;
//        public static GUIStyle textStyle;


//        static TaskNetworkDiagnosticsStyle()
//        {
//            textStyle = new GUIStyle();
//            textStyle.normal.textColor = Color.white;
//            textStyle.fontSize = fontSize;
//            textStyle.richText = true;
//        }
//    }

//}


namespace UtilityAI
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using UnityEditor;



    [CustomEditor(typeof(TaskNetworkComponent))]
    public class TaskNetworkEditor : Editor
    {
        TaskNetworkComponent taskNetwork;
        SerializedObject taskNetworkSO;
        IContextProvider contextProvider;

        //  Currently selected Client.  Used for the Editor tab.
        UtilityAIAsset activeClient;
        SerializedObject activeClientSO;
        GenericMenu clientList;

        bool showDefaultInspector, showDeleteAssetOption;
        bool debugEditorFoldout = true;
        string selectorConfigInfo;


        bool displayContextProviderName = true;


        void OnEnable()
        {
            taskNetwork = target as TaskNetworkComponent;
            taskNetworkSO = new SerializedObject(taskNetwork);
            contextProvider = taskNetwork.GetComponent<IContextProvider>();

            //UpdateClientList();

            //selectorConfigInfo = activeClient != null ? DebugEditorUtilities.SelectorConfig(activeClient.configuration.selector) : "No Selected Selector";
            selectorConfigInfo = activeClient != null ? DebugEditorUtilities.SelectorConfig(activeClient.configuration.rootSelector) : "No Selected Selector";
        }



        public void AddNew()
        {
            AddClientWindow window = new AddClientWindow();
            window.Init(window, this);
        }


        public void Delete(int index){
            activeClient = null;
            taskNetwork.clients.RemoveAt(index);
            //taskNetwork.assets.RemoveAt(index);

            EditorUtility.SetDirty(target);
            Repaint();
        }


        public void DoAddNew(UtilityAIAsset aiAsset)
        {
            //  Use the aiID to be able to grab the correct UtilityAIAsset;

            //  Add asset and client to TaskNetwork
            //UtilityAIClient client = new UtilityAIClient(aiAsset.configuration, taskNetwork.contextProvider);
            UtilityAIClient client = new UtilityAIClient(aiAsset.configuration, taskNetwork.GetComponent<IContextProvider>());
            client.ai = aiAsset.configuration;
            taskNetwork.clients.Add(client);
            //taskNetwork.assets.Add(aiAsset);

            EditorUtility.SetDirty(taskNetwork);
        }


        #region AI Client Inspector

        /// <summary>
        /// Client Inspector
        /// </summary>
        protected virtual void DrawTaskNetworkInspector()
        {

            //  Displaying header options.
            using (new EditorGUILayout.HorizontalScope()){
                EditorGUILayout.LabelField("AIs", EditorStyles.boldLabel);


                //  Button to add predefined mocks.
                if (GUILayout.Button("Add Predefined", EditorStyles.miniButton, GUILayout.Width(130f)))
                {
                    CreatePredefinedClientWindow window = new CreatePredefinedClientWindow();
                    window.Init(window, taskNetwork);
                }

                //  Add a new UtilityAIClient
                if (GUILayout.Button("Add", EditorStyles.miniButton, GUILayout.Width(65f))){
                    AddNew();
                }
            }


            //  Displaying the AI Clients
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                if (taskNetwork.clients.Count == 0){
                    EditorGUILayout.HelpBox("There are no AI's attached to this TaskNetworkComponent.", MessageType.Info);
                }


                for (int i = 0; i < taskNetwork.clients.Count; i++)  
                {
                    UtilityAIClient client = taskNetwork.clients[i];
                    //UtilityAIAsset asset = taskNetwork.assets[i];
                    using (new EditorGUILayout.VerticalScope())
                    {
                        if (client != null){
                            //  For Client Options
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.ToggleLeft(GUIContent.none, true);  // GUILayout.Width(Screen.width * 0.6f)

                                //  Debug button
                                if (GUILayout.Button("Debug", EditorStyles.miniButton, GUILayout.Width(48f))){  //  GUILayout.Width(Screen.width * 0.15f)
                                    Debug.Log(ClientDiagnosticsInfo(i));
                                    //Debug.Log(client.ai.jsonData);
                                    //Debug.Log(asset.configuration.jsonData);
                                }

                                //  Add button
                                if (InspectorUtility.OptionsPopupButton(InspectorUtility.AddContent)){
                                    AddPredefinedAIWindow window = new AddPredefinedAIWindow();
                                    //window.Init(window,taskNetwork, asset);
                                    window.Init(window, taskNetwork, client.ai);
                                }

                                //  Delete button
                                if (InspectorUtility.OptionsPopupButton(InspectorUtility.DeleteContent)){
                                    Delete(i);
                                }
                            }


                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.LabelField(new GUIContent("AI: "), GUILayout.Width(Screen.width * 0.33f));
                                //EditorGUILayout.LabelField(new GUIContent(asset.friendlyName));
                                EditorGUILayout.LabelField(new GUIContent(client.ai.name));  //  Name resets after scene reload
                            }


                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.LabelField("Interval: ", GUILayout.Width(Screen.width * 0.33f));
                                client.intervalMin = EditorGUILayout.FloatField(client.intervalMin, GUILayout.Width(35f));
                                EditorGUILayout.LabelField("to ", GUILayout.Width(20f));
                                client.intervalMax = EditorGUILayout.FloatField(client.intervalMax, GUILayout.Width(35f));
                            }

                            //  For Client StartDelay
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                //EditorGUILayout.LabelField("Start Delay: ", GUILayout.Width(Screen.width * 0.33f));
                                //client.startDelayMin = EditorGUILayout.FloatField(client.startDelayMin, GUILayout.Width(35f));
                                //EditorGUILayout.LabelField("to ", GUILayout.Width(20f));
                                //client.startDelayMax = EditorGUILayout.FloatField(client.startDelayMax, GUILayout.Width(35f));
                                float min = client.startDelayMin;
                                float max = client.startDelayMax;
                                InspectorUtility.MinMaxInputField(ref min, ref max, new GUIContent("Start Delay: "));
                            }
                            EditorGUILayout.Space();
                        }
                    }
                }

            }  // The group is now ended





            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Editor", EditorStyles.miniButton, GUILayout.Width(65f))){
                    AIAssetEditor.Init();
                }
            }
            GUILayout.Space(8);






        }


        #endregion



        public override void OnInspectorGUI()
        {
            serializedObject.Update();


            using (new EditorGUILayout.HorizontalScope())
            {
                taskNetwork.showDefaultInspector = EditorGUILayout.ToggleLeft("Show Default Inspector", taskNetwork.showDefaultInspector);

                if (GUILayout.Button("Clear", EditorStyles.miniButton, GUILayout.Width(65f)))
                {
                    for (int index = 0; index < taskNetwork.clients.Count; index ++){
                        Delete(index);
                    }
                }
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                taskNetwork.showDeleteAssetOption = EditorGUILayout.ToggleLeft("Show Delete Asset Btn", taskNetwork.showDeleteAssetOption);
                if(taskNetwork.showDeleteAssetOption){
                    if (GUILayout.Button("Delete", EditorStyles.miniButton, GUILayout.Width(65f)))
                    {
                        for (int index = 0; index < taskNetwork.clients.Count; index++)
                        {
                            Delete(index);
                        }
                        var results = AssetDatabase.FindAssets("t:UtilityAIAsset", new string[]{AiManager.StorageFolder} );
                        foreach(string guid in results){
                            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));
                        }
                        //Repaint();
                    }
                }
            }
            //using (new EditorGUILayout.HorizontalScope())
            //{
            //    taskNetwork.selectAiAssetOnCreate = EditorGUILayout.ToggleLeft("Select Asset On Create", taskNetwork.selectAiAssetOnCreate);
            //}

            GUILayout.Space(8);

            if (taskNetwork.showDefaultInspector)
            {
                DrawDefaultInspector();
                GUILayout.Space(8);
            } 


            //  Draw the TaskNetworkInspector
            DrawTaskNetworkInspector();


            //  Display a message box if there is not ContextProvider.
            if (contextProvider == null)
            {
                var contextMsg = string.Format("{0} has no context provider.", target.name);
                EditorGUILayout.HelpBox(contextMsg, MessageType.Error);
                EditorGUILayout.Space();
            }
            else if (contextProvider != null && displayContextProviderName)
            {
                var contextMsg = string.Format("ContextProvider name:  {0}", contextProvider.GetType());
                EditorGUILayout.HelpBox(contextMsg, MessageType.Info);
            }



            serializedObject.ApplyModifiedProperties();
        }











        string ClientDiagnosticsInfo(int index)
        {
            string clientInfo = "";

            UtilityAI utilityAI = taskNetwork.clients[index].ai;
            Selector rootSelector = utilityAI.rootSelector;
            bool isSelected = false;

            clientInfo += string.Format("<b>AI Name:  {0}</b>\n", utilityAI.name);
            foreach (IQualifier qualifier in rootSelector.qualifiers)
            {
                isSelected = taskNetwork.clients[index].activeAction == qualifier.action;
                clientInfo += QualifierNetworkInfo(qualifier, isSelected);
            }

            isSelected = taskNetwork.clients[index].activeAction == rootSelector.defaultQualifier.action;
            clientInfo += QualifierNetworkInfo((DefaultQualifier)rootSelector.defaultQualifier, isSelected);

            return clientInfo;
        }



        string QualifierNetworkInfo(IQualifier qualifier, bool isSelected)
        {
            //string networkInfo = " <b>Qualifier:</b> {0} | <b>Score:</b>: <color=lime>{1}</color>\n <b>Action:</b>:  <color=lime>{2}</color>\n";
            string networkInfo = "";

            string qualiferName = "";
            //string qualifierScore;
            string[] scorers;
            //string[] scorersScores;
            string actionName = "";
            string[] optionScorers;

            if (qualifier is DefaultQualifier)
                qualiferName = string.Format("<b>{0}</b>{1}", "DefaultQualifier:  ", qualifier.GetType().Name);
            else
                qualiferName = string.Format("<b>{0}</b>{1}", "Qualifier:  ", qualifier.GetType().Name);


            networkInfo += string.Format(" {0}\n", qualiferName);


            if (qualifier is CompositeQualifier)
            {
                var _scorers = ((CompositeQualifier)qualifier).scorers;
                int count = _scorers.Count;
                scorers = new string[count];
                //scorersScores = new string[count];
                for (int i = 0; i < count; i++)
                {
                    scorers[i] = string.Format("<b>{0}</b>{1} ", "Scorer: ", _scorers[i].GetType().Name);
                    networkInfo += string.Format(" -------- {0}\n", scorers[i]);
                }
            }

            if (qualifier is DefaultQualifier)
            {
                string _actionName = qualifier.action == null ? "<None>" : qualifier.action.name;
                actionName = string.Format("<b>{0}</b>{1}", "Action: ", _actionName);
                networkInfo += string.Format(" -- {0}\n", actionName);
                return networkInfo;
            }

            var action = qualifier.action;
            actionName = string.Format("<b>{0}</b>{1}", "Action: ", action.name);
            //networkInfo += string.Format(" -- {0} <{1}>\n", actionName, action.GetType());
            networkInfo += string.Format(" -- {0}\n", actionName);

            //if (action.GetType() == typeof(ActionWithOptions<>))
            if (action is ActionWithOptions<Vector3>)
            {
                var _actionWithOption = action as ActionWithOptions<Vector3>;
                var _optionScorers = _actionWithOption.scorers;
                int count = _optionScorers.Count;

                optionScorers = new string[count];
                for (int i = 0; i < count; i++)
                {
                    optionScorers[i] = string.Format("<b>{0}</b>{1} ", "OptionScorer: ", _optionScorers[i].GetType().Name);
                    networkInfo += string.Format("     ----- {0}\n", optionScorers[i]);
                }
            }
            else if(action is ActionWithOptions<Bang.IHasHealth>)
            {
                var _actionWithOption = action as ActionWithOptions<Bang.IHasHealth>;
                var _optionScorers = _actionWithOption.scorers;
                int count = _optionScorers.Count;

                optionScorers = new string[count];
                for (int i = 0; i < count; i++)
                {
                    optionScorers[i] = string.Format("<b>{0}</b>{1} ", "OptionScorer: ", _optionScorers[i].GetType().Name);
                    networkInfo += string.Format("     ----- {0}\n", optionScorers[i]);
                }
            }

            return networkInfo;
        }







    }






}
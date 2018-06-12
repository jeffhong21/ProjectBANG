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





        bool showDefaultInspector, showDeleteAssetOption;
        bool debugEditorFoldout = true;



        bool displayContextProviderName = false;


        void OnEnable()
        {
            taskNetwork = target as TaskNetworkComponent;

            if(taskNetworkSO == null)
                taskNetworkSO = new SerializedObject(taskNetwork);
            
            contextProvider = taskNetwork.GetComponent<IContextProvider>();


        }



        public void AddNew()
        {
            AddClientWindow window = new AddClientWindow();
            window.Init(window, this);
        }


        public void Delete(int idx)
        {

            taskNetwork.aiConfigs = ShrinkArray(taskNetwork.aiConfigs, idx);

            EditorUtility.SetDirty(target);
            Repaint();
        }


        public void DoAddNew(AIStorage[] aiAsset)
        {
            //  Use the aiID to be able to grab the correct AIStorage;
            //serializedObject.Update();

            taskNetwork.aiConfigs = GrowArray(taskNetwork.aiConfigs, aiAsset.Length);

            for (int idx = 0; idx < aiAsset.Length; idx ++)
            {
                taskNetwork.aiConfigs[ (taskNetwork.aiConfigs.Length - aiAsset.Length) + idx] = new UtilityAIConfig()
                {
                    aiId = aiAsset[idx].aiId
                };
            }

            EditorUtility.SetDirty(taskNetwork);
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }


        public void DoAddNew(AIStorage[] aiAsset, Type type)
        {

            ////  Create a new UtilityAIClient
            //UtilityAIClient client = new UtilityAIClient(aiAsset[0].configuration, taskNetwork.GetComponent<IContextProvider>());
            //client.ai = aiAsset[0].configuration;      //  Add the UtilityAI to the UtilityAIClient.
            //taskNetwork.clients.Add(client);          //  Add the client to the TaskNetwork.

            ////  Initialize AIConfig so we can get the name of the predefined config.
            //IUtilityAIConfig config = (IUtilityAIConfig)Activator.CreateInstance(type);
            ////  Configure the predefined settings.
            //config.ConfigureAI(client.ai);



            taskNetwork.aiConfigs = GrowArray(taskNetwork.aiConfigs, aiAsset.Length);

            for (int idx = 0; idx < aiAsset.Length; idx++)
            {
                taskNetwork.aiConfigs[(taskNetwork.aiConfigs.Length - aiAsset.Length) + idx] = new UtilityAIConfig()
                {
                    aiId = aiAsset[idx].aiId,
                    isPredefined = true,
                    type = type.ToString()
                };
            }

            EditorUtility.SetDirty(taskNetwork);
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();


            //  Add asset and client to TaskNetwork
            //UtilityAIClient client = new UtilityAIClient(aiAsset.configuration, taskNetwork.contextProvider);

            //UtilityAIClient client = new UtilityAIClient(aiAsset[0].configuration, taskNetwork.GetComponent<IContextProvider>());
            //client.ai = aiAsset[0].configuration;
            //taskNetwork.clients.Add(client);

            //taskNetwork.assets.Add(aiAsset);
        }



        private T[] GrowArray<T>(T[] array, int increase)
        {
            T[] newArray = array;
            Array.Resize(ref newArray, array.Length + increase);
            return newArray;
        }

        private T[] ShrinkArray<T>(T[] array, int idx)
        {
            T[] newArray = new T[array.Length - 1];
            if (idx > 0)
                Array.Copy(array, 0, newArray, 0, idx);

            if (idx < array.Length - 1)
                Array.Copy(array, idx + 1, newArray, idx, array.Length - idx - 1);

            return newArray;
        }



        #region AI Client Inspector

        float propertyHeight;
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
                    AddPredefinedAIWindow window = new AddPredefinedAIWindow();
                    window.Init(window, this);
                }

                //  Add a new UtilityAIClient
                if (GUILayout.Button("Add", EditorStyles.miniButton, GUILayout.Width(65f))){
                    AddNew();
                }
            }


            //  Displaying the AI Clients

            //var aiClientsDisplayBox = new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Height(propertyHeight));
            var aiClientsDisplayBox = new EditorGUILayout.VerticalScope(EditorStyles.helpBox);
            using (aiClientsDisplayBox)
            {
                if (taskNetwork.aiConfigs.Length == 0){
                    EditorGUILayout.HelpBox("There are no AI's attached to this TaskNetworkComponent.", MessageType.Info);
                }

                using (new EditorGUILayout.VerticalScope())
                {
                    for (int i = 0; i < taskNetwork.aiConfigs.Length; i++)
                    {
                        if (taskNetwork.aiConfigs[i] != null)
                        {
                            UtilityAIConfig aiConfig = taskNetwork.aiConfigs[i];
                            SerializedProperty property = serializedObject.FindProperty("aiConfigs").GetArrayElementAtIndex(i);
                            SerializedProperty isActive = property.FindPropertyRelative("isActive");
                            SerializedProperty _debugClient = property.FindPropertyRelative("_debugClient");

                            //  For Client Options
                            using (new EditorGUILayout.HorizontalScope())
                            {

                                //  aiConfig isActive toggle.
                                EditorGUILayout.PropertyField(isActive, GUIContent.none);
                                //EditorGUILayout.PropertyField(isActive, GUIContent.none, GUILayout.Width(28f));

                                ////  Debug Client toggle
                                //GUILayout.Space(Screen.width * 0.12f);
                                //EditorGUILayout.LabelField("Debug Client", GUILayout.Width(75f));
                                //EditorGUILayout.PropertyField(_debugClient, GUIContent.none, GUILayout.Width(28f));
                                //GUILayout.Space(Screen.width * 0.25f - 75f - 28f);

                                //  Debug button
                                if (GUILayout.Button("Debug", EditorStyles.miniButton, GUILayout.Width(48f)))//  GUILayout.Width(Screen.width * 0.15f)
                                {
                                    //Debug.Log("Currently cannot debug at the moment.");

                                    //Debug.Log(typeof(AINameMapHelper).GetField(aiConfig.aiId, BindingFlags.Static).GetValue(null));

                                    //var field = typeof(AINameMapHelper).GetField(aiConfig.aiId, BindingFlags.Static);
                                    //Debug.Log(field.GetValue(typeof(AINameMapHelper)));

                                    Debug.Log(aiConfig.aiId);
                                }

                                //  Add or Switch button
                                if (InspectorUtility.OptionsPopupButton(InspectorUtility.AddContent))
                                {
                                    Debug.Log("TODO:  Add functionality for changing AIs.");
                                }

                                //  Delete button
                                if (InspectorUtility.OptionsPopupButton(InspectorUtility.DeleteContent))
                                {
                                    Delete(i);
                                }
                            }

                            //  Draw the aiconfigs.
                            EditorGUILayout.PropertyField(property);


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
                    for (int index = taskNetwork.aiConfigs.Length - 1; index >= 0; index--)
                    {
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
                        for (int index = taskNetwork.aiConfigs.Length -1 ; index >= 0 ; index--)
                        {
                            Delete(index);
                        }
                        var results = AssetDatabase.FindAssets("t:AIStorage", new string[]{AIManager.StorageFolder} );
                        foreach(string guid in results)
                        {
                            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));
                            AINameMap.UnRegister(guid);
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


            //  Name Map
            if (GUILayout.Button("AINameMap", EditorStyles.miniButton, GUILayout.Width(65f)))
            {
                if(AINameMap.aiNameMap == null){
                    Debug.Log("aiNameMap is not initialized");
                }
                else if (AINameMap.aiNameMap.Count == 0)
                {
                    Debug.Log("Nothing registered to aiNameMap. Reregistering AiNameMap.");
                    AINameMap.RegenerateNameMap();

                }
                else{
                    foreach (KeyValuePair<Guid, string> keyValue in AINameMap.aiNameMap)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(keyValue.Key.ToString("N"));
                        Debug.LogFormat("AIID:  {0}  | Guid:  {1}\nPath:  {2}", keyValue.Value, keyValue.Key.ToString("N"), path);


                        AIStorage aiStorage = AssetDatabase.LoadMainAssetAtPath(path) as AIStorage;

                        Debug.LogFormat("Path:  {0}\nAIStorage:  {1}", path, aiStorage);
                    }

                }
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Generate Name Map", EditorStyles.miniButton, GUILayout.Width(65f)))
            {
                AINameMapGenerator.WriteNameMapFile();
                Debug.Log(AINameMapHelper.AgentActionAI);
                Debug.Log(AINameMapHelper.AgentMoveAI);
                Debug.Log(AINameMapHelper.AgentScanAI);
            }


            serializedObject.ApplyModifiedProperties();
        }











        string ClientDiagnosticsInfo(int index)
        {
            string clientInfo = "";

            IUtilityAI utilityAI = taskNetwork.clients[index].ai;
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
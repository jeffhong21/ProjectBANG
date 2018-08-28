//namespace AtlasAI
//{
//    using System;
//    using System.IO;
//    using UnityEngine;
//    using UnityEditor;


//    /// <summary>
//    /// Used in TaskNetworkEditor to create new AIStorage
//    /// </summary>
//    public class AddClientWindow_TEMP : OptionsWindow<AddClientWindow_TEMP>
//    {
//        AddClientWindow_TEMP window;
//        const string filterType = "t:AIStorage";

//        protected string windowTitle = "Add Clients";
//        protected string defaultAiID = "NewUtilityAI";
//        protected string defaultAiName = "New Utility AI";
//        //  Name for the AI file.
//        string aiName { get; set; }

//        protected string defaultDemoAiID = "NewDemoMockAI";
//        protected string defaultDemoAiName = "NewDemoMockAI";


//        void AddAIAsset(AIStorage aiAsset)
//        {
//            //  Add asset and client to TaskNetwork
//            UtilityAIClient client = new UtilityAIClient(aiAsset.configuration, taskNetwork.contextProvider);
//            client.ai = aiAsset.configuration;
//            taskNetwork.clients.Add(client);
//            //taskNetwork.assets.Add(aiAsset);

//            //aiAsset.configuration.OnBeforeSerialize();
//            //aiAsset.configuration.OnAfterDeserialize();

//            EditorUtility.SetDirty(taskNetwork);
            
//        }


//        public override void Init(AddClientWindow_TEMP window, TaskNetworkComponent taskNetwork)
//        {
//            this.taskNetwork = taskNetwork;
//            this.window = window;
//            this.window.minSize = this.window.maxSize = new Vector2(windowMinSize, windowMaxSize);
//            this.window.titleContent = new GUIContent(windowTitle);

//            this.window.ShowUtility();
//        }


//        protected override void OnGUI()
//        {
//            //  Section for new name
//            CreateClientDrawer();

//            //  Al Clients
//            DrawWindowContents();

//            ////  Draw Create Demo AI's.
//            //EditorGUILayout.Space();
//            //DrawCreateDemoAIContents();
//        }


//        protected override void DrawWindowContents()
//        {
//            EditorGUILayout.Space();
//            using (new EditorGUILayout.VerticalScope())
//            {
//                foreach (var guid in AssetDatabase.FindAssets(filterType))
//                {
//                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
//                    GUIContent buttonLabel = new GUIContent(Path.GetFileNameWithoutExtension(assetPath));

//                    if (GUILayout.Button(buttonLabel, GUILayout.Height(18)))
//                    {
//                        AIStorage aiAsset = AssetDatabase.LoadMainAssetAtPath(assetPath) as AIStorage;

//                        //  Add asset and client to TaskNetwork
//                        AddAIAsset(aiAsset);
//                        // -----------------------------------

//                        //taskNetworkEditor.AddUtilityAIAsset(aiAsset);
//                        CloseWindow();
//                    }
//                }
//            }

//        }


//        //protected void DrawCreateDemoAIContents()
//        //{
//        //    //  Section for new name
//        //    CreateClientDrawer(true);

//        //    EditorGUILayout.Space();


//        //    if (GUILayout.Button("Create Scan AI"))
//        //    {
//        //        var utilityAIAsset = new AIStorage();
//        //        var aiAsset = utilityAIAsset.CreateAsset<ScanAIConfig>("DemoScanAI", "DemoScanAI", taskNetwork.selectAiAssetOnCreate);
//        //        //  Add asset and client to TaskNetwork
//        //        AddAIAsset(aiAsset);
//        //        CloseWindow();
//        //    }
//        //    if (GUILayout.Button("Create Move AI"))
//        //    {
//        //        var utilityAIAsset = new AIStorage();
//        //        var aiAsset = utilityAIAsset.CreateAsset<MoveAIConfig>("DemoMoveAI", "DemoMoveAI", taskNetwork.selectAiAssetOnCreate);
//        //        //  Add asset and client to TaskNetwork
//        //        AddAIAsset(aiAsset);
//        //        CloseWindow();
//        //    }

//        //}




//        protected override void CloseWindow()
//        {
//            window.Close();
//        }



//        void CreateClientDrawer(bool isDemoAI = false)
//        {
//            string _defaultAiID = isDemoAI == false ? defaultDemoAiID : defaultAiID;
//            string _defaultAiName = isDemoAI == false ? defaultDemoAiName : defaultAiName;
//            AIStorage utilityAIAsset;

//            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
//            {
//                EditorGUILayout.LabelField("New AI Name", Styles.TextCenterStyle);
//                using (new EditorGUILayout.HorizontalScope())
//                {
//                    GUILayout.Label("Name: ", GUILayout.Width(windowMinSize * 0.18f));
//                    aiName = GUILayout.TextField(aiName);
//                }
//                using (new EditorGUILayout.HorizontalScope())
//                {
//                    if (GUILayout.Button("Ok"))
//                    {
//                        //utilityAIAsset = isDemoAI == false ? new AIStorage() : new UtilityAIConfig();
//                        utilityAIAsset = new AIStorage();
//                        var aiAsset = utilityAIAsset.CreateAsset(String.IsNullOrEmpty(aiName) || String.IsNullOrWhiteSpace(aiName) ? _defaultAiID : aiName,
//                                                                 String.IsNullOrEmpty(aiName) || String.IsNullOrWhiteSpace(aiName) ? _defaultAiName : aiName,
//                                                                 taskNetwork.selectAiAssetOnCreate);
//                        //  Add asset and client to TaskNetwork
//                        AddAIAsset(aiAsset);
//                        // -----------------------------------

//                        //editor.AddUtilityAIAsset(aiAsset);
//                        CloseWindow();
//                    }
//                    if (GUILayout.Button("Cancel"))
//                    {
//                        CloseWindow();
//                    }
//                }
//            }
//        }









//    }
//}
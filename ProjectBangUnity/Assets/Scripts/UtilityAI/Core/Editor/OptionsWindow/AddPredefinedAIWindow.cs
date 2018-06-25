namespace UtilityAI
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using UnityEditor;



    /// <summary>
    ///  Adds a predefined aiConfig to a client.
    /// </summary>
    public class AddPredefinedAIWindow : EditorWindow
    {
        AddPredefinedAIWindow window;
        const string filterType = "t:AIStorage";
        protected TaskNetworkEditor editor { get; set; }
        protected TaskNetworkComponent taskNetwork { get; set; }

        protected int windowMinSize = 250;
        protected int windowMaxSize = 350;
        protected string windowTitle = "Create Predefined Client Window";




        List<Type> displayTypes;

        GUIStyle contentStyle;




        public void Init(AddPredefinedAIWindow window, TaskNetworkEditor editor)
        {
            displayTypes = TaskNetworkUtilities.GetAllOptions<UtilityAIAssetConfig>();
            displayTypes.Reverse();

            this.editor = editor;
            this.taskNetwork = editor.target as TaskNetworkComponent;
            this.window = window;
            this.window.minSize = this.window.maxSize = new Vector2(windowMinSize, windowMaxSize);
            this.window.titleContent = new GUIContent(windowTitle);

            this.window.ShowUtility();

            contentStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter
            };
        }


        protected virtual void OnGUI()
        {

            foreach (Type type in displayTypes)
            {
                GUIContent buttonLabel = new GUIContent(type.Name);
                if (GUILayout.Button(buttonLabel, contentStyle, GUILayout.Height(18)))
                {
                    editor.DoAddNew(new AIStorage[]{ AIStorage.CreateAsset(type.Name, type.Name, taskNetwork.selectAiAssetOnCreate) } , type);
                    CloseWindow();
                }
                GUILayout.Space(4);
            }

        }


        ////  AIStorage aiAsset is a premade aiAsset
        //void AddAIAsset(Type type)
        //{


        //    //  Create a new AIStorage instance.
        //    AIStorage aiStorage = new AIStorage();

        //    //  Initialize AIConfig so we can get the name of the predefined config.
        //    IUtilityAIConfig config = (IUtilityAIConfig)Activator.CreateInstance(type);
        //    //  Use the new instance to create a scriptableObject of aiAsset.
        //    AIStorage aiAsset = aiStorage.CreateAsset(config.name, config.name, taskNetwork.selectAiAssetOnCreate);

        //    //  Create a new UtilityAIClient
        //    UtilityAIClient client = new UtilityAIClient(aiAsset.configuration, taskNetwork.GetComponent<IContextProvider>());
        //    client.ai = aiAsset.configuration;      //  Add the UtilityAI to the UtilityAIClient.
        //    taskNetwork.clients.Add(client);        //  Add the client to the TaskNetwork.

        //    //  Configure the predefined settings.
        //    config.SetupAI(client.ai);

        //    ////  Remove the type from the list of predefined clients.
        //    //displayTypes.Remove(type);
        //    editor.DoAddNew(new AIStorage[] { aiAsset });
        //}



        protected void CloseWindow()
        {
            window.Close();
        }

    }











}
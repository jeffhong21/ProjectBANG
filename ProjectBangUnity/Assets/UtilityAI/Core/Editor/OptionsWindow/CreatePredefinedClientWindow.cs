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
    ///  A window to add predefined AI Clients.
    /// </summary>
    public class CreatePredefinedClientWindow : EditorWindow
    {
        CreatePredefinedClientWindow window;
        const string filterType = "t:UtilityAIAsset";
        protected TaskNetworkComponent taskNetwork { get; set; }

        protected int windowMinSize = 250;
        protected int windowMaxSize = 350;
        protected string windowTitle = "Create Predefined Client Window";




        List<Type> displayTypes;

        GUIStyle contentStyle;




        public void Init(CreatePredefinedClientWindow window, TaskNetworkComponent taskNetwork)
        {
            displayTypes = TaskNetworkUtilities.GetAllOptions<UtilityAIAssetConfig>();
            displayTypes.Reverse();

            this.taskNetwork = taskNetwork;
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
                    AddAIAsset(type);
                    CloseWindow();
                }
                GUILayout.Space(4);
            }

        }


        //  UtilityAIAsset aiAsset is a premade aiAsset
        void AddAIAsset(Type type)
        {
            //  Initialize AIConfig so we can get the name of the predefined config.
            IUtilityAIConfig config = (IUtilityAIConfig)Activator.CreateInstance(type);

            //  Create a new UtilityAIAsset instance.
            UtilityAIAsset utilityAIAsset = new UtilityAIAsset();
            //  Use the new instance to create a scriptableObject of aiAsset.
            UtilityAIAsset aiAsset = utilityAIAsset.CreateAsset(config.name, config.name, taskNetwork.selectAiAssetOnCreate);

            //  Add asset and client to TaskNetwork
            UtilityAIClient client = new UtilityAIClient(aiAsset.configuration, taskNetwork.GetComponent<IContextProvider>());
            client.ai = aiAsset.configuration;      //  Add the scriptableObject to the UtilityAIClient.
            taskNetwork.clients.Add(client);        //  Add the UtilityAIClient to the TaskNetwork.

            //  Configure the predefined settings.
            config.ConfigureAI(client.ai);

            ////  Remove the type from the list of predefined clients.
            //displayTypes.Remove(type);

            EditorUtility.SetDirty(taskNetwork);
        }



        protected void CloseWindow()
        {
            window.Close();
        }


    }











}
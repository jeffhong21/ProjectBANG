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
        //UtilityAIAsset asset;
        TaskNetworkComponent taskNetwork;
        UtilityAI ai;
        protected int windowMinSize = 250;
        protected int windowMaxSize = 350;
        protected string windowTitle = "Add Options | AI Object Selector";


        List<Type> displayTypes;
        string searchStr;



        GUIStyle contentStyle;



        //public void Init(AddPredefinedAIWindow window, TaskNetworkComponent taskNetwork, UtilityAIAsset asset)
        //{
        //    displayTypes = TaskNetworkUtilities.GetAllOptions<UtilityAIConfig>();

        //    windowTitle = string.Format("Add Predefined AI");

        //    this.window = window;
        //    this.window.minSize = this.window.maxSize = new Vector2(windowMinSize, windowMaxSize);
        //    this.window.titleContent = new GUIContent(windowTitle);
        //    this.window.ShowUtility();

        //    this.asset = asset;
        //    this.taskNetwork = taskNetwork;
        //    contentStyle = new GUIStyle(GUI.skin.button)
        //    {
        //        alignment = TextAnchor.MiddleCenter
        //    };
        //}

        public void Init(AddPredefinedAIWindow window, TaskNetworkComponent taskNetwork, UtilityAI ai)
        {
            displayTypes = TaskNetworkUtilities.GetAllOptions<UtilityAIAssetConfig>();

            windowTitle = string.Format("Add Predefined AI");

            this.window = window;
            this.window.minSize = this.window.maxSize = new Vector2(windowMinSize, windowMaxSize);
            this.window.titleContent = new GUIContent(windowTitle);
            this.window.ShowUtility();

            this.ai = ai;
            this.taskNetwork = taskNetwork;

            contentStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter
            };
        }



        protected virtual void OnGUI()
        {

            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                string newSearchStr = EditorGUILayout.TextField(searchStr, new GUIStyle("SearchTextField"), GUILayout.Height(EditorGUIUtility.singleLineHeight));
                if (GUILayout.Button(GUIContent.none, new GUIStyle("SearchCancelButton"), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    Debug.Log("TODO:  Clearing Search Field");
                }
            }
            EditorGUILayout.Space();


            foreach (Type type in displayTypes)
            {
                GUIContent buttonLabel = new GUIContent(type.Name);
                if (GUILayout.Button(buttonLabel, contentStyle, GUILayout.Height(18)))
                {
                    IUtilityAIConfig config = (IUtilityAIConfig)Activator.CreateInstance(type);
                    config.ConfigureAI(ai);
                    EditorUtility.SetDirty(taskNetwork);
                    //EditorUtility.SetDirty(asset);
                    CloseWindow();
                }
                GUILayout.Space(2);
            }

        }


        protected void CloseWindow()
        {
            window.Close();
        }


    }











}
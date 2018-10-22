﻿namespace AtlasAI
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(AIStorage)) ]
    public class UtilityAIAssetDrawer : Editor
    {

        AIStorage obj;

        private GUIStyle textStyle;

		public void OnEnable()
		{
            obj = target as AIStorage;

            textStyle = new GUIStyle();
            //textStyle.normal.textColor = Color.white;
            textStyle.richText = true;
		}


		public override void OnInspectorGUI()
		{
            //if (serializedObject.isEditingMultipleObjects)
            //{
            //    DrawDefaultInspector();
            //    return;
            //}
            DrawDefaultInspector();

            serializedObject.Update();


            GUILayout.Space(8);
            //string config = DebugEditorUtilities.SelectorConfig(obj.configuration.selector);
            //EditorGUILayout.LabelField("Selector");
            //EditorGUILayout.HelpBox(config, MessageType.Info);
            ////EditorGUILayout.LabelField(config, textStyle);

            //string rootConfig = DebugEditorUtilities.SelectorConfig(obj.configuration.rootSelector);
            //EditorGUILayout.LabelField("RootSelector");
            //EditorGUILayout.LabelField("Qualifers:  " + obj.configuration.rootSelector.qualifiers.Count);
            //EditorGUILayout.HelpBox(rootConfig, MessageType.Info);

            //EditorGUILayout.LabelField(rootConfig, textStyle);

            serializedObject.ApplyModifiedProperties();
		}
	}
}
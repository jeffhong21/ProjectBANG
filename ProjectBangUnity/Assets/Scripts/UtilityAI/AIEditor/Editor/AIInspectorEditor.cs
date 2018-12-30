namespace AtlasAI.AIEditor
{
    using System;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(AIUI))]
    public class AIInspectorEditor : Editor
    {
        //
        // Static Fields
        //
        public static AIInspectorEditor instance;

        //
        // Fields
        //
        private AIUI aiui;


        private void DrawAIUI(AIUI aIUI)
        {
            
        }


		public override void OnInspectorGUI()
		{
            aiui = (AIUI)serializedObject.targetObject;

            if(aiui.selectedNode != null)
            {
                if(aiui.currentSelector != null)
                {
                    EditorGUILayout.LabelField(aiui.currentSelector.name + " | UTILITY AI", EditorStyling.Skinned.inspectorTitle);
                    EditorGUILayout.TextField("Name: ", aiui.currentSelector.name );
                    EditorGUILayout.HelpBox(aiui.currentSelector.viewArea.ToString(), MessageType.None);
                }
                else if (aiui.currentQualifier != null)
                {
                    EditorGUILayout.LabelField(aiui.currentQualifier.name + " | UTILITY AI", EditorStyling.Skinned.inspectorTitle);
                    EditorGUILayout.TextField("Name: ", aiui.currentQualifier.name );
                }
                else{
                    EditorGUILayout.LabelField("UTILITY AI INSPECTOR EDITOR", EditorStyling.Skinned.inspectorTitle);
                }
            }
            else 
            {
                EditorGUILayout.LabelField("UTILITY AI INSPECTOR EDITOR", EditorStyling.Skinned.inspectorTitle);
                EditorGUILayout.HelpBox("This is an AI Editor Window.", MessageType.None);
            }




            EditorUtility.SetDirty(aiui);
            //serializedObject.ApplyModifiedProperties();
		}


	}
}

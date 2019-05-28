namespace CharacterController
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;



    [CustomEditor(typeof(CharacterAction), true)]
    public class CharacterActionEditor : Editor
    {
        private static readonly string[] m_DontIncude = { "m_Script" };

        private CharacterAction m_Action;
        private GUIContent m_ActionSettingsHeader;
        private GUIStyle m_HeaderStyle;


        private void OnEnable()
        {
            if (target == null) return;
            m_Action = (CharacterAction)target;
            m_Action.hideFlags = HideFlags.HideInInspector;

            if(m_ActionSettingsHeader == null){
                m_ActionSettingsHeader = new GUIContent()
                {
                    text = "-- " + m_Action.GetType().Name + " Settings --",
                    tooltip = "Settings for " + m_Action.GetType().Name
                };
            }
            if(m_HeaderStyle == null){
                m_HeaderStyle = new GUIStyle()
                {
                    font = new GUIStyle(EditorStyles.label).font,
                    fontStyle = FontStyle.Bold,
                    fontSize = 12,
                };
            }
        }



        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUI.enabled = false;
            InspectorUtility.PropertyField(serializedObject.FindProperty("m_Script"));
            GUI.enabled = true;
            EditorGUILayout.Space();

            InspectorUtility.PropertyField(serializedObject.FindProperty("m_TransitionDuration"));
            InspectorUtility.PropertyField(serializedObject.FindProperty("m_SpeedMultiplier"));

            InspectorUtility.PropertyField(serializedObject.FindProperty("m_StartType"));
            InspectorUtility.PropertyField(serializedObject.FindProperty("m_StopType"));

            if(m_Action.StartType == ActionStartType.ButtonDown || 
               m_Action.StartType == ActionStartType.DoublePress ||
               m_Action.StopType == ActionStopType.ButtonToggle ||
               m_Action.StopType == ActionStopType.ButtonUp
              )
            {
                EditorGUILayout.Space();
                SerializedProperty inputNames = serializedObject.FindProperty("m_InputNames");
                inputNames.isExpanded = true;
                EditorGUI.indentLevel++;
                if(inputNames.isExpanded){
                    if(inputNames.arraySize == 0){
                        inputNames.InsertArrayElementAtIndex(0);
                    }
                    else{
                        for (int index = 0; index < inputNames.arraySize; index++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            //inputNames.GetArrayElementAtIndex(index).stringValue = EditorGUILayout.TextField(inputNames.GetArrayElementAtIndex(index).stringValue);
                            EditorGUILayout.PropertyField(inputNames.GetArrayElementAtIndex(index), GUIContent.none);
                            if (index == inputNames.arraySize - 1){
                                if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(28))){
                                    inputNames.InsertArrayElementAtIndex(index);
                                }
                            }
                            if (inputNames.arraySize > 1){
                                if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(28))){
                                    inputNames.DeleteArrayElementAtIndex(index);
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }


            InspectorUtility.PropertyField(serializedObject.FindProperty("m_ApplyBuiltinRootMotion"));
            InspectorUtility.PropertyField(serializedObject.FindProperty("m_StartAudioClips"), true);
            InspectorUtility.PropertyField(serializedObject.FindProperty("m_StopAudioClips"), true);
            InspectorUtility.PropertyField(serializedObject.FindProperty("m_StartEffect"));
            InspectorUtility.PropertyField(serializedObject.FindProperty("m_EndEffect"));

            GUILayout.Space(12);
            InspectorUtility.LabelField(m_ActionSettingsHeader, 12, FontStyle.Bold);

            DrawPropertiesExcluding(serializedObject, m_DontIncude);




            serializedObject.ApplyModifiedProperties();
        }





    }

}


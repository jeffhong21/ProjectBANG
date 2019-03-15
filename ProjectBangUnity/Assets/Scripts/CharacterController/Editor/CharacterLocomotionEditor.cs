namespace CharacterController
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    [CustomEditor(typeof(CharacterLocomotion))]
    public class CharacterLocomotionEditor : Editor
    {
        private static readonly string[] m_DontIncude = new string[] { "m_Script", "m_Actions" };
        //private static readonly string[] m_DontIncude = new string[] { "m_Script" };
        //private static readonly string[] m_DontIncude = new string[] { "m_Actions" };

        CharacterLocomotion m_Controller;

        ReorderableList m_ActionsList;
        CharacterAction m_SelectedAction;
        Editor m_ActionEditor;

        float m_LineHeight;
        float m_LineHeightSpace;

        bool m_ShowMovementFoldout = true;
        bool m_ShowComponents;

        private SerializedProperty m_Script;
        private SerializedProperty m_Actions;
        private SerializedProperty m_UseRootMotion;
        private SerializedProperty m_RootMotionSpeedMultiplier;
        private SerializedProperty m_SpeedChangeMultiplier;
        private SerializedProperty m_RotationSpeed;
        private SerializedProperty m_AimRotationSpeed;
        private SerializedProperty m_AlignToGround;
        private SerializedProperty m_AlignToGroundDepthOffset;
        private SerializedProperty m_GroundSpeed;
        private SerializedProperty m_SkinWidth;
        private SerializedProperty m_SlopeLimit;

        private SerializedProperty m_MaxStepHeight;
        private SerializedProperty m_StepOffset;
        private SerializedProperty m_StepSpeed;
        private SerializedProperty m_Acceleration;




		private void OnEnable()
		{
            if (target == null) return;
            m_Controller = (CharacterLocomotion)target;

            m_LineHeight = EditorGUIUtility.singleLineHeight;
            m_LineHeightSpace = m_LineHeight + 10;


            //m_ShowComponents = serializedObject.FindProperty("m_ShowComponents").boolValue;

            m_Script = serializedObject.FindProperty("m_Script");
            m_Actions = serializedObject.FindProperty("m_Actions");
            m_UseRootMotion = serializedObject.FindProperty("m_UseRootMotion");
            m_RootMotionSpeedMultiplier = serializedObject.FindProperty("m_RootMotionSpeedMultiplier");
            m_SpeedChangeMultiplier = serializedObject.FindProperty("m_SpeedChangeMultiplier");
            m_RotationSpeed = serializedObject.FindProperty("m_RotationSpeed");
            m_AimRotationSpeed = serializedObject.FindProperty("m_AimRotationSpeed");
            m_AlignToGround = serializedObject.FindProperty("m_AlignToGround");
            m_AlignToGroundDepthOffset = serializedObject.FindProperty("m_AlignToGroundDepthOffset");
            m_GroundSpeed = serializedObject.FindProperty("m_GroundSpeed");
            m_SkinWidth = serializedObject.FindProperty("m_SkinWidth");
            m_SlopeLimit = serializedObject.FindProperty("m_SlopeLimit");

            m_MaxStepHeight = serializedObject.FindProperty("m_MaxStepHeight");
            m_StepOffset = serializedObject.FindProperty("m_StepOffset");
            m_StepSpeed = serializedObject.FindProperty("m_StepSpeed");
            m_Acceleration = serializedObject.FindProperty("m_Acceleration");

            m_ActionsList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_Actions"), true, true, true, true);
		}


		public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(12);
            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_Script);
            GUI.enabled = true;
            //m_ShowMovementFoldout = Foldout("Character Locomtion", m_ShowMovementFoldout);
            m_ShowMovementFoldout  = EditorGUILayout.Foldout(m_ShowMovementFoldout, "Character Locomtion");
            if(m_ShowMovementFoldout){
                EditorGUILayout.PropertyField(m_UseRootMotion);
                EditorGUILayout.PropertyField(m_RootMotionSpeedMultiplier);
                EditorGUILayout.PropertyField(m_SpeedChangeMultiplier);
                EditorGUILayout.PropertyField(m_RotationSpeed);
                EditorGUILayout.PropertyField(m_AimRotationSpeed);
                EditorGUILayout.PropertyField(m_AlignToGround);
                EditorGUILayout.PropertyField(m_AlignToGroundDepthOffset);
                EditorGUILayout.PropertyField(m_GroundSpeed);
                EditorGUILayout.PropertyField(m_SkinWidth);
                EditorGUILayout.PropertyField(m_SlopeLimit);

                EditorGUILayout.PropertyField(m_MaxStepHeight);
                EditorGUILayout.PropertyField(m_StepOffset);
                EditorGUILayout.PropertyField(m_StepSpeed);
                EditorGUILayout.PropertyField(m_Acceleration);
            }
            EditorGUILayout.Space();


            //m_Actions.isExpanded = EditorGUILayout.Foldout(m_Actions.isExpanded, m_Actions.displayName);
            //if (m_Actions.isExpanded) DrawReorderableList<CharacterAction>(m_ActionsList);

            DrawReorderableList(m_ActionsList);
            DrawActionInspector(m_SelectedAction);

            DrawPropertiesExcluding(serializedObject, m_DontIncude);

            serializedObject.ApplyModifiedProperties();
        }



        private void DrawReorderableList(ReorderableList list)
        {
            GUILayout.Space(12);
            //GUILayout.BeginVertical("box");
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                Rect elementRect = new Rect(rect.x, rect.y, rect.width, m_LineHeight);
                DrawListElement(elementRect, element, isActive);
            };

            list.drawHeaderCallback = (Rect rect) => {
                DrawListHeader(rect);
            };


            list.onSelectCallback = (ReorderableList l) => {
                SerializedProperty element = l.serializedProperty.GetArrayElementAtIndex(l.index);
                m_SelectedAction = (CharacterAction)element.objectReferenceValue;
                EditorUtility.SetDirty(m_SelectedAction);
            };


            list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
                var menu = new GenericMenu();
                var optionTypes = Assembly.GetAssembly(typeof(CharacterAction)).GetTypes()
                                          .Where(t => t.IsClass && t.IsSubclassOf(typeof(CharacterAction))).ToList();
                for (int i = 0; i < optionTypes.Count; i++){
                    var charAction = optionTypes[i];
                    menu.AddItem(new GUIContent(Path.GetFileNameWithoutExtension(charAction.Name)), false, () => AddCharacterAction(charAction));
                }
                menu.ShowAsContext();
            };

            list.onRemoveCallback = (ReorderableList l) => {
                RemoveCharacterAction(l.index);
            };


            list.DoLayoutList();
            //GUILayout.EndVertical();
            GUILayout.Space(12);
        }







        private void DrawListHeader(Rect rect)
        {
            Rect headerRect = rect;
            headerRect.x += 12;
            EditorGUI.LabelField(headerRect, m_Actions.displayName);

            //  Action state name.
            headerRect.x = rect.width * 0.48f;
            EditorGUI.LabelField(headerRect, "State Name");

            //  Action state name.
            headerRect.x = rect.width - 50f;
            EditorGUI.LabelField(headerRect, "ID");
        }


        private void DrawListElement(Rect elementRect, SerializedProperty element, bool isActive)
        {
            
            if(element.objectReferenceValue != null)
            {
                Rect rect = elementRect;
                rect.y += 2;
                SerializedObject elementObj = new SerializedObject(element.objectReferenceValue);
                SerializedProperty stateName = elementObj.FindProperty("m_StateName");
                SerializedProperty actionID = elementObj.FindProperty("m_ActionID");


                //  Action name.
                rect.width = elementRect.width * 0.40f;
                CharacterAction action = (CharacterAction)element.objectReferenceValue;
                //EditorGUI.LabelField(rect, action.GetType().Name);
                EditorGUI.LabelField(rect, isActive ?  action.GetType().Name + string.Format(" ({0})", "IsActive") : action.GetType().Name);


                //  Action state name.
                rect.x += elementRect.width * 0.40f;
                rect.width = elementRect.width * 0.36f;
                stateName.stringValue = EditorGUI.TextField(rect, stateName.stringValue);
                //  Action ID
                //rect.x = elementRect.width * 0.85f;
                rect.x = elementRect.width - 36;
                rect.width = 36;
                actionID.intValue = EditorGUI.IntField(rect, actionID.intValue);

                //  Toggle Enable
                rect.x = elementRect.width + 12;
                rect.width = 36; ;
                action.enabled = EditorGUI.Toggle(rect, action.enabled);

                elementObj.ApplyModifiedProperties();



                Event evt = Event.current;
                if(elementRect.Contains(evt.mousePosition)){
                    if(evt.button == 1 && evt.isMouse && evt.type == EventType.MouseUp){
                        
                        var menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Add"), false, () => TestContextMenu(action.GetType().Name));
                        menu.AddItem(new GUIContent("Remove"), false, () => TestContextMenu(action.GetType().Name));
                        menu.ShowAsContext();
                    }
                }
            }

        }


        private void TestContextMenu(string actionName)
        {
            Debug.LogFormat("Right clicked {0}.", actionName);
        }


        private void AddCharacterAction(Type type)
        {
            CharacterAction characterAction = (CharacterAction)m_Controller.gameObject.AddComponent(type);

            //m_ActionsList.serializedProperty.InsertArrayElementAtIndex(m_ActionsList.count);
            ////  You have to ApplyModifiedProperties after inserting a new array element otherwise the changes don't get reflected right away.
            //serializedObject.ApplyModifiedProperties();
            //m_ActionsList.serializedProperty.GetArrayElementAtIndex(m_ActionsList.count).objectReferenceValue = characterAction;
            //serializedObject.ApplyModifiedProperties();


            int index = m_ActionsList.count;
            SerializedProperty serializedList = m_ActionsList.serializedProperty;
            //  You have to ApplyModifiedProperties after inserting a new array element otherwise the changes don't get reflected right away.
            serializedList.InsertArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();

            SerializedProperty arrayElement = serializedList.GetArrayElementAtIndex(index);
            arrayElement.objectReferenceValue = characterAction;

            serializedObject.ApplyModifiedProperties();
        }


        private void RemoveCharacterAction(int index)
        {
            SerializedProperty serializedList = m_ActionsList.serializedProperty;
            //  Cache the Component
            SerializedProperty listElement = serializedList.GetArrayElementAtIndex(index);
            CharacterAction characterAction = (CharacterAction)listElement.objectReferenceValue;


            serializedList.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();


            DestroyImmediate(characterAction, true);
            serializedObject.Update();
        }




        private void DrawActionInspector(SerializedObject selectedObject)
        {
            if(selectedObject != null){
                GUILayout.Space(12);
                SerializedProperty propertyIterator = selectedObject.GetIterator();
                while (propertyIterator.NextVisible(true))
                {
                    //var propertyPath = propertyIterator.propertyPath;
                    //if (propertyPath.EndsWith("m_StateName")) continue;
                    EditorGUILayout.PropertyField(propertyIterator);
                }
                GUILayout.Space(12);
            }
        }


        private void DrawActionInspector(CharacterAction selectedObject)
        {
            if (selectedObject != null)
            {
                GUILayout.BeginVertical("box");

                GUILayout.Space(12);
                m_ActionEditor = CreateEditor(selectedObject);
                m_ActionEditor.DrawDefaultInspector();
                GUILayout.Space(12);

                GUILayout.EndVertical();
            }

        }
















        public bool Foldout(string title, bool display)
        {
            var style = new GUIStyle("ShurikenModuleTitle");
            style.font = new GUIStyle(EditorStyles.label).font;
            style.border = new RectOffset(15, 7, 4, 4);
            style.fixedHeight = 22;
            style.contentOffset = new Vector2 (20f, -2f);

            var rect = GUILayoutUtility.GetRect(16f, 22f, style);
            GUI.Box(rect, title, style);

            var e = Event.current;

            var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if (e.type == EventType.Repaint)
            {
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
            }

            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                display = !display;
                e.Use();
            }

            return display;
        }
    }

}


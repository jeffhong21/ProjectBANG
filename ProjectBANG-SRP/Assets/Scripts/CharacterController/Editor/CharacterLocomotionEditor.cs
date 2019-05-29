namespace CharacterController
{
    /*  ° TODO: When selecting the action outside of prefab edit mode, it resets the selected action to null.  (2019.1.4f1)
     *  
     *  
     *  
     *  
     *  
     * */
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
        private readonly string[] m_DontIncude = new string[] { "m_Script", "m_Actions" };
        //private static readonly string[] m_DontIncude = new string[] { "m_Script" };
        //private static readonly string[] m_DontIncude = new string[] { "m_Actions" };

        private readonly bool m_UseDefaultFoldout = false;


        private const string MotorFoldoutHeader = "Character Movement";
        private const string PhysicsFoldoutHeader = "Character Physics";
        private const string ActionsFoldoutHeader = "Actions List";
        private const string DebugHeader = "-- Debug --";



        CharacterLocomotion m_Controller;

        ReorderableList m_ActionsList;
        CharacterAction m_SelectedAction;
        Editor m_ActionEditor;


        private bool m_ShowCharMotorFoldout = true;
        private bool m_ShowCharPhysicsFoldout;
        private bool m_ShowActionListFoldout = true;
        private bool m_DebugFoldout = true;

        private GUIStyle m_DefaultActionTextStyle = new GUIStyle();
        private GUIStyle m_ActiveActionTextStyle = new GUIStyle();



        private float m_LineHeight;

        private SerializedProperty m_Script;
        private SerializedProperty m_Debug;

        private SerializedProperty m_Actions;
        private SerializedProperty m_UseRootMotion;
        private SerializedProperty m_RootMotionSpeedMultiplier;
        private SerializedProperty m_Acceleration;
        private SerializedProperty m_MovementSpeed;
        private SerializedProperty m_RotationSpeed;
        private SerializedProperty m_SlopeForceDown;

        private SerializedProperty m_Mass;
        private SerializedProperty m_SkinWidth;
        private SerializedProperty m_SlopeLimit;
        private SerializedProperty m_MaxStepHeight;


        private SerializedProperty m_StepOffset;
        private SerializedProperty m_StepSpeed;


        private SerializedProperty displayMovement;
        private SerializedProperty displayPhysics;
        private SerializedProperty displayActions;



		private void OnEnable()
		{
            if (target == null) return;
            m_Controller = (CharacterLocomotion)target;

            m_LineHeight = EditorGUIUtility.singleLineHeight;
            m_DefaultActionTextStyle.fontStyle = FontStyle.Normal;
            m_ActiveActionTextStyle.fontStyle = FontStyle.Bold;

            displayMovement = serializedObject.FindProperty("displayMovement");
            displayPhysics = serializedObject.FindProperty("displayPhysics");
            displayActions = serializedObject.FindProperty("displayActions");

            m_Script = serializedObject.FindProperty("m_Script");
            m_Debug = serializedObject.FindProperty("m_Debug");
            m_Actions = serializedObject.FindProperty("m_Actions");
            m_UseRootMotion = serializedObject.FindProperty("m_UseRootMotion");
            m_RootMotionSpeedMultiplier = serializedObject.FindProperty("m_RootMotionSpeedMultiplier");
            m_Acceleration = serializedObject.FindProperty("m_Acceleration");
            m_MovementSpeed = serializedObject.FindProperty("m_MovementSpeed");
            m_RotationSpeed = serializedObject.FindProperty("m_RotationSpeed");
            m_SlopeForceDown = serializedObject.FindProperty("m_SlopeForceDown");

            m_Mass = serializedObject.FindProperty("m_Mass");
            m_MaxStepHeight = serializedObject.FindProperty("m_MaxStepHeight");
            m_MaxStepHeight = serializedObject.FindProperty("m_MaxStepHeight");
            m_SkinWidth = serializedObject.FindProperty("m_SkinWidth");
            m_SlopeLimit = serializedObject.FindProperty("m_SlopeLimit");


            m_StepOffset = serializedObject.FindProperty("m_StepOffset");
            m_StepSpeed = serializedObject.FindProperty("m_StepSpeed");

            m_ActionsList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_Actions"), true, true, true, true);
		}


		public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(12);
            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_Script);
            GUI.enabled = true;


            EditorGUILayout.PropertyField(m_Debug);

            //  -----
            //  Character Movement
            //  -----
            EditorGUILayout.Space();
            displayMovement.boolValue = m_UseDefaultFoldout ? EditorGUILayout.Foldout(displayMovement.boolValue, MotorFoldoutHeader) : InspectorUtility.Foldout(displayMovement.boolValue, MotorFoldoutHeader);
            if(displayMovement.boolValue)
            {
                EditorGUILayout.PropertyField(m_UseRootMotion);
                //  Root motion related variables.
                //EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_RootMotionSpeedMultiplier);
                //EditorGUI.indentLevel--;
                //  Motion related variables.
                EditorGUILayout.PropertyField(m_MovementSpeed);
                EditorGUILayout.PropertyField(m_Acceleration);
                EditorGUILayout.PropertyField(m_RotationSpeed);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SlopeForceUp"));
                EditorGUILayout.PropertyField(m_SlopeForceDown);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_StopMovementThreshold"));

            }

            //  -----
            //  Character Physics
            //  -----
            EditorGUILayout.Space();
            displayPhysics.boolValue = m_UseDefaultFoldout ? EditorGUILayout.Foldout(displayPhysics.boolValue, PhysicsFoldoutHeader) : InspectorUtility.Foldout(displayPhysics.boolValue, PhysicsFoldoutHeader);
            if(displayPhysics.boolValue)
            {
                EditorGUILayout.PropertyField(m_Mass);
                EditorGUILayout.PropertyField(m_SkinWidth);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_GroundStickiness"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ExternalForceDamping"));
                EditorGUILayout.PropertyField(m_SlopeLimit);
                EditorGUILayout.PropertyField(m_MaxStepHeight);
                EditorGUILayout.PropertyField(m_StepOffset);
                EditorGUILayout.PropertyField(m_StepSpeed);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_GravityModifier"));


            }

            //  -----
            //  Character Actions
            //  -----
            EditorGUILayout.Space();
            displayActions.boolValue = m_UseDefaultFoldout ? EditorGUILayout.Foldout(displayActions.boolValue, ActionsFoldoutHeader) : InspectorUtility.Foldout(displayActions.boolValue, ActionsFoldoutHeader);
            if(displayActions.boolValue)
            {
                EditorGUILayout.Space();
                //  Active Action.
                EditorGUILayout.BeginHorizontal();
                InspectorUtility.LabelField(serializedObject.FindProperty("m_ActiveAction").displayName, 11, FontStyle.Normal);
                GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ActiveAction"), GUIContent.none);
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();

                InspectorUtility.LabelField("Selected Action: " + (m_SelectedAction == null ? "<None>" : m_SelectedAction.name));
                //  Draw Action Inspector.
                //EditorGUI.indentLevel++;
                EditorGUILayout.Space();

                //GUILayout.BeginVertical("box");
                DrawReorderableList(m_ActionsList);
                //GUILayout.EndVertical();

                //  Draw Selected Action Inspector.
                EditorGUI.indentLevel++;
                if (m_SelectedAction != null)
                {
                    GUILayout.BeginVertical("box");

                    GUILayout.Space(12);
                    m_ActionEditor = CreateEditor(m_SelectedAction);
                    //m_ActionEditor.DrawDefaultInspector();
                    m_ActionEditor.OnInspectorGUI();

                    GUILayout.Space(12);

                    GUILayout.EndVertical();
                }
                //EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }



            //  -----
            //  Debugging
            //  -----
            if(m_Debug.boolValue){
                EditorGUILayout.Space();
                m_DebugFoldout = m_UseDefaultFoldout ? EditorGUILayout.Foldout(m_DebugFoldout, DebugHeader) : InspectorUtility.Foldout(m_DebugFoldout, DebugHeader);
                if (m_DebugFoldout)
                {
                    DrawPropertiesExcluding(serializedObject, m_DontIncude);


                    EditorGUILayout.Space();
                    InspectorUtility.LabelField("-- Debug Settings--");
                    EditorGUI.indentLevel++;

                    InspectorUtility.PropertyField(serializedObject.FindProperty("m_DrawDebugLine"));

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }



        private void DrawReorderableList(ReorderableList list)
        {
            //GUILayout.Space(12);
            //GUILayout.BeginVertical("box");
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                DrawListElement(rect, element, isActive);


            };

            list.drawHeaderCallback = (Rect rect) => 
            {
                Rect headerRect = rect;
                headerRect.x += 12;
                EditorGUI.LabelField(headerRect, m_Actions.displayName);

                //  Action state name.
                //headerRect.x = rect.width * 0.465f;
                headerRect.x += rect.width * 0.40f;
                headerRect.width = rect.width * 0.36f;
                EditorGUI.LabelField(headerRect, "State Name");

                //  Action state name.
                headerRect.x = rect.width - 50f;
                EditorGUI.LabelField(headerRect, "ID");
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

            //Event evt = Event.current;
            //if (rect.Contains(evt.mousePosition))
            //{
            //    if (evt.button == 0 && evt.isMouse && evt.type == EventType.MouseDown)
            //    {
                    
            //    }
            //}

            list.DoLayoutList();
            //GUILayout.EndVertical();
            GUILayout.Space(12);
        }


        private void DrawListElement(Rect elementRect, SerializedProperty element, bool isSelected)
        {
            if(element.objectReferenceValue != null)
            {
                Rect rect = elementRect;
                rect.y += 2;
                rect.height = m_LineHeight;

                SerializedObject elementObj = new SerializedObject(element.objectReferenceValue);
                SerializedProperty stateName = elementObj.FindProperty("m_StateName");
                SerializedProperty isActive = elementObj.FindProperty("m_IsActive");
                SerializedProperty actionID = elementObj.FindProperty("m_ActionID");


                CharacterAction action = (CharacterAction)element.objectReferenceValue;
                //  Action name.
                rect.width = elementRect.width * 0.40f;
                if(action.IsActive){
                    EditorGUI.LabelField(rect, string.Format("{0} ({1})", action.GetType().Name, "Active"), m_ActiveActionTextStyle);
                }
                else{
                    EditorGUI.LabelField(rect, action.GetType().Name, m_DefaultActionTextStyle);
                }


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
                //isActive.boolValue = EditorGUI.Toggle(rect, isActive.boolValue);
                //isActive.boolValue = action.enabled;

                elementObj.ApplyModifiedProperties();



                Event evt = Event.current;
                if (elementRect.Contains(evt.mousePosition))
                {
                    if (evt.button == 1 && evt.isMouse && evt.type == EventType.MouseUp)
                    {
                        var menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Add"), false, () => TestContextMenu(action.GetType().Name));
                        menu.AddItem(new GUIContent("Remove"), false, () => TestContextMenu(action.GetType().Name));
                        menu.ShowAsContext();
                    }
                }
                //else {
                //    if (evt.button == 0 && evt.isMouse && evt.type == EventType.MouseUp)
                //    {
                //        if(action != null) action = null;
                //    }
                //}
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


            DestroyImmediate(characterAction, true);
            //serializedObject.Update();
            AssetDatabase.Refresh();

            serializedObject.ApplyModifiedProperties();
        }
















        //public bool Foldout(bool display, string title, int fontSize = 12)
        //{
        //    var style = new GUIStyle("ShurikenModuleTitle");
        //    style.font = new GUIStyle(EditorStyles.label).font;
        //    style.fontSize = fontSize;
        //    //style.fontStyle = FontStyle.Bold;
        //    style.border = new RectOffset(15, 7, 4, 4);
        //    style.fixedHeight = 22;
        //    style.contentOffset = new Vector2 (20f, -2f);

        //    var rect = GUILayoutUtility.GetRect(16f, 22f, style);
        //    GUI.Box(rect, title, style);

        //    var e = Event.current;

        //    var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
        //    if (e.type == EventType.Repaint)
        //    {
        //        EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
        //    }

        //    if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        //    {
        //        display = !display;
        //        e.Use();
        //    }

        //    return display;
        //}
    }

}


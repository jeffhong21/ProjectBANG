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
    public class CharacterLocomotionEditor : ccEditor.InspectorEditor
    {
        //private CharacterLocomotion script { get { return target as CharacterLocomotion; } }


        private readonly bool m_UseDefaultFoldout = true;


        private const string MotorFoldoutHeader = "Character Movement";
        private const string PhysicsFoldoutHeader = "Character Physics";
        private const string AnimationFoldoutHeader = "Character Animation";
        private const string ActionsFoldoutHeader = "Actions List";
        private const string DebugHeader = "Debug ";



        CharacterLocomotion m_Controller;

        ReorderableList m_ActionsList;
        CharacterAction m_SelectedAction;
        Editor m_ActionEditor;




        private GUIStyle m_DefaultActionTextStyle = new GUIStyle();
        private GUIStyle m_ActiveActionTextStyle = new GUIStyle();



        private float m_LineHeight;


        private SerializedProperty displayActions;


        protected override void OnEnable()
		{
            base.OnEnable();

            if (target == null) return;
            m_Controller = (CharacterLocomotion)target;

            m_LineHeight = EditorGUIUtility.singleLineHeight;
            m_DefaultActionTextStyle.fontStyle = FontStyle.Normal;
            m_ActiveActionTextStyle.fontStyle = FontStyle.Bold;

            displayActions = serializedObject.FindProperty("displayActions");
            m_ActionsList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_Actions"), true, true, true, true);
		}


		public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            serializedObject.Update();


            //  -----
            //  Character Actions
            //  -----

            displayActions.boolValue = EditorGUILayout.Foldout(displayActions.boolValue, ActionsFoldoutHeader);
            if(displayActions.boolValue)
            {
                ////  Active Action.
                //EditorGUILayout.BeginHorizontal();
                //InspectorUtility.LabelField(serializedObject.FindProperty("m_ActiveAction").displayName, 11, FontStyle.Normal);
                //GUI.enabled = false;
                //EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ActiveAction"), GUIContent.none);
                //GUI.enabled = true;
                //EditorGUILayout.EndHorizontal();

                InspectorUtility.LabelField("Selected Action: " + (m_SelectedAction == null ? "<None>" : m_SelectedAction.GetType().Name));
                ////  Draw Action Inspector.
                ////EditorGUI.indentLevel++;
                //EditorGUILayout.Space();

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

                EditorGUILayout.Space();
            }


            //  -----
            //  Debugging
            //  -----
            EditorGUILayout.Space();
            InspectorUtility.PropertyField(serializedObject.FindProperty("debugger"), true);



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
                EditorGUI.LabelField(headerRect, "Action Type");  // m_Actions.displayName

                //  Action state name.
                //headerRect.x = rect.width * 0.465f;
                headerRect.x += rect.width * 0.40f;
                headerRect.width = rect.width * 0.36f;
                EditorGUI.LabelField(headerRect, "Sub-State Machine");

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
                SerializedProperty priority = elementObj.FindProperty("m_Priority");
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
                rect.width = 36;
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
                        //ChangeStateName(stateName, action.GetType().Name);
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


        private void ChangeStateName( SerializedProperty property, string actionType)
        {
            Animator animator = m_Controller.GetComponent<Animator>();
            var states = AnimatorUtil.GetStateMachineChildren.GetChildren(animator, actionType);

            var menu = new GenericMenu();

            for (int i = 0; i < states.Count; i++) {
                var state = states[i];
                menu.AddItem(new GUIContent(state), false, () =>
                {
                    property.stringValue = state;
                    serializedObject.Update();
                });
            }
            menu.ShowAsContext();
        }

        private void TestContextMenu(string actionName)
        {
            //Debug.LogFormat("Right clicked {0}.", actionName);

            Animator animator = m_Controller.GetComponent<Animator>();
            if (animator == null) {
                Debug.Log("No Animator");
                    return;
            }

            var states = AnimatorUtil.GetStateMachineChildren.GetChildren(animator, actionName);
            string debugStateInfo = "";
            debugStateInfo += "<b>" + actionName + " </b>\n";

            for (int i = 0; i < states.Count; i++) {
                debugStateInfo += "<b> " + states[i] + " </b> \n";
            }
            Debug.Log(debugStateInfo);
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

            DestroyImmediate(characterAction, true);
            serializedObject.ApplyModifiedProperties();
            serializedList.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();


            //serializedObject.Update();
            AssetDatabase.Refresh();

            
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


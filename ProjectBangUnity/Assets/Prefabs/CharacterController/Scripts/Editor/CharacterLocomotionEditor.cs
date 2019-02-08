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
        CharacterLocomotion m_Controller;

        ReorderableList m_Actions;
        CharacterAction m_SelectedAction;
        Editor m_ActionEditor;

        float m_LineHeight;
        float m_LineHeightSpace;

        bool m_ShowMovementFoldout;
        bool m_ShowComponents;



        private SerializedProperty m_UseRootMotion;
        private SerializedProperty m_RootMotionSpeedMultiplier;
        private SerializedProperty m_RotationSpeed;
        private SerializedProperty m_AimRotationSpeed;
        private SerializedProperty m_AlignToGround;
        private SerializedProperty m_AlignToGroundDepthOffset;
        private SerializedProperty m_GroundSpeed;
        private SerializedProperty m_SkinWidth;
        private SerializedProperty m_SlopeLimit;





		private void OnEnable()
		{
            if (target == null) return;
            m_Controller = (CharacterLocomotion)target;

            m_LineHeight = EditorGUIUtility.singleLineHeight;
            m_LineHeightSpace = m_LineHeight + 10;

            m_Actions = new ReorderableList(serializedObject, serializedObject.FindProperty("m_Actions"), true, true, true, true);
            m_ShowComponents = serializedObject.FindProperty("m_ShowComponents").boolValue;



            m_UseRootMotion = serializedObject.FindProperty("m_UseRootMotion");
            m_RootMotionSpeedMultiplier = serializedObject.FindProperty("m_RootMotionSpeedMultiplier");
            m_RotationSpeed = serializedObject.FindProperty("m_RotationSpeed");
            m_AimRotationSpeed = serializedObject.FindProperty("m_AimRotationSpeed");
            m_AlignToGround = serializedObject.FindProperty("m_AlignToGround");
            m_AlignToGroundDepthOffset = serializedObject.FindProperty("m_AlignToGroundDepthOffset");
            m_GroundSpeed = serializedObject.FindProperty("m_GroundSpeed");
            m_SkinWidth = serializedObject.FindProperty("m_SkinWidth");
            m_SlopeLimit = serializedObject.FindProperty("m_SlopeLimit");
		}


		public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(12);
            m_ShowMovementFoldout = Foldout("Character Locomtion", m_ShowMovementFoldout);
            if(m_ShowMovementFoldout){
                EditorGUILayout.PropertyField(m_UseRootMotion);
                EditorGUILayout.PropertyField(m_RootMotionSpeedMultiplier);
                EditorGUILayout.PropertyField(m_RotationSpeed);
                serializedObject.ApplyModifiedProperties();
                EditorGUILayout.PropertyField(m_AimRotationSpeed);
                EditorGUILayout.PropertyField(m_AlignToGround);
                EditorGUILayout.PropertyField(m_AlignToGroundDepthOffset);
                EditorGUILayout.PropertyField(m_GroundSpeed);
                EditorGUILayout.PropertyField(m_SkinWidth);
                EditorGUILayout.PropertyField(m_SlopeLimit);
            }

            DrawDefaultInspector();


            DrawReorderableList<CharacterAction>(m_Actions, "Character Actions");
            DrawActionInspector(m_SelectedAction);


            m_ShowComponents = EditorGUILayout.ToggleLeft(new GUIContent("Show Action Components"), serializedObject.FindProperty("m_ShowComponents").boolValue);  // serializedObject.FindProperty("m_ShowComponents").boolValue
            for (int i = 0; i < m_Controller.CharActions.Length; i++)
            {
                if( m_ShowComponents == false && m_Controller.CharActions[i].hideFlags == HideFlags.None){
                    m_Controller.CharActions[i].hideFlags = HideFlags.HideInInspector;
                }
                    
                else if( m_ShowComponents && m_Controller.CharActions[i].hideFlags == HideFlags.HideInInspector){
                    m_Controller.CharActions[i].hideFlags = HideFlags.None;
                }
                    
            }

            serializedObject.ApplyModifiedProperties();
        }



        private void DrawReorderableList<T>(ReorderableList list, string header)
        {
            GUILayout.Space(12);
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2;
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                SerializedObject elementObj = new SerializedObject(element.objectReferenceValue);
                elementObj.Update();

                Rect elementRect = new Rect(rect.x, rect.y, rect.width, m_LineHeight);

                EditorGUI.LabelField(elementRect, m_Controller.CharActions[index].GetType().Name);

                elementRect.x = rect.width; // * 0.96f;
                //elementRect.width = rect.width * 0.04f;
                m_Controller.CharActions[index].enabled = EditorGUI.Toggle(elementRect, m_Controller.CharActions[index].enabled);

                elementObj.ApplyModifiedProperties();
            };

            list.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, header);
            };


            list.onSelectCallback = (ReorderableList l) => {
                SerializedProperty element = l.serializedProperty.GetArrayElementAtIndex(l.index);
                //m_SelectedAction = new SerializedObject(element.objectReferenceValue);
                m_SelectedAction = (CharacterAction)element.objectReferenceValue;
                EditorUtility.SetDirty(m_SelectedAction);
            };


            list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
                var menu = new GenericMenu();

                var optionTypes = Assembly.GetAssembly(typeof(CharacterAction)).GetTypes()
                                          .Where(t => t.IsClass && t.IsSubclassOf(typeof(CharacterAction))).ToList();

                for (int i = 0; i < optionTypes.Count; i++)
                {
                    var charAction = optionTypes[i];
                    menu.AddItem(new GUIContent(Path.GetFileNameWithoutExtension(charAction.Name)), false, () => AddCharacterAction(charAction));
                }

                menu.ShowAsContext();
            };

            list.onRemoveCallback = (ReorderableList l) => {

                int i = m_Actions.index;
                m_Controller.CharActions[i].hideFlags = HideFlags.HideInInspector;
                DestroyImmediate(m_Controller.CharActions[i]);
                ShrinkArray(m_Controller.CharActions, i);
            };


            list.DoLayoutList();
            GUILayout.Space(12);
        }






        private void UpdateSelectedElement<T>()
        {
            
        }


        private void AddCharacterAction(Type type)
        {
            CharacterAction characterAction = (CharacterAction)m_Controller.gameObject.AddComponent(type);

            m_Actions.serializedProperty.InsertArrayElementAtIndex(0);
            //  You have to ApplyModifiedProperties after inserting a new array element otherwise the changes don't get reflected right away.
            serializedObject.ApplyModifiedProperties();
            m_Actions.serializedProperty.GetArrayElementAtIndex(0).objectReferenceValue = characterAction;
            serializedObject.ApplyModifiedProperties();
        }


        private void RemoveCharacterAction(int idx)
        {
            m_Actions.serializedProperty.DeleteArrayElementAtIndex(idx);
            serializedObject.ApplyModifiedProperties();
            //m_Controller.CharActions = ShrinkArray(m_Controller.CharActions, idx);
        }


        //private void AddCharacterAction(object obj)
        //{
        //    string charAction = (string)obj;
        //    Type assetType = Type.GetType(charAction);
        //    CharacterAction characterAction = (CharacterAction)m_Controller.gameObject.AddComponent(assetType);

        //    //int index = m_Actions.serializedProperty.arraySize++;
        //    //m_Actions.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue = characterAction;

        //    m_Actions.serializedProperty.InsertArrayElementAtIndex(0);
        //    //  You have to ApplyModifiedProperties after inserting a new array element otherwise the changes don't get reflected right away.
        //    serializedObject.ApplyModifiedProperties();
        //    m_Actions.serializedProperty.GetArrayElementAtIndex(0).objectReferenceValue = characterAction;

        //    //serializedObject.ApplyModifiedProperties();
        //}


        private void DrawActionInspector(SerializedObject selectedObject)
        {
            if(selectedObject != null){
                GUILayout.Space(12);
                SerializedProperty propertyIterator = selectedObject.GetIterator();
                while (propertyIterator.NextVisible(true))
                {
                    EditorGUILayout.PropertyField(propertyIterator);
                }
                GUILayout.Space(12);
            }
        }

        private void DrawActionInspector(CharacterAction selectedObject)
        {
            if (selectedObject != null)
            {
                GUILayout.Space(12);
                m_ActionEditor = CreateEditor(selectedObject);
                m_ActionEditor.DrawDefaultInspector();
                GUILayout.Space(12);
            }
        }





        private T[] GrowArray<T>(T[] array, int increase)
        {
            T[] newArray = array;
            Array.Resize(ref newArray, array.Length + increase);
            return newArray;
        }

        private T[] ShrinkArray<T>(T[] array, int idx)
        {
            T[] newArray = new T[array.Length - 1];
            if (idx > 0)
                Array.Copy(array, 0, newArray, 0, idx);

            if (idx < array.Length - 1)
                Array.Copy(array, idx + 1, newArray, idx, array.Length - idx - 1);

            return newArray;
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


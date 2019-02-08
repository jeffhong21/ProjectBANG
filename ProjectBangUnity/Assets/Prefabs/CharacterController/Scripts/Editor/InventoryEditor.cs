//namespace CharacterController
//{
//    using UnityEngine;
//    using UnityEditor;
//    using UnityEditorInternal;



//    [CustomEditor(typeof(Inventory))]
//    public class InventoryEditor : Editor
//    {

//        Inventory m_Inventory;
//        ReorderableList m_DefaultLoadout;


//        private SerializedProperty m_InputName;
//        private SerializedProperty m_StartType;
//        private SerializedProperty m_StopType;
//        private SerializedProperty m_TransitionDuration;
//        private SerializedProperty m_SpeedMultiplier;



//        private void OnEnable()
//        {
//            if (target == null) return;
//            m_Inventory = (Inventory)target;

//            m_DefaultLoadout = new ReorderableList(serializedObject, serializedObject.FindProperty("m_DefaultLoadout"), true, true, true, true);
//        }


//        public override void OnInspectorGUI()
//        {
//            serializedObject.Update();




//            DrawDefaultInspector();



//            serializedObject.ApplyModifiedProperties();
//        }



//        private void DrawReorderableList<T>(ReorderableList list, string header)
//        {
//            GUILayout.Space(12);
//            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
//            {
//                rect.y += 2;
//                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
//                SerializedObject elementObj = new SerializedObject(element.objectReferenceValue);
//                elementObj.Update();

//                Rect elementRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

//                EditorGUI.LabelField(elementRect, m_Inventory.DefaultLoadout[index].GetType().Name);


//                elementObj.ApplyModifiedProperties();
//            };

//            list.drawHeaderCallback = (Rect rect) => {
//                EditorGUI.LabelField(rect, header);
//            };


//            list.onSelectCallback = (ReorderableList l) => {

//            };


//            list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {

//            };

//            list.onRemoveCallback = (ReorderableList l) => {

//            };


//            list.DoLayoutList();
//            GUILayout.Space(12);
//        }
//    }

//}


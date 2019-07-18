namespace CharacterController
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;
    using System;
    using System.IO;


    [CustomEditor(typeof(Inventory))]
    public class InventoryEditor : Editor
    {
        private static readonly string[] m_DontIncude = new string[] { "m_Script", "m_DefaultLoadout" };



        private Inventory m_Inventory;
        private ReorderableList m_DefaultLoadoutList;

        private SerializedProperty m_Script;
        private SerializedProperty m_DefaultLoadout;


        string itemTypeCount = "";

        private void OnEnable()
        {
            if (target == null) return;
            m_Inventory = (Inventory)target;

            m_Script = serializedObject.FindProperty("m_Script");
            m_DefaultLoadout = serializedObject.FindProperty("m_DefaultLoadout");

            m_DefaultLoadoutList = new ReorderableList(serializedObject, m_DefaultLoadout, true, true, true, true);

            //if(m_Inventory.ItemTypeCount != null)
            //{
            //    foreach (var item in m_Inventory.ItemTypeCount)
            //    {
            //        itemTypeCount += "<b>ItemType:</b> " + item.Key + " | Count:" + item.Value + "\n";
            //    }
            //}
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(12);
            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_Script);
            GUI.enabled = true;

            SerializedProperty m_ActiveItems = serializedObject.FindProperty("m_ActiveItems");


            GUI.enabled = itemTypeCount.Length > 0;
            m_ActiveItems.isExpanded = EditorGUILayout.Foldout(m_ActiveItems.isExpanded, m_ActiveItems.displayName);
            if (m_ActiveItems.isExpanded){
                EditorGUILayout.TextArea(itemTypeCount);
            }
            GUI.enabled = true;


            m_DefaultLoadout.isExpanded = EditorGUILayout.Foldout(m_DefaultLoadout.isExpanded, m_DefaultLoadout.displayName);
            if (m_DefaultLoadout.isExpanded) DrawReorderableList(m_DefaultLoadoutList);

            InspectorUtility.PropertyField(serializedObject.FindProperty("m_SlotCount"));
            EditorGUI.indentLevel++;
            InspectorUtility.PropertyField(serializedObject.FindProperty("m_LeftItemSlot"));
            InspectorUtility.PropertyField(serializedObject.FindProperty("m_RightItemSlot"));
            EditorGUI.indentLevel--;

            //DrawPropertiesExcluding(serializedObject, m_DontIncude);



            //DrawDefaultInspector();



            serializedObject.ApplyModifiedProperties();
        }



        private void DrawReorderableList(ReorderableList list)
        {

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2;
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                //SerializedObject elementObj = new SerializedObject(element.objectReferenceValue);
                //elementObj.Update();

                Rect elementRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);


                //EditorGUI.LabelField(rect, m_Inventory.DefaultLoadout[index].GetType().Name);
                DrawDefaultLoadoutElement(elementRect, element);

                //elementObj.ApplyModifiedProperties();
            };

            list.drawHeaderCallback = (Rect rect) =>
            {
                //m_DefaultLoadout.isExpanded = EditorGUI.ToggleLeft(rect, "ItemType", m_DefaultLoadout.isExpanded);
                rect.x += 12;
                EditorGUI.LabelField(rect, "Items");
                rect.x = rect.width - 36;
                EditorGUI.LabelField(rect, "Amount");

            };


            list.onSelectCallback = (ReorderableList l) =>
            {

            };


            list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
            {
                var menu = new GenericMenu();
                var itemGuids = AssetDatabase.FindAssets("t:ItemType");
                for (int i = 0; i < itemGuids.Length; i++)
                {
                    var itemPath = AssetDatabase.GUIDToAssetPath(itemGuids[i]);
                    menu.AddItem(new GUIContent(Path.GetFileNameWithoutExtension(itemPath)), false, () => AddItemToDefaultLoadout(itemPath));
                }
                menu.ShowAsContext();
            };

            list.onRemoveCallback = (ReorderableList l) =>
            {
                RemoveCharacterAction(l.index);
            };


            list.DoLayoutList();
        }


        private void AddItemToDefaultLoadout(string itemPath)
        {
            ItemType item = (ItemType)AssetDatabase.LoadAssetAtPath(itemPath, typeof(ItemType));

            int index = m_DefaultLoadoutList.count;

            SerializedProperty serializedList = m_DefaultLoadoutList.serializedProperty;
            //  You have to ApplyModifiedProperties after inserting a new array element otherwise the changes don't get reflected right away.
            serializedList.InsertArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();

            SerializedProperty arrayElement = serializedList.GetArrayElementAtIndex(index);
            SerializedProperty itemProperty = arrayElement.FindPropertyRelative("m_Item");
            itemProperty.objectReferenceValue = item;

            serializedObject.ApplyModifiedProperties();
        }

        private void RemoveCharacterAction(int index)
        {
            SerializedProperty serializedList = m_DefaultLoadoutList.serializedProperty;
            serializedList.DeleteArrayElementAtIndex(index);


            serializedObject.ApplyModifiedProperties();
            //m_Controller.CharActions = ShrinkArray(m_Controller.CharActions, idx);
        }


        private void DrawDefaultLoadoutElement(Rect elementRect, SerializedProperty element)
        {
            Rect rect = elementRect;
            int intFieldWidth = 36;
            //SerializedObject elementObj = new SerializedObject(m_Inventory.DefaultLoadout[index].ItemType);
            SerializedProperty m_Item = element.FindPropertyRelative("m_Item");
            SerializedProperty m_Amount = element.FindPropertyRelative("m_Amount");
            SerializedProperty m_Equip = element.FindPropertyRelative("m_Equip");
            ItemType itemType = (ItemType)m_Item.objectReferenceValue;

            //EditorGUI.LabelField(rect, m_Inventory.DefaultLoadout[index].ItemType.ItemAnimName);

            //  ItemType Scriptableobject
            rect.width = elementRect.width * 0.95f - intFieldWidth - 4;
            EditorGUI.ObjectField(rect, m_Item, GUIContent.none);
            ////  Toggle Enable
            //rect.x = elementRect.width + 12;
            //rect.width = intFieldWidth; ;
            //EditorGUI.PropertyField(rect, m_Equip, GUIContent.none);

            //  Amount field.
            rect.width = intFieldWidth;
            rect.x = elementRect.width - (rect.width * 0.5f);
            EditorGUI.PropertyField(rect, m_Amount, GUIContent.none);



        }




    }

}


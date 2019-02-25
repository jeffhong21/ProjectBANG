using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace ObjectPool
{
    [CustomEditor(typeof(ObjectPoolManager))]
    public class ObjectPoolManagerEditor : Editor
    {

        ObjectPoolManager m_ObjectPoolManager;
        ReorderableList m_PoolList;
        SerializedProperty m_Pools;

        SerializedProperty m_ID;
        SerializedProperty m_Prefab;
        SerializedProperty m_Count;
        SerializedProperty m_MaxCount;




        private void OnEnable()
        {
            if (target == null) return;
            m_ObjectPoolManager = (ObjectPoolManager)target;


            m_PoolList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_Pools"), true, true, false, false);

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUILayout.Space(12);



            //DrawReorderableList(m_PoolList);


            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }



        private void DrawReorderableList(ReorderableList list)
        {
            GUILayout.Space(12);
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2;
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                Rect elementRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

                DrawListElement(elementRect, element);


            };

            list.drawHeaderCallback = (Rect rect) => {
                //EditorGUI.LabelField(rect, header);
                rect.x += 12;
                EditorGUI.LabelField(rect, "Object Pool Manager");

            };




            list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
                var menu = new GenericMenu();

                menu.ShowAsContext();
            };

            list.onRemoveCallback = (ReorderableList l) => {

            };


            list.DoLayoutList();
            GUILayout.Space(12);
        }



        private void DrawListElement(Rect elementRect, SerializedProperty element)
        {
            Rect rect = elementRect;
            m_ID = element.FindPropertyRelative("m_ID");
            m_Prefab = element.FindPropertyRelative("m_Prefab");
            m_Count = element.FindPropertyRelative("m_Count");
            m_MaxCount = element.FindPropertyRelative("m_MaxCount");
            GameObject prefab = (GameObject)m_Prefab.objectReferenceValue;

            //EditorGUI.TextField(rect, "")
            rect.width = elementRect.width * 0.45f;
            EditorGUI.PropertyField(rect, m_ID, GUIContent.none);

            rect.width = elementRect.width * 0.5f;
            rect.x = elementRect.width - (rect.width * 0.90f);
            EditorGUI.ObjectField(rect, m_Prefab, GUIContent.none);
        }



    }
}


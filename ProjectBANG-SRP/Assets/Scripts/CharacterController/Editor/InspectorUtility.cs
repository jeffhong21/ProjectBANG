using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public static class InspectorUtility
{



    public static void DrawReorderableList(SerializedObject serializedObject, SerializedProperty property)
    {
        ReorderableList list = new ReorderableList(serializedObject, property, true, true, true, true)
        {
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = property.GetArrayElementAtIndex(index);
                rect.y += 1.0f;
                rect.x += 10.0f;
                rect.width -= 10.0f;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, 0.0f), element, true);
            },


            drawHeaderCallback = (Rect rect) =>
            {
                //rect.x += 12;
                EditorGUI.LabelField(rect, string.Format("{0}: {1}", property.displayName, property.arraySize), EditorStyles.label);
            },

            elementHeightCallback = (int index) =>
            {
                return EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(index)) + 4.0f;
            },

            onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
            {
                l.serializedProperty.InsertArrayElementAtIndex(l.count);
            },


            onRemoveCallback = (ReorderableList l) =>
            {
                l.serializedProperty.DeleteArrayElementAtIndex(l.count);
            }
        };

    }

    public static void DrawReorderableList(ReorderableList list)
    {
        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 1.0f;
            rect.x += 10.0f;
            rect.width -= 10.0f;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, 0.0f), element, true);
        };


        list.drawHeaderCallback = (Rect rect) =>
        {
                //rect.x += 12;
            EditorGUI.LabelField(rect, string.Format("{0}: {1}", list.serializedProperty.displayName, list.serializedProperty.arraySize), EditorStyles.label);
        };

        list.elementHeightCallback = (int index) =>
        {
            return EditorGUI.GetPropertyHeight(list.serializedProperty.GetArrayElementAtIndex(index)) + 4.0f;
        };

        list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
        {
            l.serializedProperty.InsertArrayElementAtIndex(l.count);
        };


        list.onRemoveCallback = (ReorderableList l) =>
        {
            l.serializedProperty.DeleteArrayElementAtIndex(l.count);
        };
    }
}

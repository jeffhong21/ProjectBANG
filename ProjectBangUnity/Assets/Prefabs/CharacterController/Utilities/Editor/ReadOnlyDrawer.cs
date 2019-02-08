namespace JH_Utils
{
    using UnityEditor;
    using UnityEngine;



    [CustomPropertyDrawer(typeof(DisplayOnlyAttribute))]
    public sealed class DisplayOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}
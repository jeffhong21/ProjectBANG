//using UnityEngine;
//using UnityEditor;


//[CustomPropertyDrawer(typeof(ObjectPool))]
//public class PoolPropertyDrawer : PropertyDrawer
//{
    
//    Rect rect;

//    SerializedProperty m_ID;
//    SerializedProperty m_Prefab;
//    SerializedProperty m_Count;
//    SerializedProperty m_MaxCount;


//    GUIContent m_idContent = new GUIContent("ID:", "ID of the pooled object.");


//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        EditorGUI.BeginProperty(position, label, property);

//        m_ID = property.FindPropertyRelative("m_ID");
//        m_Prefab = property.FindPropertyRelative("m_Prefab");
//        m_Count = property.FindPropertyRelative("m_Count");
//        m_MaxCount = property.FindPropertyRelative("m_MaxCount");

//        rect = position;
//        //EditorGUI.LabelField(rect, m_idContent);
//        //rect.x += position.width * 0.25f;
//        EditorGUI.PropertyField(rect, m_ID, GUIContent.none);
//        rect.x += position.width * 0.5f;
//        EditorGUI.ObjectField(rect, m_Prefab, GUIContent.none);






//        EditorGUI.EndProperty();

//    }


//    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//    {
//        return base.GetPropertyHeight(property, label);
//        //return (EditorGUIUtility.singleLineHeight + linePadding) * 3;
//    }



//}
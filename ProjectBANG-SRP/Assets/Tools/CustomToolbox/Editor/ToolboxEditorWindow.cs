using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;

public class ToolboxEditorWindow : EditorWindow
{
    private GUIContent m_title = new GUIContent("Toolbox");
    private Vector2 m_minSize = new Vector2(200, 630);




    [MenuItem("Tools/Toolbox", false, -100)]
    public static void ShowWindow()
    {
        var window = GetWindow<ToolboxEditorWindow>();
        window.minSize = window.m_minSize;
        window.titleContent = window.m_title;



        window.Show();
    }



    //protected void OnGUI()
    //{
    //    for (int index = 8; index <= 31; index++)
    //    {
    //        string layerName = InternalEditorUtility.SetSortingLayerLocked(index, true);
    //        EditorGUILayout.LabelField(layerName);
    //    }
    //}

}

using UnityEngine;
using UnityEditor;

public static class GizmosUtils
{

    public static void DrawString(string text, Vector3 worldPos, Color? colour = null)
    {
        Handles.BeginGUI();
        if (colour.HasValue) GUI.color = colour.Value;
        var view = SceneView.currentDrawingSceneView;
        Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
        Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));

        //GUI.Label(new Rect(screenPos.x, -screenPos.y + 4, size.x, size.y), text);
        GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);

        Handles.EndGUI();
    }


}

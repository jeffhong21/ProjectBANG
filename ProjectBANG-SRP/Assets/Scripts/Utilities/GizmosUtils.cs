using UnityEngine;
using UnityEditor;

public static class GizmosUtils
{
    static GUIStyle boldText = new GUIStyle()
    {
        fontStyle = FontStyle.Bold
    };

    public static void DrawString(string text, Vector3 worldPos, Color? color = null)
    {
        //Handles.BeginGUI();
        ////if (color.HasValue) GUI.color = color.Value;
        //if (color == null)
        //    GUI.color = Color.black;
        //else
        //    GUI.color = color.Value;
        
        //var view = SceneView.currentDrawingSceneView;
        //Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
        //Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));

        ////GUI.Label(new Rect(screenPos.x, -screenPos.y + 4, size.x, size.y), text);
        //GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text, boldText);

        //Handles.EndGUI();
    }


    public static void DrawMarker(Vector3 position, float size, Color color, float duration = 0, bool depthTest = true)
    {
        Vector3 line1PosA = position + Vector3.up * size * 0.5f;
        Vector3 line1PosB = position - Vector3.up * size * 0.5f;

        Vector3 line2PosA = position + Vector3.right * size * 0.5f;
        Vector3 line2PosB = position - Vector3.right * size * 0.5f;

        Vector3 line3PosA = position + Vector3.forward * size * 0.5f;
        Vector3 line3PosB = position - Vector3.forward * size * 0.5f;

        Debug.DrawLine(line1PosA, line1PosB, color, duration, depthTest);
        Debug.DrawLine(line2PosA, line2PosB, color, duration, depthTest);
        Debug.DrawLine(line3PosA, line3PosB, color, duration, depthTest);
    }
}

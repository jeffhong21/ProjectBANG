//namespace AtlasAI
//{
//    using UnityEngine;
//    using UnityEditor;

//    using Visualization;

//    public class DiagnosticsEditor : EditorWindow
//    {
//        string registeredVisualizers;


//        [MenuItem("AI TaskNetwork/Diagnostics")]
//        public static void Init()
//        {
//            var window = EditorWindow.GetWindow<DiagnosticsEditor>();
//            window.minSize = new Vector2(330f, 360f);
//            window.maxSize = new Vector2(600f, 4000f);
//            window.titleContent = new GUIContent("Diagnostics");
//            window.Show();
//        }




//        void OnGUI()
//        {
//            Rect rect = new Rect(0, 2, position.width, position.height);
//            GUILayout.BeginArea(rect);

//            GUILayout.Label("Registered Visualizers: \n" + registeredVisualizers);

//            if (GUILayout.Button("Refresh", EditorStyles.miniButton, GUILayout.Width(65f)))
//            {
//                registeredVisualizers = VisualizerManager.DebugLogRegisteredVisualziers(true);
//            }

//            GUILayout.EndArea();



//            if (GUI.changed) Repaint();
//        }

//		private void OnEnable()
//		{
//            registeredVisualizers = VisualizerManager.DebugLogRegisteredVisualziers(true);
//		}






//	}
//}



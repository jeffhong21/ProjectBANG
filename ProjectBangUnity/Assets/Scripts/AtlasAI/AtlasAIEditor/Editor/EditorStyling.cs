namespace AtlasAI
{
    using UnityEngine;
    using UnityEditor;



    public class EditorStyling
    {


        public static class Icons
        {
            public static Texture2D DeleteIcon;
            public static Texture2D ChangeIcon;
            public static Texture2D AddIcon;

            static Icons()
            {
                DeleteIcon = EditorGUIUtility.IconContent("Toolbar Minus").image as Texture2D;
                ChangeIcon = EditorGUIUtility.IconContent("preAudioLoopOff").image as Texture2D;
                AddIcon = EditorGUIUtility.IconContent("Toolbar Plus").image as Texture2D;
            }
        }



        public static class Styles
        {
            //  Node Styles.
            public static GUIStyle defaultNodeStyle;

            //  Text Styles.
            public static GUIStyle TextCenterStyle;
            public static GUIStyle DebugTextStyle;
            //  Node Style
            public static GUIStyle nodeStyle;
            public static GUIStyle selectedNodeStyle;

            static Styles()
            {

                defaultNodeStyle = new GUIStyle
                {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft,
                    contentOffset = new Vector2(25, -3)
                };


                TextCenterStyle = new GUIStyle
                {
                    alignment = TextAnchor.MiddleCenter
                };

                DebugTextStyle = new GUIStyle
                {
                    richText = true
                };


                nodeStyle = new GUIStyle("TL SelectionButton PreDropGlow")
                {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft,
                };

                selectedNodeStyle = new GUIStyle("TL SelectionButton PreDropGlow")
                {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft,
                };


            }
        }




    }
}



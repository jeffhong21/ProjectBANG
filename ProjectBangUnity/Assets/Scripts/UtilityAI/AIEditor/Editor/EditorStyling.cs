namespace AtlasAI.AIEditor
{
    using UnityEngine;
    using UnityEditor;



    public static class EditorStyling
    {
        private const int defaultFontSize = 11;
        private static readonly Color defaultTextColor = Color.black;
        private static readonly GUIStyle smallButtonBase = new GUIStyle()
        {

        };


        public static class Canvas
        {
            public static GUIStyle normalHeader
            {
                get;
                private set;
            }

            public static GUIStyle defaultNode
            {
                get;
                private set;
            }

            public static GUIStyle activeNode
            {
                get;
                private set;
            }



            public static void Init()
            {
                normalHeader = new GUIStyle()
                {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft,
                };
                defaultNode = new GUIStyle(GUI.skin.box)
                {
                    border = new RectOffset(12, 12, 12, 12),
                    normal = new GUIStyleState(){
                        background = EditorGUIUtility.Load("Textures/NE_Box.png") as Texture2D
                    }
                };
                activeNode = new GUIStyle("TL SelectionButton PreDropGlow")
                {
                    border = new RectOffset(12, 12, 12, 12),
                    normal = new GUIStyleState(){
                        background = EditorGUIUtility.Load("Textures/NE_Box.png") as Texture2D
                    }
                };
            }

            public static void SetTextures()
            {
                //defaultNode.normal.background = 
            }
        }


        public static class Skinned
        {
            public static GUIStyle AddButtonSmall
            {
                get;
                private set;
            }

            public static GUIStyle DeleteButtonSmall
            {
                get;
                private set;
            }

            public static GUIStyle ChangeButtonSmall
            {
                get;
                private set;
            }


            public static void Init()
            {
                AddButtonSmall = new GUIStyle(smallButtonBase)
                {
                    normal = new GUIStyleState(){
                        background = EditorGUIUtility.IconContent("Toolbar Plus").image as Texture2D
                    }
                };

                DeleteButtonSmall = new GUIStyle(smallButtonBase)
                {
                    normal = new GUIStyleState()
                    {
                        background = EditorGUIUtility.IconContent("Toolbar Minus").image as Texture2D
                    }
                };

                ChangeButtonSmall = new GUIStyle(smallButtonBase)
                {
                    normal = new GUIStyleState()
                    {
                        background = EditorGUIUtility.IconContent("preAudioLoopOff").image as Texture2D
                    }
                };
            }

        }









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



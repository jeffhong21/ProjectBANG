namespace CharacterController
{
    using UnityEngine;
    using System.Collections;



    public static class CharacterControllerUtility
    {
        private static int _guiFontSize = 16;
        private static GUIStyle _guiStyle;
        private static Color _debugTextColor = Color.white;
        private static Rect _animatorMonitorRect = new Rect(10, 10, 200, 20);
        private static Rect _characterControllerRect = new Rect(10, 50, 200, 20);



        public static Rect AnimatorMonitorRect
        {
            get
            {
                //_animatorMonitorRect.y = Screen.height * 0.1f;
                _animatorMonitorRect.width = Screen.width - _animatorMonitorRect.x;
                _animatorMonitorRect.height = UnityEditor.EditorGUIUtility.singleLineHeight * 5 + 4;
                return _animatorMonitorRect;
            }
        }

        public static Rect CharacterControllerRect
        {
            get
            {
                _characterControllerRect.y = AnimatorMonitorRect.y + AnimatorMonitorRect.height;
                _characterControllerRect.width = 250;
                if(Screen.currentResolution.width > 1920)
                    _characterControllerRect.width += _characterControllerRect.width * ScreenMultiplier;

                _characterControllerRect.height = Screen.height * 0.75f;
                return _characterControllerRect;
            }
        }



        public static float ScreenMultiplier {
            get {
                float defaultSize = 1920;
                return ((float)Screen.currentResolution.width - defaultSize) / defaultSize;
            }
        }



        public static int GuiFontSize
        {
            get {
                _guiFontSize += Mathf.RoundToInt(_guiFontSize * ScreenMultiplier);
                return _guiFontSize;
            }
            set
            {
                if (value < 11)
                    _guiFontSize = 11;
                else
                    _guiFontSize = value;
            }
        }

        public static Color DebugTextColor
        {
            get { return _debugTextColor; }
            set { _debugTextColor = value; }
        }

        public static GUIStyle GuiStyle
        {
            get
            {
                if (_guiStyle == null)
                {
                    _guiStyle = new GUIStyle()
                    {
                        font = new GUIStyle(UnityEditor.EditorStyles.label).font,
                        fontStyle = FontStyle.Normal,
                        fontSize = GuiFontSize,
                    };
                }
                return _guiStyle;
            }
        }

        //public static Rect CharacterControllerOnGUIRect()
        //{
        //    return 
        //}


    }

}

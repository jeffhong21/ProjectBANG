namespace AtlasAI
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;


    public abstract class SelectorWindow<T> : EditorWindow
    {
        //
        // Fields
        //
        private IEnumerable<T> _itemList;
        private bool _allowNoneSelection;
        private Action<T[]> _onSelect;


        GUIStyle contentStyle;

        //
        // Static Methods
        //
        protected static TWin GetWindow<TWin>(Vector2 screenPosition, string title) where TWin : EditorWindow
        {
            //  Initialize window
            TWin window = GetWindow<TWin>();
            window.position = new Rect(new Vector2(250, 350), screenPosition);
            window.titleContent = new GUIContent(title);
            return window;
        }



        //
        // Methods
        //
        private void OnEditorUpdate()
        {
            
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                if(_itemList != null)
                {
                    foreach (var item in _itemList)
                    {
                        var buttonLabel = new GUIContent(item.GetType().Name);
                        if (GUILayout.Button(buttonLabel, contentStyle, GUILayout.Height(18)))
                        {
                            //editor.DoAddNew(new AIStorage[] { AIStorage.CreateAsset(type.Name, type.Name, taskNetwork.selectAiAssetOnCreate) }, type);
                            SafeClose();
                        }
                    }
                }

            }
        }



        private void OnLostFocus()
        {
            
        }

        public void Preselect(Func<T, bool> predicate, string filter = null)
        {
            
        }

        protected void RenderInfo()
        {
            
        }

        private void SafeClose()
        {
            this.Close();
        }

        //protected void Show(IEnumerable<T> items, Func<T, GUIContent> itemRenderer, Func<T, string, bool> searchPredicate, bool allowNoneSelection, bool allowMultiSelect, Action<T[]> onSelect = null)
        protected void Show(IEnumerable<T> items, Func<T, GUIContent> itemRenderer, Func<T, string, bool> searchPredicate, 
                            bool allowNoneSelection, bool allowMultiSelect, Action<T[]> onSelect = null)
        {
            _itemList = items;
            _allowNoneSelection = allowMultiSelect;
            _onSelect = onSelect;


        }
    }



    public sealed class AISelectorWindow : SelectorWindow<AIStorage>
    {
        //
        // Fields
        //
        private GUIContent _listItemContent;
        private Action<AIStorage> _singleCallback;
        private Action<AIStorage[]> _multiCallback;



        //
        // Static Methods
        //
        public static AISelectorWindow Get(Vector2 screenPosition, Action<AIStorage> callback, bool allowNoneSelection = false)
        {
            var window = GetWindow<AISelectorWindow>(screenPosition, "AI Selector Window");
            window.Show(callback, allowNoneSelection);
            return window;
        }



        //
        // Methods
        //

        private void OnSelect(AIStorage[] items)
        {
            throw new NotImplementedException();
        }

        private GUIContent RenderListItem(AIStorage ai)
        {
            throw new NotImplementedException();
        }

        private void Show(Action<AIStorage> callback, bool allowNoneSelection)
        {
            _singleCallback = callback;
        }

    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using AtlasAI.Visualization;

namespace AtlasAI.AIEditor
{
    public class AIEditorWindow : EditorWindow
    {
        //
        // Static Fields
        //
        private static GameObject _lastSelectedGameObject;
        private static readonly Color _majorGridColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
        private static readonly Color _minorGridColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
        private static readonly Color _gridBackground = Color.gray;
        private static readonly Color _connectorLineColor;
        private static readonly Color _connectorLineActiveColor;
        private const float _minimumDragDelta = 25f;
        public static AIEditorWindow activeInstance;

        //
        // Fields
        //
        private GameObject _visualizedEntity;
        private GUIContent _title = new GUIContent("<DefaultTitle>");
        private GUIContent _label = new GUIContent("<DefaultLabel>");
        private DragData _drag;
        private AIUI _ui;

        //
        // Properties
        //
        private float _topPadding = 18;


        private float topPadding{
            get{
                return _topPadding + 2f;
            }
        }


        //
        // Static Methods
        //
        [MenuItem("UtilityAI/AI Editor Window")]
        public static AIEditorWindow AIEditor()
        {
            //  Initialize window
            activeInstance = GetWindow<AIEditorWindow>();
            activeInstance.minSize = new Vector2(400, 200);
            Texture2D _icon = EditorGUIUtility.Load(EditorGUIUtility.isProSkin ? "Textures/Icon_Dark.png" : "Textures/Icon_Light.png") as Texture2D;
            activeInstance._title.text = "AI Editor";
            activeInstance._title.image = _icon;
            activeInstance.titleContent = activeInstance._title;
            //  Show window
            activeInstance.Show();

            //  Initialize variables.
            activeInstance._drag = new DragData(activeInstance);
            activeInstance._ui = AIUI.Create("DefaultAI");


            return activeInstance;
        }




        //
        // Methods
        //
        private GUIContent DoLabel(string text){
            return new GUIContent(text);
        }

        private GUIContent DoLabel(string text, string tooltip){
            return new GUIContent(text, tooltip);
        }


		private void OnEnable()
		{
            //EditorStyling.Canvas.Init();
            //EditorStyling.Skinned.Init();

            activeInstance = this;
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;

            if(_drag == null) _drag = new DragData(activeInstance);
            if (_ui == null) _ui = AIUI.Create("DefaultAI");
		}


		private void OnDisable()
		{
            EditorApplication.update -= OnEditorUpdate;
		}

		private void OnDestroy()
		{
            EditorApplication.update -= OnEditorUpdate;
		}


		private void OnGUI()
        {
            DrawToolbar();
            DrawBackgroundGrid();

            DrawNodes(GetViewport());


            DrawUI();
            if (GUI.changed) Repaint();
        }


        private void DrawBackgroundGrid()
        {
            Color _oldColor = Handles.color;

            Handles.color = _majorGridColor;
            DrawGridLines(_ui.canvas.offset, position.width, position.height, 100);
            Handles.color = _minorGridColor;
            DrawGridLines(_ui.canvas.offset, position.width, position.height, 20);
            Handles.color = _oldColor;

            //Handles.color = _majorGridColor;
            //DrawGridLines(_drag.offset, position.width, position.height, 100);
            //Handles.color = _minorGridColor;
            //DrawGridLines(_drag.offset, position.width, position.height, 20);
            //Handles.color = _oldColor;
        }


        private void DrawGridLines(Vector2 offset, float width, float height, float size)
        {
            int widthDivs = Mathf.CeilToInt(width / size);
            int heightDivs = Mathf.CeilToInt(height / size);

            Handles.BeginGUI();

            offset += _drag.dragStart * _minimumDragDelta;
            Vector3 newOffset = new Vector3(offset.x % size, offset.y % size, 0);

            //  Draw lines for height.
            for (int i = 0; i < widthDivs; i++){
                Vector3 p1 = new Vector3(size * i, -size + topPadding + size, 0) ; // + newOffset;
                Vector3 p2 = new Vector3(size * i, position.height, 0f); // + newOffset;
                Handles.DrawLine(p1, p2);
            }
            for (int i = 0; i < heightDivs; i++){
                Vector3 p1 = new Vector3(-size, size * i, 0); // + newOffset;
                Vector3 p2 = new Vector3(position.width, size * i, 0f) ; // + newOffset;
                Handles.DrawLine(p1, p2);
            }
            Handles.EndGUI();
        }


        private void DrawToolbar()
        {
            GUIContent newButton = DoLabel("New", "Creates a new AI");
            GUIContent saveButton = DoLabel("Save", "Saves current AI");
            GUIContent loadButton = DoLabel("Load", "Loads a saved AI");
            GUIContent reloadButton = DoLabel("Reload", "Reloads an AI");

            Rect rect = new Rect(0, 2, position.width, topPadding);
            GUILayout.BeginArea(rect, EditorStyles.toolbar);
            using (new EditorGUILayout.HorizontalScope())
            {
                
                if(GUILayout.Button(newButton, EditorStyles.toolbarButton, GUILayout.Width(48)))
                {
                    Debug.Log(newButton.tooltip);
                }
                GUILayout.Space(5);
                if (GUILayout.Button(saveButton, EditorStyles.toolbarButton, GUILayout.Width(48)))
                {
                    Debug.Log(saveButton.tooltip);
                }
                GUILayout.Space(5);
                if (GUILayout.Button(loadButton, EditorStyles.toolbarButton, GUILayout.Width(48)))
                {
                    Debug.Log(loadButton.tooltip);
                }
                GUILayout.Space(5);
                if (GUILayout.Button(reloadButton, EditorStyles.toolbarButton, GUILayout.Width(48)))
                {
                    Debug.Log(reloadButton.tooltip);
                }

                //  Window Rect
                GUILayout.Space(5);
                GUILayout.FlexibleSpace();
                Rect windowRect = GetViewport();
                GUILayout.Label(string.Format("Position: ({0}, {1}) | Width: {2} | Height: {3}", 
                                              windowRect.x, windowRect.y, windowRect.width, windowRect.height));
                //  Number of Nodes.
                if(_ui != null){
                    GUILayout.Space(5);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(string.Format("Number of Nodes: {0} ", _ui.canvas.nodes.Count));
                }

                //  Mouse Position
                GUILayout.Space(5);
                GUILayout.FlexibleSpace();
                GUILayout.Label(string.Format("Mouse Position: ({0}, {1})", Event.current.mousePosition.x, Event.current.mousePosition.y));

                //  Mouse Delta
                GUILayout.Space(5);
                GUILayout.FlexibleSpace();
                GUILayout.Label(string.Format("Is Dragging: {0}", _drag.isDragging));
                GUILayout.Space(5);
            }
            GUILayout.EndArea();
        }


        private void DrawCompleteQualifier(Vector2 pos, float totalWidth, QualifierNode qualifierNode, SelectorLayout layout)
        {

        }

        private void DrawSelectorUI(SelectorNode selectorNode, SelectorLayout layout)
        {
            Rect nodeRect = selectorNode.viewArea;
            Vector2 pos = _drag.offset;
            nodeRect.position = new Vector2(nodeRect.x + pos.x, nodeRect.y + pos.y);


            // Create a headerRect out of the previous rect and draw it, marking the selected node as such by making the header bold
            var titleHeight = layout.titleHeight;
            Rect headerRect = new Rect(nodeRect.x, nodeRect.y, nodeRect.width, titleHeight);
            GUI.Box(headerRect, GUIContent.none, GUI.skin.box);
            GUI.Label(headerRect, string.Format(" Position: ({0}, {1}) | Is Selected : {2}", selectorNode.viewArea.x, selectorNode.viewArea.y, selectorNode.isSelected));
            //GUI.Label(headerRect, string.Format(" Position: ({0}, {1}) | Is Selected : {2}", nodeRect.x, nodeRect.y, selectorNode.isSelected));

            // Begin the body frame around the Node
            Rect bodyRect = new Rect(nodeRect.x, nodeRect.y + titleHeight, nodeRect.width, nodeRect.height - titleHeight);
            GUI.BeginGroup(bodyRect, GUI.skin.box);
            //GUI.BeginGroup(bodyRect, selectorNode.isSelected ? EditorStyling.Canvas.activeNode : EditorStyling.Canvas.defaultNode);
            bodyRect.position = Vector2.zero;
            GUILayout.BeginArea(bodyRect);

            GUI.changed = false;


            // End NodeGUI frame
            GUILayout.EndArea();
            GUI.EndGroup();
            //selectorNode.viewArea = nodeRect;

        }


        private void DrawUI()
        {
            Event evt = Event.current;

            // Process all node events.
            for (int i = _ui.canvas.nodes.Count - 1; i >= 0; i--)
            {
                var node = _ui.canvas.nodes[i];
                switch (evt.type)
                {
                    case EventType.MouseDown:
                        //  If left mouse button is clicked
                        if(evt.button == 0)
                        {
                            //TopLevelNode nodeAtPosition = _ui.canvas.NodeAtPosition(evt.mousePosition);
                            //if (node == nodeAtPosition)
                            if(node.isSelected){
                                //  That means node is selected.
                                _drag.StartNodeDrag(node, evt.delta);
                            }
                            else{
                                //_drag.EndDrag(evt.delta);
                            }
                            GUI.changed = true;
                        }
                        //  If right mouse button is clicked and the node is selected.
                        if (evt.button == 1 && node.isSelected && node.viewArea.Contains(evt.mousePosition))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Add Selector"), false, _TestAddSelector, evt.mousePosition);
                            menu.ShowAsContext();
                            evt.Use();
                        }
                        break;

                    case EventType.MouseUp:
                        //if (evt.button == 0 && _drag.isDragging){
                        //    _drag.EndDrag(evt.delta);
                        //}

                        break;

                    case EventType.MouseDrag:
                        if (evt.button == 0 && _drag.isDragging)
                        {
                            _drag.DoDrag(evt.delta);
                            //node.viewArea.position += evt.delta;
                            evt.Use();
                            GUI.changed = true;
                        }
                        break;
                }
            }


            //  Process canvas events.
            switch (evt.type)
            {
                case EventType.MouseDown:
                    if (evt.button == 0)
                    {
                        //ClearConnectionSelection();
                    }

                    if (evt.button == 1)
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Add Selector"), false, _TestAddSelector, evt.mousePosition);
                        menu.ShowAsContext();
                        evt.Use();
                    }
                    if(evt.button == 2)
                    {
                        
                    }
                    break;

                case EventType.MouseDrag:
                    if (evt.button == 0)
                    {
                        _ui.canvas.offset += _drag.offset;
                        var nodes = _ui.canvas.nodes;
                        if (nodes != null){
                            for (int i = 0; i < nodes.Count; i++){
                                nodes[i].viewArea.position += evt.delta;
                            }
                        }
                        GUI.changed = true;
                    }
                    break;
            }

        }


        private void DrawNodes(Rect viewPort)
        {
            GUI.BeginGroup(viewPort);

            var canvas = _ui.canvas;
            foreach (SelectorNode node in canvas.selectorNodes){
                DrawSelectorUI(node, new SelectorLayout(node, 20f));
            }

            GUI.EndGroup();
            //Repaint();
        }




        private Rect GetViewport()
        {
            Rect rect = position;
            rect.height = position.height - topPadding;
            rect.x = (position.width / 2) - (position.width / 2);
            rect.y = ((position.height / 2) - (position.height / 2)) + topPadding;
            return rect;
        }



        private void Load(string aiId, bool forceRefresh)
        {
            
        }


        private void OnAIExecute()
        {
            
        }

        private void OnEditorUpdate()
        {
            
        }


		private void OnSelectionChange()
		{
            _lastSelectedGameObject = _visualizedEntity;
            _visualizedEntity = Selection.activeGameObject;

            if (GUI.changed) Repaint();
		}


        private void ShowLoadMenu()
        {
            Debug.Log("Test menu");
        }

		private void UpdateVisualizedEntity(bool forceUpdate)
        {
            
        }


        private void _TestAddSelector(object mousePos)
        {
            Debug.Log((Vector2)mousePos);
            _ui.AddSelector((Vector2)mousePos, typeof(ScoreSelector));
        }



        private class DragData
        {
            public enum DragType
            {
                None,
                Node,
                Qualifier,
                Connector,
                Resize,
                MassSelect,
                Pan
            }

            //
            //  Fields
            //
            private AIEditorWindow _parent;
            private DragType _type;
            private TopLevelNode _node;
            private SelectorNode _selector;
            private QualifierNode _qualifier;
            private SelectorLayout _layout;
            private int _qualifierIndex;
            private Rect _startPositionAndSize;
            private Vector2 _offset;
            private Vector2 _dragStart;
            private Vector2 _dragLast;
            private Vector3 _dragAnchor;


            //
            //  Properties
            //
            public bool isDragging{
                get { return IsDraggingType(_type); }
            }

            public int qualifierIndex{
                get { return _qualifierIndex; }
            }

            public Vector2 dragStart{
                get {
                    _dragStart = Event.current.mousePosition;
                    return _dragStart; }
            }

            public Vector2 offset{
                get { return _offset; }
            }

            public Vector3 anchor{
                get { return _dragAnchor; }
            }


            //
            //  Constructor
            //
            public DragData(AIEditorWindow parent){
                _parent = parent;
            }


            //
            //  Methods
            //

            public bool IsDraggingType(DragType t)
            {
                switch(t){
                    case DragType.None:
                        return false;
                    case DragType.Node:
                        return true;
                    case DragType.Qualifier:
                        return true;
                    case DragType.Connector:
                        return true;
                    case DragType.Resize:
                        return false;
                    case DragType.MassSelect:
                        return false;
                    case DragType.Pan:
                        return true;
                }
                return false;
            }


            public void StartNodeDrag(TopLevelNode target, Vector2 mousePos)
            {
                _node = target;
                _type = DragType.Node;
                //_node.viewArea.position += mousePos;
            }

            private void DoNodeDrag(Vector2 mousePos)
            {
                //  Do something else with the node.
                _node.viewArea.position += mousePos;
            }


            public void DoDrag(Vector2 mousePos)
            {
                if(_type == DragType.Node){
                    DoNodeDrag(mousePos);
                }
                _offset += mousePos;
            }


            public void EndDrag(Vector2 mousePos)
            {
                _type = DragType.None;
            }
        }



    }
}
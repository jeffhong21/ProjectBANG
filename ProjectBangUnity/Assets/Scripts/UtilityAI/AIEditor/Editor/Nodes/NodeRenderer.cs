using System;
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


namespace AtlasAI.AIEditor
{
    public class NodeRenderer
    {
        private NodeSettings nodeSettings;
        private NodeLayout nodeLayout;

        ReorderableList listElement;
        bool useReorderableList = false;



        public NodeRenderer(NodeSettings settings)
        {
            nodeSettings = settings;
        }

        //
        // Methods
        //
        public void DrawNodes(SelectorNode selectorNode, NodeLayout layout)
        {
            DrawSelectorUI(selectorNode, layout);
        }


        private void DrawSelectorUI(SelectorNode selectorNode, NodeLayout layout)
        {
            selectorNode.RecalcHeight(nodeSettings);
            Rect nodeRect = selectorNode.viewArea;

            nodeRect.height = layout.titleHeight;
            GUI.Box(nodeRect, GUIContent.none, EditorStyling.Canvas.normalHeader);
            GUI.Label(nodeRect, selectorNode.friendlyName, selectorNode.isSelected ? EditorStyling.NodeStyles.nodeTitleActive : EditorStyling.NodeStyles.nodeTitle);

            nodeRect.y = nodeRect.y + layout.titleHeight;
            nodeRect.height = selectorNode.viewArea.height - layout.titleHeight;
            // Begin the body frame around the Node
            using (new GUI.GroupScope(nodeRect))
            {
                nodeRect.position = Vector2.zero;
                using (new GUILayout.AreaScope(nodeRect))
                {
                    GUI.Box(nodeRect, GUIContent.none, EditorStyling.Canvas.normalSelector);
                    GUI.changed = false;

                    Vector2 pos = new Vector2(nodeRect.x, nodeRect.y);
                    float elementHeight = nodeSettings.qualifierHeight + nodeSettings.actionHeight;

                    if (selectorNode.qualifierNodes.Count > 0)
                    {
                        for (int i = 0; i < selectorNode.qualifierNodes.Count; i++)
                        {
                            pos.y = i * elementHeight;
                            DrawCompleteQualifier(pos, nodeRect.width, selectorNode.qualifierNodes[i], layout);
                        }
                    }

                    pos.y = selectorNode.qualifierNodes.Count * elementHeight;  //  + settings.actionHeight
                    // Draw Node contents
                    DrawCompleteQualifier(pos, nodeRect.width, selectorNode.defaultQualifierNode, layout);
                    selectorNode.RecalcHeight(nodeSettings);
                }
            }
        }


        private void DrawCompleteQualifier(Vector2 pos, float totalWidth, QualifierNode qualifierNode, NodeLayout layout)
        {

            //Rect dragAreaRect = GetDragAreaLocal(pos.x, pos.y);
            Rect qualifierRect = layout.GetContentAreaLocal(pos.x, pos.y, nodeSettings.qualifierHeight);
            //Rect scoreAreaRect;
            //Rect toggleAreaRect;
            Rect actionRect = layout.GetContentAreaLocal(pos.x, pos.y + nodeSettings.actionHeight, nodeSettings.actionHeight);
            Rect anchorRect = layout.GetAnchorAreaLocal(pos.x, pos.y);


            GUI.Box(qualifierRect, GUIContent.none, EditorStyling.Canvas.normalQualifier);
            GUI.Label(qualifierRect, qualifierNode.friendlyName, EditorStyling.NodeStyles.normalBoxText);

            GUI.Box(actionRect, GUIContent.none, EditorStyling.Canvas.normalAction);
            GUI.Label(actionRect, "Action Name", EditorStyling.NodeStyles.normalBoxText);
            GUI.Label(anchorRect, EditorStyling.ConnectorOpen);


        }












        private void DrawListElement(SerializedObject so, Rect pos)
        {
            listElement = new ReorderableList(so, so.FindProperty("qualifierNodes"), true, false, false, false);

            //listElement.onReorderCallback += new ReorderableList.ReorderCallbackDelegate()
            listElement.elementHeight = nodeSettings.qualifierHeight;
            listElement.showDefaultBackground = false;
            listElement.headerHeight = 0;
            listElement.footerHeight = 0;


            listElement.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = listElement.serializedProperty.GetArrayElementAtIndex(index);
                SerializedProperty elementName = element.FindPropertyRelative("friendlyName");
                GUI.Box(rect, GUIContent.none, EditorStyling.Canvas.normalQualifier);
                //GUI.Label(rect, elementName.stringValue, EditorStyling.NodeStyles.normalBoxText);
            };


            listElement.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                //SerializedProperty list_element = list.serializedProperty.GetArrayElementAtIndex(index);
            };


            listElement.onSelectCallback = (ReorderableList l) =>
            {

            };


            listElement.DoList(pos);
            so.ApplyModifiedProperties();
        }



        private void DrawListElement<T>(IList list, Rect pos)
        {
            listElement = new ReorderableList(list, typeof(T), true, false, false, false);

            //listElement.onReorderCallback += new ReorderableList.ReorderCallbackDelegate()
            listElement.elementHeight = nodeSettings.qualifierHeight;
            listElement.showDefaultBackground = false;
            listElement.headerHeight = 0;
            listElement.footerHeight = 0;


            listElement.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                GUI.Box(rect, GUIContent.none, EditorStyling.Canvas.normalQualifier);
                GUI.Label(rect, listElement.list[index].ToString(), EditorStyling.NodeStyles.normalBoxText);
            };


            listElement.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                //SerializedProperty list_element = list.serializedProperty.GetArrayElementAtIndex(index);
            };


            listElement.onSelectCallback = (ReorderableList l) =>
            {

            };


            listElement.DoList(pos);
        }








    }
}
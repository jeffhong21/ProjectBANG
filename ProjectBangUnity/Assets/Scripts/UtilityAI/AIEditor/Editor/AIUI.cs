using System;
using System.Collections.Generic;
using UnityEngine;

using AtlasAI.Visualization;

namespace AtlasAI.AIEditor
{
    public class AIUI
    {
        public string name;
        public AICanvas canvas;
        private List<TopLevelNode> _selectedNodes;
        private AIStorage _aiStorage;
        private IUtilityAI _visualizedAI;
        private IUtilityAI _ai;

        //
        // Properties
        //
        public IUtilityAI ai{
            get { return _ai; }
        }

        public UtilityAIVisualizer visualizedAI{
            get { return _visualizedAI as UtilityAIVisualizer; }
        }

        public Selector rootSelector{
            get { return ai.rootSelector; }
        }


        public INode selectedNode{
            get;
            set;
        }

        public List<TopLevelNode> selectedNodes{
            get { return _selectedNodes; }
        }

        public bool isDirty{
            get;
            set;
        }




        //
        // Constructors
        //
        private AIUI()
        {
            _selectedNodes = new List<TopLevelNode>();
            canvas = new AICanvas();

        }

        //
        // Static Methods
        //
        public static AIUI Create(string name)
        {
            AIUI aiui= new AIUI();
            aiui.InitNew(name);
            return aiui;
        }

        public static AIUI Load(string aiId, bool refreshState)
        {
            AIUI aiui = Create(aiId);
            //AINameMapGenerator.WriteNameMapFile();
            var field = typeof(AINameMapHelper).GetField(aiId, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            Guid id = (Guid)field.GetValue(null);
            aiui._ai = AIManager.GetAI(id);

            //aiui._aiStorage = new AIStorage();
            Debug.LogFormat("Loading AIUI.  UtilityAI is {0} |", id);
            return aiui;
        }


        //
        // Methods
        //
        public void InitAI()
        {
            
        }

        private void InitNew(string name)
        {
            this.name = name;
            //_ai = new UtilityAI(name);
            //_visualizedAI = new UtilityAIVisualizer(_ai);
        }



        public void ShowVisualizedAI(UtilityAIVisualizer ai)
        {
            throw new NotImplementedException();
        }



        public SelectorNode AddSelector(Vector2 position, Type selectorType)
        {
            Rect rect = new Rect(position.x, position.y, 250, 100);
            SelectorNode node = SelectorNode.Create(selectorType, this, rect);
            //  Add node to the canvas.
            canvas.nodes.Add(node);

            return node;
        }

        public QualifierNode AddQualifier(Type qualiferType, SelectorNode parent)
        {
            QualifierNode node = AddQualifier(qualiferType);
            node.parent = parent;
            return node;
        }

        public QualifierNode AddQualifier(QualifierNode qn, SelectorNode parent){
            qn.parent = parent;
            return qn;
        }

        public QualifierNode AddQualifier(Type qualiferType){
            return QualifierNode.Create(qualiferType);
        }

        public ActionNode SetAction(ActionNode an, QualifierNode parent)
        {
            throw new NotImplementedException();
        }

        public ActionNode SetAction(Type actionType, QualifierNode parent)
        {
            throw new NotImplementedException();
        }

        public ActionNode SetAction(Type actionType)
        {
            throw new NotImplementedException();
        }





        public void ReplaceAction(ActionNode an, IAction replacement)
        {
            throw new NotImplementedException();
        }

        public void ReplaceDefaultQualifier(QualifierNode newDefault, SelectorNode parent)
        {
            throw new NotImplementedException();
        }

        public void ReplaceQualifier(QualifierNode qn, IQualifier replacement)
        {
            throw new NotImplementedException();
        }

        public void ReplaceSelector(SelectorNode sn, Selector replacement)
        {
            throw new NotImplementedException();
        }

        public void RemoveAction(ActionNode an)
        {
            throw new NotImplementedException();
        }

        public void RemoveQualifier(QualifierNode qn)
        {
            throw new NotImplementedException();
        }

        public bool RemoveSelected()
        {
            throw new NotImplementedException();
        }

        public bool RemoveSelector(SelectorNode sn)
        {
            throw new NotImplementedException();
        }

        public bool RemoveNode()
        {
            throw new NotImplementedException();
        }



        public void SetRoot(Selector newRoot)
        {
            throw new NotImplementedException();
        }


        public bool Delete()
        {
            throw new NotImplementedException();
        }

        private bool LoadFrom(AIStorage data, bool refreshState)
        {
            throw new NotImplementedException();
        }

        public void PingAsset()
        {
            throw new NotImplementedException();
        }

        public void RefreshState()
        {
            throw new NotImplementedException();
        }

        public void Save(string newName)
        {
            throw new NotImplementedException();
        }

        public void Select(SelectorNode sn, QualifierNode qn, ActionNode an)
        {
            throw new NotImplementedException();
        }


    }
}

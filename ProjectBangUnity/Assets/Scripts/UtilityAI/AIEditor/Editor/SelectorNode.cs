using System;
using System.Collections.Generic;
using UnityEngine;


namespace AtlasAI.AIEditor
{

    public class SelectorNode : TopLevelNode
    {
        //
        // Fields
        //
        private Selector _selector;
        private Type _selectorType;
        public List<QualifierNode> qualifierNodes;
        public QualifierNode defaultQualifierNode;

        //
        // Properties
        //
        public IEnumerable<QualifierNode> AllQualifierNodes{
            get{
                foreach (var node in qualifierNodes){
                    yield return node;
                }
            }
        }

        public string friendlyName{
            get { return _selectorType.Name; }
        }

        public bool isRoot{
            get{
                if (parent.rootSelector == _selector) return true;
                return false;
            }
        }

        public Selector selector{
            get { return _selector; }
            set { _selector = value; }
        }


        //
        // Constructors
        //
        private SelectorNode(){
        }

        private SelectorNode(Rect viewArea){
            this.viewArea = viewArea;
            qualifierNodes = new List<QualifierNode>();
        }

        //
        // Static Methods
        //
        public static SelectorNode Create(Type selectorType, AIUI parent, Rect viewArea){
            var node = new SelectorNode(viewArea);
            node.name = selectorType.ToString();
            node.parent = parent;

            return node;
        }


        //
        // Methods
        //
        public override string ToString(){
            return string.Format("SelectorNode");
        }
    }
}

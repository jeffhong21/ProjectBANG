using System;
using System.Collections.Generic;
using UnityEngine;


namespace AtlasAI.AIEditor
{

    public class QualifierNode : INode
    {
        //
        // Fields
        //
        private IQualifier _qualifier;
        private Type _qualifierType;
        private bool _expanded;



        //
        // Properties
        //
        public SelectorNode parent
        {
            get;
            set;
        }

        AIUI INode.parentUI{
            get{
                if (parent == null) return null;
                return parent.parentUI;
            }
        }

        public IQualifier qualifier
        {
            get { return _qualifier; }
            set { _qualifier = value; }
        }

        public ActionNode actionNode{
            get;
            set;
        }

        public string description{
            get;
            set;
        }

        public string friendlyDescription{
            get;
        }

        public string friendlyName{
            get { return qualifier.GetType().Name; }
        }

        public bool isDefault{
            get{
                if (parent == null) return false;
                return parent.defaultQualifierNode == this;
            }
        }

        public bool isExpanded{
            get { return _expanded; }
            set { _expanded = value; }
        }

        public bool isHighScorer{
            get;
        }


        public string name{
            get;
            set;
        }


        //
        // Constructors
        //
        public QualifierNode(){
            
        }


        //
        // Static Methods
        //
        public static QualifierNode Create(Type qualifierType){
            QualifierNode node = new QualifierNode();

            //  TODO: Need to ge tthe correct qualifier.  

            //  TODO: Need to set the selector node..  
            node.name = qualifierType.Name;


            return node;
        }


        public override string ToString(){
            return string.Format("QualifierNode");
        }
    }
}

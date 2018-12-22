using System;
using UnityEngine;

namespace AtlasAI.AIEditor
{

    public abstract class TopLevelNode : INode
    {
        //
        // Fields
        //
        public Rect viewArea;
        public AIUI parent;

        //
        // Properties
        //
        public string description{
            get;
            set;
        }

        public bool isSelected{
            get{
                if(viewArea.Contains(Event.current.mousePosition)){
                    return true;
                }
                return false;
            }
        }

        public string name{
            get;
            set;
        }

        public AIUI parentUI{
            get{ return parent;}
        }



        //
        // Constructors
        //
        public TopLevelNode(){
            
        }

        public TopLevelNode(Rect viewArea){
            this.viewArea = viewArea;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;


namespace AtlasAI.AIEditor
{
    public class AICanvas
    {
        //
        // Fields
        //
        public Vector2 offset;
        public float zoom;
        public List<TopLevelNode> nodes;


        //
        // Properties
        //
        public IEnumerable<SelectorNode> selectorNodes{
            get{
                foreach (var node in nodes){
                    if(node is SelectorNode)
                        yield return node as SelectorNode;
                }
            }
        }


        //
        // Constructors
        //
        public AICanvas()
        {
            nodes = new List<TopLevelNode>();
        }


        //
        // Methods
        //
        public TopLevelNode NodeAtPosition(Vector2 position)
        {
            for (int i = 0; i < nodes.Count; i++){
                if(nodes[i].viewArea.Contains(position)){
                    //nodes[i].isSelected = true;
                    return nodes[i];
                }
            }
            return null;
        }
    }
}

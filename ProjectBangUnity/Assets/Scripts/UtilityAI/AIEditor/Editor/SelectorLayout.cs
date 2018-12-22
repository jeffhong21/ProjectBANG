using System;
using UnityEngine;

namespace AtlasAI.AIEditor
{

    public class SelectorLayout : ViewLayout
    {
        //
        // Fields
        //
        private SelectorNode _selectorView;
        private Rect _dragArea;
        private Rect _contentArea;
        private Rect _scoreArea;
        private Rect _toggleArea;
        private Rect _anchorArea;

        //
        // Properties
        //
        public SelectorNode selectorView{
            get { return _selectorView; }
        }


        public SelectorLayout(TopLevelNode node, float windowTop) : base(node, windowTop)
        {
        }
    }
}

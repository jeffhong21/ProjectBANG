using System;
using UnityEngine;

namespace AtlasAI.AIEditor
{

    public class ViewLayout
    {
        protected Rect _viewRect;
        private Rect _leftResizeArea;
        private Rect _rightResizeArea;

        //
        // Properties
        //
        public Rect leftResizeArea{
            get { return _leftResizeArea; }
        }

        public Rect rightResizeArea{
            get { return _rightResizeArea; }
        }

        public float titleHeight{
            get;
        }

        public Rect viewRect{
            get { return _viewRect; }
        }

        //
        // Constructors
        //
        public ViewLayout(TopLevelNode node, float windowTop)
        {
            _viewRect = node.viewArea;
            titleHeight = windowTop;
        }

        //
        // Methods
        //
        public Rect GetIconArea(Rect parentArea)
        {
            throw new NotImplementedException();   
        }

        public bool InResizeArea(Vector2 position)
        {
            throw new NotImplementedException();   
        }

        public bool InTitleArea(Vector2 position)
        {
            throw new NotImplementedException();   
        }

    }
}

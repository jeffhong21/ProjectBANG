namespace UtilityAI.Visualization
{
    using System;
    using System.Collections.Generic;

    public abstract class CustomGizmoGUIVisualizerComponent<T, TData> : CustomVisualizerComponent<T, TData> where T : class
    {
        //
        // Fields
        //
        public bool drawGizmos;

        public bool drawGUI;

        //
        // Constructors
        //
        protected CustomGizmoGUIVisualizerComponent()
        {

        }

        //
        // Methods
        //

        private void OnDrawGizmos()
        {
            if(_data != null && drawGizmos)  // && VisualizerManager.isVisualizing
            {
                foreach (IAIContext key in _data.Keys)
                {
                    DrawGizmoData(key);
                }
            }

        }



        private void OnGUI()
        {
            if (_data != null && drawGUI)  // && VisualizerManager.isVisualizing
            {
                foreach (IAIContext key in _data.Keys)
                {
                    DrawGUIData(key);
                }
            }
        }

        private void DrawGizmoData(IAIContext context)
        {
            DrawGizmos(_data[context]);
        }


        private void DrawGUIData(IAIContext context)
        {
            DrawGUI(_data[context]);
        }


        protected abstract void DrawGizmos(TData data);

        protected abstract void DrawGUI(TData data);
    }
}
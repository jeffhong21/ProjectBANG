namespace UtilityAI.Visualization
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Custom gizmo GUIV isualizer component.
    /// </summary>
    /// <typeparam name="T">The type that this visualizer visualizes.</typeparam>
    /// <typeparam name = "TData" > The type of the data to be visualized.</typeparam>
    public abstract class CustomGizmoGUIVisualizer<T, TData> : MonoBehaviour
    {
        public bool drawGUI = true;
        public bool drawGizmo = true;
        protected T data;

        //[SerializeField, HideInInspector]
        [SerializeField]
        protected IContextProvider contextProvider;


        private void OnEnable(){
            if (contextProvider == null)
                contextProvider = gameObject.GetComponent<IContextProvider>();
        }



		protected virtual void OnGUI(){
            DrawGUI(data);
		}

        protected virtual void OnDrawGizmos(){
            DrawGizmos(data);
		}


        protected abstract void DrawGUI(T data);

        protected abstract void DrawGizmos(T data);


    }








}

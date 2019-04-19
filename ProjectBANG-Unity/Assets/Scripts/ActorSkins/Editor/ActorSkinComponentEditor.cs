using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ActorSkins
{
    [CustomEditor(typeof(ActorSkinComponent))]
    public class ActorSkinComponentEditor : Editor
    {
        ActorSkinComponent _target;

        private void OnEnable()
        {
            _target = target as ActorSkinComponent;
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //GUILayout.Space(12);
            //if (GUILayout.Button("Load Accessories"))
            //{
            //    _target.LoadAccessories();
            //}

        }
    }
}


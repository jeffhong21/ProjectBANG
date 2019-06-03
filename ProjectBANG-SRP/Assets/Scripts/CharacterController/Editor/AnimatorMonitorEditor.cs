namespace CharacterController
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections;

    [CustomEditor(typeof(AnimatorMonitor))]
    public class AnimatorMonitorEditor : Editor
    {

        private AnimatorMonitor m_AnimatorMonitor;




        private void OnEnable()
        {
            if (target == null) return;
            m_AnimatorMonitor = (AnimatorMonitor)target;
        }



        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if(GUILayout.Button("Register Animator States"))
            {
                m_AnimatorMonitor.GetAllStateIDs(true);
            }
        }


    }

}

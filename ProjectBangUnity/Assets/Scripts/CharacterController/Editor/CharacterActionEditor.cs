namespace CharacterController
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;



    [CustomEditor(typeof(CharacterAction))]
    public class CharacterActionEditor : Editor
    {
        private static readonly string[] m_DontIncude = new string[] { "m_Script" };

        private CharacterAction m_Action;




        private void OnEnable()
        {
            if (target == null) return;
            m_Action = (CharacterAction)target;


        }


        public override void OnInspectorGUI()
        {
            m_Action.hideFlags = HideFlags.HideInInspector;
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, m_DontIncude);




            serializedObject.ApplyModifiedProperties();
        }
    }

}


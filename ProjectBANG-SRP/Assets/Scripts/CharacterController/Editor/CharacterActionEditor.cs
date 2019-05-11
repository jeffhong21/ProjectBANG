namespace CharacterController
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;



    [CustomEditor(typeof(CharacterAction), true)]
    public class CharacterActionEditor : Editor
    {
        private static readonly string[] m_DontIncude = new string[] { "m_Script" };

        private CharacterAction m_Action;


        //ReorderableList m_InputNamesList;



        private void OnEnable()
        {
            if (target == null) return;
            m_Action = (CharacterAction)target;
            m_Action.hideFlags = HideFlags.HideInInspector;

            //m_InputNamesList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_Integers"), true, true, true, true);
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();


            //InspectorUtility.DrawReorderableList(m_InputNamesList);
            DrawPropertiesExcluding(serializedObject, m_DontIncude);




            serializedObject.ApplyModifiedProperties();
        }
    }

}


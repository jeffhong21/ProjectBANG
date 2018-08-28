namespace Bang
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(CharacterSkin))]
    public class CharacterSkinEditor : Editor
    {

        CharacterSkin _target;


        private void OnEnable()
        {
            _target = target as CharacterSkin;
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(12);
            if (GUILayout.Button("Generate Random Character"))
            {
                Debug.Log(" TODO: Generating random character.");
                //_target.LoadCharacter();
            }

            //GUILayout.Space(12);
            //if (GUILayout.Button("Pull Assets from Resource Manager"))
            //{
            //    _target.GetDataFromResourceManager(_target.buildingType);
            //}
        }


    }
}



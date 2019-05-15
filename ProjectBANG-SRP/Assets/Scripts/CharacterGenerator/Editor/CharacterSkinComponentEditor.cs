namespace CharacterSkins
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(CharacterSkinComponent))]
    public class CharacterSkinComponentEditor : Editor
    {
        
        CharacterSkinComponent _target;

        private void OnEnable()
        {
            _target = target as CharacterSkinComponent;
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(12);
            if (GUILayout.Button("Load Character Skin"))
            {
                _target.LoadCharacterSkin();
            }

        }
    }

}


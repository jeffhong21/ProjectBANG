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
            serializedObject.Update();
            base.OnInspectorGUI();

            GUILayout.Space(12);
            if (GUILayout.Button("Load Character Skin")){
                _target.LoadCharacter();
                serializedObject.ApplyModifiedProperties();
            }

            GUILayout.Space(8);
            if(_target.SkinData){
                if(_target.SkinData.SkinTextureSet){
                    if (_target.SkinData.SkinTextureSet.textures.Length > 0){
                        for (int i = 0; i < _target.SkinData.SkinTextureSet.textures.Length; i++){
                            if (GUILayout.Button("Set Texture " + i))
                            {
                                _target.SkinData.SetMaterialTexture(i);
                                serializedObject.ApplyModifiedProperties();
                            }
                        }
                    }
                }
            }

        }
    }

}


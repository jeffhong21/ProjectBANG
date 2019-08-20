namespace CharacterSkins
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "TextureSetData", menuName = "Character Controller/Character Skins/Set Data/Texture Set Data")]
    public class TextureSetData : ScriptableObject, ISetData
    {
        public string setId;
        [Header("-- Texture Data --")]
        public Texture2D[] textures = new Texture2D[0];


        public string SetId{
            get { return setId; }
        }
    }

}

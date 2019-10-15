using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorGenerator
{
    public class TextureSet : ScriptableObject
    {
        [SerializeField]
        private string m_textureSet;

        public Texture2D[] textures = new Texture2D[0];


    }

}

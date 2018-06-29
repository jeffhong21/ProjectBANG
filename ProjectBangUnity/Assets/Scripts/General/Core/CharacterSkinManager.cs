namespace Bang
{
    using UnityEngine;
    using System;


    public class CharacterSkinManager : SingletonMonoBehaviour<CharacterSkinManager>
    {
        [Serializable]
        public class CharacterSkin
        {
            public string id;
            public CharacterSkinTypes skinType;
            public SkinnedMeshRenderer mesh;

        }


        //
        //  Fields
        //
        public CharacterSkin[] skins;
        public Texture2D[] textures;



        //
        //  Methods
        //
        protected override void Awake()
        {
            base.Awake();


            DontDestroyOnLoad(gameObject);
        }
    }
}




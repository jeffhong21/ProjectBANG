namespace CharacterSkins
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    public interface ISetData
    {
        string SetId { get;}
    }


    [CreateAssetMenu(fileName = "SkinSetData", menuName = "Character Skins/Set Data/Skin Set Data")]
    public class SkinSetData : ScriptableObject, ISetData
    {
        [SerializeField]
        protected string setId;
        [Header("-- Mesh Data --")]
        [SerializeField]
        protected Mesh characterMesh;
        [SerializeField]
        protected Mesh hairMesh;
        [SerializeField]
        protected Mesh hatHairMesh;
        [Header("-- Texture Data --")]
        [SerializeField]
        protected TextureSetData textureSetData;
        [SerializeField]
        protected Material skinMaterial;


        public string SetId{
            get { return setId; }
        }

        public Mesh CharacterMesh{
            get { return characterMesh; }
        }

        public Mesh HairMesh{
            get { return hairMesh; }
        }

        public Mesh HatHairMesh{
            get { return hatHairMesh; }
        }

        public TextureSetData SkinTextureSet{
            get { return textureSetData; }
        }

        public Material SkinMaterial{
            get { return skinMaterial; }
        }

    }



}

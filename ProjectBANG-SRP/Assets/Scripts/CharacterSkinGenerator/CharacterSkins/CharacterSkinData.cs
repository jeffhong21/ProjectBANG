namespace CharacterSkins
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    public interface ISetData
    {
        string SetId { get;}
    }


    [CreateAssetMenu(fileName = "SkinSetData", menuName = "Character Skins/Skin Set Data")]
    public class CharacterSkinData : ScriptableObject, ISetData
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
        [SerializeField]
        protected Mesh facialHairMesh;
        [Header("-- Texture Data --")]
        [SerializeField]
        protected TextureSetData textureSetData;
        [SerializeField]
        protected Material skinMaterial;
        [SerializeField]
        protected string shaderProperty = "_MainTex";


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




        public void SetMaterialTexture(int index, string nameID = "_BaseColorRGBOutlineWidthA")
        {
            if(textureSetData.textures.Length > index){
                skinMaterial.SetTexture(nameID, textureSetData.textures[index]);
            }
                
        }
    }



}

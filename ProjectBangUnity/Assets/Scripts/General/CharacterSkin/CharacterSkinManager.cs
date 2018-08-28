namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = "CharacterSkinManager", menuName = "ProjectBang/CharacterSkinManager")]
    public class CharacterSkinManager : ScriptableObject
    {
        
        //
        //  Fields
        //
        public CharacterSkinMesh[] skins;
        public CharacterSkinTexture[] textures;

        private Dictionary<CharacterMeshTypes, SkinnedMeshRenderer> characterSkinMesh = new Dictionary<CharacterMeshTypes, SkinnedMeshRenderer>();
        private Dictionary<CharacterTextureTypes, Texture2D> characterSkinTexture = new Dictionary<CharacterTextureTypes, Texture2D>();



        public void Reload(int index)
        {
            characterSkinMesh.Clear();
            for (int i = 0; i < skins.Length; i ++)
            {
                characterSkinMesh.Add(skins[i].skinType, skins[i].mesh);
            }

            characterSkinTexture.Clear();
            for (int i = 0; i < textures.Length; i++)
            {
                characterSkinTexture.Add(textures[i].textureType, textures[i].texture[index]);
            }
        }


        public void LoadCharacter(SkinnedMeshRenderer meshRenderer, CharacterMeshTypes meshType, CharacterTextureTypes textureType, int textureIndex)
        {
            Reload(textureIndex);

            if(characterSkinMesh.ContainsKey(meshType))
            {
                meshRenderer.sharedMesh = characterSkinMesh[meshType].sharedMesh;
            }

            if (characterSkinTexture.ContainsKey(textureType))
            {
                meshRenderer.sharedMaterial.SetTexture("_MainTex", characterSkinTexture[textureType]);
            }
        }


        public void LoadCharacter(SkinnedMeshRenderer meshRenderer, CharacterMeshTypes meshType, CharacterTextureTypes textureType)
        {
            characterSkinMesh.Clear();
            for (int i = 0; i < skins.Length; i++)
            {
                characterSkinMesh.Add(skins[i].skinType, skins[i].mesh);
            }

            characterSkinTexture.Clear();
            for (int i = 0; i < textures.Length; i++)
            {
                characterSkinTexture.Add(textures[i].textureType, textures[i].texture[0]);
            }

            if (characterSkinMesh.ContainsKey(meshType))
            {
                meshRenderer.sharedMesh = characterSkinMesh[meshType].sharedMesh;
            }

            if (characterSkinTexture.ContainsKey(textureType))
            {
                meshRenderer.sharedMaterial.SetTexture("_MainTex", characterSkinTexture[textureType]);
            }
        }


    }



    [Serializable]
    public class CharacterSkinMesh
    {
        public string id;
        public CharacterMeshTypes skinType;
        public SkinnedMeshRenderer mesh;
    }


    [Serializable]
    public class CharacterSkinTexture
    {
        public string id;
        public CharacterTextureTypes textureType;
        public Texture2D[] texture;
    }
}




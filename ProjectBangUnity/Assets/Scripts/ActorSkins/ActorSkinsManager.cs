using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace ActorSkins
{
    [CreateAssetMenu(fileName = "ActorSkinManager", menuName = "ActorSkins/Manager")]
    public class ActorSkinsManager : ScriptableObject
    {
        public static string path = Path.GetDirectoryName("Assets/Scripts/ActorSkins/Resources/Skins/");

        public SkinMesh[] skins = new SkinMesh[0];

        public Texture2D[] textures = new Texture2D[0];

        private Dictionary<string, SkinnedMeshRenderer> allSkins;

        //private Dictionary<string, Texture2D> allTextures;




        private void Reload()
        {
            if(allSkins == null) allSkins = new Dictionary<string, SkinnedMeshRenderer>();
            //if (allTextures == null) allTextures = new Dictionary<string, Texture2D>();


            allSkins.Clear();
            for (int i = 0; i < skins.Length; i++){
                allSkins.Add(skins[i].id, skins[i].mesh);
            }

            //allTextures.Clear();
            //for (int i = 0; i < textures.Length; i++){
            //    allTextures.Add(textures[i].id, textures[i].texture);
            //}
        }


        public void LoadCharacter(SkinnedMeshRenderer meshRenderer, string meshID, string textureID)
        {
            Reload();

            if(allSkins.ContainsKey(meshID)){
                meshRenderer.sharedMesh = allSkins[meshID].sharedMesh;
            }
            //if (allTextures.ContainsKey(textureID)){
            //    meshRenderer.sharedMaterial.SetTexture("_MainTex", allTextures[textureID]);
            //}
        }



        public void CreateActorSkinObjects(string nameID, SkinnedMeshRenderer mesh)
        {
            ActorSkin actorSkin = ActorSkin.CreateAsset(this, nameID, mesh, path);
        }






        [Serializable]
        public class SkinMesh
        {
            public string id;
            public SkinnedMeshRenderer mesh;
        }


        [Serializable]
        public class TextureSet
        {
            public string id;
            public Texture2D texture;
        }
    }


}


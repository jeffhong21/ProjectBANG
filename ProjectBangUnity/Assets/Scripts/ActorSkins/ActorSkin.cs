using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace ActorSkins
{


    [CreateAssetMenu(fileName = "ActorSkin", menuName = "ActorSkins/Skins")]
    public class ActorSkin : ScriptableObject
    {
        
        public string nameID;

        public SkinnedMeshRenderer mesh;

        public string guid;

        public ActorSkinsManager manager;



        public static ActorSkin CreateAsset(ActorSkinsManager manager, string nameID, SkinnedMeshRenderer mesh, string path)
        {
            ActorSkin asset = CreateInstance<ActorSkin>();
            string assetDir = AssetDatabase.GenerateUniqueAssetPath(path + "/" + nameID + ".asset");

            asset.manager = manager;
            asset.nameID = nameID;
            asset.mesh = mesh;
            asset.guid = AssetDatabase.AssetPathToGUID(assetDir);

            AssetDatabase.CreateAsset(asset, assetDir);
            AssetDatabase.SaveAssets();

            return asset;
        }


        public SkinnedMeshRenderer GetRandomSkin()
        {
            int index = Random.Range(0, manager.skins.Length);
            return manager.skins[index].mesh;
        }

        public Texture2D GetRandmTexture()
        {
            int index = Random.Range(0, manager.textures.Length);
            return manager.textures[index];
        }

    }
}



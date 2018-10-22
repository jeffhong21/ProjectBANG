namespace AtlasAI
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;
    using System.IO;




    [Serializable]
    public class AIStorage : ScriptableObject
    {
        [SerializeField]
        private bool debug;

        [Multiline]
        public string description;
        [ReadOnly]
        public float version = 1.0f;
        [ReadOnly]
        public string aiId;


        [HideInInspector]
        public UtilityAI configuration;
        //[ReadOnly]
        //public string guid;


        public string friendlyName{
            get{
                var assetDir = AssetDatabase.GetAssetPath(this);
                return Path.GetFileNameWithoutExtension(assetDir);
            }
        }


        ///<summary>
        ///Creates an instance for the specified AI.
        ///</summary>
        ///<param name = "aiId" > The name of the ai that gets registered.</param>
        ///<param name = "aiName" > Name of the asset file.</param>
        ///<returns></returns>
        public static AIStorage CreateAsset(string aiId, string aiName, bool isSelect = false)
        {
            AIStorage asset = ScriptableObject.CreateInstance<AIStorage>();

            string assetDir = AssetDatabase.GenerateUniqueAssetPath(AIManager.StorageFolder + "/" + aiName + ".asset");

            //  Generate unique friendly name.
            //asset.friendlyName = Path.GetFileNameWithoutExtension(assetDir);
            asset.aiId = aiId;
            //asset.aiId = AINameMap.GenerateUniqueID(aiId);
            asset.configuration = new UtilityAI(asset.aiId);


            AssetDatabase.CreateAsset(asset, assetDir);
            AssetDatabase.SaveAssets();

            //asset.guid = AssetDatabase.AssetPathToGUID(assetDir);
            //AINameMap.Register(AssetDatabase.AssetPathToGUID(assetDir), asset.aiId);


            if (isSelect){
                Selection.activeObject = asset;
            }

            return asset;
        }


        //public static string GenerateUniqueID(string aiId)
        //{
        //    string _aiId = aiId;
        //    int index = 1;
        //    string suffix = "";

        //    do
        //    {
        //        if (aiNameMap.ContainsValue(_aiId))
        //        {
        //            suffix = " " + index.ToString();
        //            _aiId = aiId + suffix;
        //        }

        //        index++;
        //        if (index > 10)
        //            break;
        //    }
        //    while (aiNameMap.ContainsValue(_aiId) == true);

        //    return _aiId;
        //}
	}

}


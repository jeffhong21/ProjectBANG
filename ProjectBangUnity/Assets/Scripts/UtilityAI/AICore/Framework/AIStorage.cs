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
        //
        // Fields
        //
        [TextArea(1, 20)]
        public string description;
        [ReadOnly]
        public float version = 1.0f;
        [ReadOnly]
        public string aiId;                 //  Is the name of the ai.  The aiId should have a unique guid associated with it.
        [HideInInspector]
        public string configuration;        
        [HideInInspector]
        public string editorConfiguration;


        //
        // Properties
        //

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
            AIStorage asset = CreateInstance<AIStorage>();

            string assetDir = AssetDatabase.GenerateUniqueAssetPath(AIManager.StorageFolder + "/" + aiName + ".asset");

            asset.aiId = aiId;

            AssetDatabase.CreateAsset(asset, assetDir);
            AssetDatabase.SaveAssets();

            if (isSelect) Selection.activeObject = asset;
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


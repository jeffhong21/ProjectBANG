namespace CharacterSkins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;



    public enum CharacterSkinBoneType
    {
        Root,
        Head,
        Chest,
        Hips
    }


    public class CharaacterSkinsManager : ScriptableObject
    {
        public static string path = Path.GetDirectoryName("Resources/CharacterSkins/");
        public SkinSetData[] skinSetData = new SkinSetData[0];
        public TextureSetData[] textureSetData = new TextureSetData[0];


        private static Dictionary<string, SkinSetData> skinSetDataLookup = new Dictionary<string, SkinSetData>();
        private static Dictionary<string, TextureSetData> textureSetDataLookup = new Dictionary<string, TextureSetData>();








        public static void CreateSetData(ISetData setData)
        {
            if(setData is SkinSetData)
            {
                var skinSetData = (SkinSetData)setData;
                if(skinSetDataLookup.ContainsKey(skinSetData.SetId))
                {
                    Debug.LogFormat("Skin Set Data already contains {0}.  SetData not registered.", skinSetData.SetId);
                    return;
                }



            }
            else if(setData is TextureSetData)
            {
                var textureSetData = (TextureSetData)setData;
                if (skinSetDataLookup.ContainsKey(textureSetData.SetId))
                {
                    Debug.LogFormat("Skin Set Data already contains {0}.  SetData not registered.", textureSetData.SetId);
                    return;
                }
            }
            else
            {
                throw new Exception("Unknown set data type.");
            }


        }






    }

}

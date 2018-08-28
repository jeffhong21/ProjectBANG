﻿namespace AtlasAI
{
    using System;
    using System.Text.RegularExpressions;
    using System.IO;

    public static class AINameMapGenerator
    {


        internal const string NameMapFileName = "AINameMapHelper.cs";

        //
        // Static Fields
        //
        private static readonly Regex _nameCorrector;

        //
        // Static Methods
        //
        private static string correctName(string name){

            return name;
        }


        public static void WriteNameMapFile()
        {
            UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceUpdate);


            string filePath = AIManager.StorageFolder + "/" + NameMapFileName;
            string filterType = "t:AIStorage";


            using (StreamWriter outfile = new StreamWriter(filePath))
            {
                string itemTemplate = "";

                foreach (var guid in UnityEditor.AssetDatabase.FindAssets(filterType))
                {
                    string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    AIStorage aiAsset = UnityEditor.AssetDatabase.LoadMainAssetAtPath(assetPath) as AIStorage;

                    itemTemplate += string.Format(Template.ItemTemplate + "\n", aiAsset.aiId, guid);
                }


                string fileTemplate = string.Format(Template.FileTemplate, itemTemplate);
                outfile.WriteLine(fileTemplate);
                //UnityEditor.AssetDatabase.Refresh();

            }


        }

        //
        // Nested Types
        //
        private class Template
        {
            public const string ItemTemplate = "\t\tpublic static readonly Guid {0} = new Guid(\"{1}\");";

            public const string FileTemplate = "//------------------------------------------------------------------------------\r\n// <auto-generated>\r\n// This class was auto generated by the AI Name Map Generator\r\n// </auto-generated>\r\n//------------------------------------------------------------------------------\r\nnamespace AtlasAI\r\n{{\r\n    using System;\r\n\r\n    public static class AINameMapHelper\r\n    {{\r\n{0}\r\n    }}\r\n}}";

            public Template(){
                
            }
        }



    }
}
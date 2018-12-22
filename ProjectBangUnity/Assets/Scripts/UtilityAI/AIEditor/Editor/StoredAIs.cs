using System;
using System.Collections.Generic;
using UnityEngine;


namespace AtlasAI.AIEditor
{

    public static class StoredAIs
    {
        //
        // Static Fields
        //
        private static List<AIStorage> _ais;


        //
        // Static Properties
        //
        public static List<AIStorage> AIs{
            get { return _ais; }
        }


        //
        // Static Methods
        //
        public static string EnsureValidName(string name, AIStorage target)
        {
            throw new NotImplementedException();
        }


        public static AIStorage GetById(string aiId)
        {
            throw new NotImplementedException();
        }

        public static bool NameExists(string name)
        {
            throw new NotImplementedException();
        }

        public static void Refresh()
        {
            throw new NotImplementedException();
        }

        //
        // Nested Types
        //
        private class AIStorageComparer : IComparer<AIStorage>
        {
            public int Compare(AIStorage x, AIStorage y)
            {
                throw new NotImplementedException();
            }

            public AIStorageComparer()
            {
                
            }
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

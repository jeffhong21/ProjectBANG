using System;
using System.Collections.Generic;
using UnityEngine;


namespace AtlasAI.AIEditor
{

    public class AIGeneralSettings : ScriptableObject
    {
        
        public const string NameMapFileName = "AINameMapHelper.cs";
        private static AIGeneralSettings _instace;


        //
        // Fields
        //
        [HideInInspector, SerializeField]
        private string _storagePath;
        [HideInInspector, SerializeField]
        private string _nameMapPath;
        private bool _isDirty;

        //
        // Static Properties
        //
        internal static AIGeneralSettings instance
        {
            get;
        }

        //
        // Properties
        //
        internal bool isDirty
        {
            get;
        }

        internal string nameMapPath
        {
            get;
            set;
        }

        internal string storagePath
        {
            get;
            set;
        }

        //
        // Constructors
        //
        public AIGeneralSettings(){
            
        }


    }
}

namespace BuildingGenerator
{
    using UnityEngine;
    using System;
    using System.Collections;

    [CreateAssetMenu(fileName = "BuildingResourceManager", menuName = "BuildingGenerator/ResourceManager")]
    public class BuildingResourceManager : ScriptableObject
    {
        public BuildingAssets[] buildingAssets;



        [Serializable]
        public class BuildingAssets
        {
            public string name;
            public BuildingType buildingType;
            public GameObject[] foundations;
            public GameObject[] decks;
            public GameObject[] frontBalconies;
            public GameObject[] backBalconies;
            public GameObject[] facades;
            public GameObject[] frontWalls;
            public GameObject[] roofs;
            public GameObject[] signs;
        }
    }
}

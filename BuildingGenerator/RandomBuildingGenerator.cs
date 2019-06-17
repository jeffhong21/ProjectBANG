namespace BuildingGenerator
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;
    using Random = System.Random;
    using uRandom = UnityEngine.Random;


    public enum BuildingType { Double, Single, Large }

    public enum BalconyType { Balcony, DeckCover, BalconyStairs }

    public enum FrontWallType { Single, Double }

    [CreateAssetMenu(fileName = "BuildingGenerator", menuName = "BuildingGenerator/RandomBuildingGenerator")]
    public class RandomBuildingGenerator : ScriptableObject
    {
        public const string foundationID = "Foundation";
        public const string deckID = "Deck";
        public const string frontBalconyID = "FrontBalcony";
        public const string backBalconyID = "BackBalcony";
        public const string facadeID = "Facade";
        public const string botWallID = "BotWall";
        public const string topWallID = "TopWall";
        public const string roofID = "Roof";
        public const string signID = "Sign";

        public BuildingResourceManager resourceManager;
        public string buildingName;
        public BuildingType buildingType;
        public Material[] materials;

        [SerializeField]
        private BuildingAsset[] foundations;
        [SerializeField]
        private BuildingAsset[] decks;
        [SerializeField]
        private Balcony[] frontBalconies;
        [SerializeField]
        private Balcony[] backBalconies;
        [SerializeField]
        private BuildingAsset[] facades;
        [SerializeField]
        private FrontWall[] frontWalls;
        [SerializeField]
        private BuildingAsset[] roofs;
        [SerializeField]
        private BuildingAsset[] signs;

        GameObject foundation, deck, frontBalcony, backBalcony, facade, botWall, topWall, roof, sign;

        private List<string> groupNames = new List<string>()
        {
            foundationID, deckID, frontBalconyID, backBalconyID, facadeID, botWallID, topWallID, roofID, signID
        };

        //private HashSet<GameObject> grpContainers = new HashSet<GameObject>();
        //  id container and the prefab
        private Dictionary<string, GameObject> grpContainers = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> assets = new Dictionary<string, GameObject>();


        private float[] secondStoryHeight = { 3f, 0f, 3f };
        private float[] signHeight = { 6.5f, 3.75f, 6.5f };


        GameObject buildingRoot;


        public void Generate()
        {
            grpContainers.Clear();
            assets.Clear();

            buildingRoot = new GameObject(buildingName);
            int typeIndex = Array.IndexOf(Enum.GetValues(buildingType.GetType()), buildingType);
            foreach (string id in groupNames)
            {
                var grp = new GameObject(id);


                if (id == backBalconyID)
                {
                    grp.transform.position = Vector3.zero;
                    grp.transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                else if (id == facadeID || id == roofID || id == topWallID)
                {
                    grp.transform.position = Vector3.zero + Vector3.up * secondStoryHeight[typeIndex];
                    grp.transform.rotation = Quaternion.identity;

                    if(buildingType == BuildingType.Large){
                        if(id == facadeID || id == roofID){
                            grp.transform.position = Vector3.zero;
                        }
                    }
                }
                else if (id == signID)
                {
                    grp.transform.position = Vector3.zero + Vector3.up * signHeight[typeIndex];
                    grp.transform.rotation = Quaternion.identity;
                }
                else
                {
                    grp.transform.position = Vector3.zero;
                    grp.transform.rotation = Quaternion.identity;
                }

                grp.transform.SetParent(buildingRoot.transform);

                grpContainers.Add(id, grp);

            }


            foundation = GetAsset(foundations, foundationID);
            deck = GetAsset(decks, deckID);
            frontBalcony = GetAsset(frontBalconies, frontBalconyID);
            backBalcony = GetAsset(backBalconies, backBalconyID);
            facade = GetAsset(facades, facadeID);
            botWall = GetAsset(frontWalls, botWallID);
            topWall = GetAsset(frontWalls, topWallID);
            roof = GetAsset(roofs, roofID);
            sign = GetAsset(signs, signID);



            Material mat = materials[uRandom.Range(0, materials.Length)];
            var allChildren = buildingRoot.GetComponentsInChildren<Transform>();
            foreach(var child in allChildren){
                if (child.gameObject.GetComponent<MeshRenderer>()){
                    child.gameObject.GetComponent<MeshRenderer>().material = mat;
                }
            }


            //  Parent all the assets to the root.
            foreach(KeyValuePair<string, GameObject> obj in assets){
                obj.Value.transform.SetParent(buildingRoot.transform);
            }

            //  Delete the group containers.
            foreach (KeyValuePair<string, GameObject> obj in grpContainers){
                var go = obj.Value;
                if (Application.isEditor)
                    DestroyImmediate(go);
                else
                    Destroy(go);
            }
            grpContainers.Clear();

        }


        private GameObject GetAsset(BuildingAsset[] prefabs, string id)
        {
            if (prefabs.Length == 0) return null;

            //BuildingAsset asset = GetRandom(prefabs, id);
            int index = uRandom.Range(0, prefabs.Length);
            BuildingAsset asset = prefabs[index];

            string assetPath = AssetDatabase.GetAssetPath(asset.prefab);
            UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            Debug.Log(assetPath);

            //  Instantiate the prefab
            //GameObject go = Instantiate(asset.prefab, grpContainers.ContainsKey(id) ? grpContainers[id].transform : buildingRoot.transform);
            GameObject go = Instantiate(prefab, grpContainers.ContainsKey(id) ? grpContainers[id].transform : buildingRoot.transform) as GameObject;
            //var go = UnityEditor.PrefabUtility.InstantiatePrefab(asset.prefab);
            //  Add it to the list of assets.
            assets.Add(id, go);

            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;

            if(id == signID){
                go.transform.rotation = Quaternion.Euler(0, -90, 0);
            }

            return go;
        }



        private BuildingAsset GetRandom(BuildingAsset[] prefabs, string id)
        {
            int index = 0;
            BuildingAsset asset;

            if(id == frontBalconyID){
                
            }
            else if(id == botWallID)
            {
                index = uRandom.Range(0, prefabs.Length + 1);
                FrontWall frontWall = prefabs[index] as FrontWall;
                if(frontWall.hasDoor == false)
                {
                    while(frontWall.hasDoor == false)
                    {
                        index = uRandom.Range(0, prefabs.Length + 1);
                        frontWall = prefabs[index] as FrontWall;
                    }
                }
            }
            else if(id == topWallID)
            {
                
            }
            else{
                index = uRandom.Range(0, prefabs.Length + 1);
            }


            var prefab = prefabs[index];

            return prefab;
        }



        public void GetDataFromResourceManager(BuildingType type)
        {
            if (resourceManager == null || resourceManager.buildingAssets.Length == 0){
                Debug.LogWarning("No Building Resource Manager.");
                return;
            }
            var assets = resourceManager.buildingAssets;
            int index = 0;
            //foreach(var assets in resourceManager.buildingAssets)
            for (int i = 0; i < assets.Length; i ++)
            {
                if(assets[i].buildingType == type){
                    index = i;
                    break;
                }
            }


            foundations = new BuildingAsset[assets[index].foundations.Length];
            for (int i = 0; i < assets[index].foundations.Length; i++)
            {
                foundations[i].prefab = assets[index].foundations[i];
            }

            decks = new BuildingAsset[assets[index].decks.Length];
            for (int i = 0; i < assets[index].decks.Length; i++)
            {
                decks[i].prefab = assets[index].decks[i];
            }

            frontBalconies = new BuildingAsset[assets[index].frontBalconies.Length] as Balcony[];
            for (int i = 0; i < assets[index].frontBalconies.Length; i++)
            {
                frontBalconies[i].prefab = assets[index].frontBalconies[i];
            }

            backBalconies = new BuildingAsset[assets[index].backBalconies.Length] as Balcony[];
            for (int i = 0; i < assets[index].backBalconies.Length; i++)
            {
                backBalconies[i].prefab = assets[index].backBalconies[i];
            }

            facades = new BuildingAsset[assets[index].facades.Length];
            for (int i = 0; i < assets[index].facades.Length; i++)
            {
                facades[i].prefab = assets[index].facades[i];
            }

            frontWalls = new BuildingAsset[assets[index].frontWalls.Length] as FrontWall[];
            for (int i = 0; i < assets[index].frontWalls.Length; i++)
            {
                frontWalls[i].prefab = assets[index].frontWalls[i];
            }

            roofs = new BuildingAsset[assets[index].roofs.Length];
            for (int i = 0; i < assets[index].roofs.Length; i++)
            {
                roofs[i].prefab = assets[index].roofs[i];
            }

            signs = new BuildingAsset[assets[index].signs.Length];
            for (int i = 0; i < assets[index].signs.Length; i++)
            {
                signs[i].prefab = assets[index].signs[i];
            }

            //[SerializeField]
            //private BuildingAsset[] foundations;
            //[SerializeField]
            //private BuildingAsset[] decks;
            //[SerializeField]
            //private Balcony[] frontBalconies;
            //[SerializeField]
            //private Balcony[] backBalconies;
            //[SerializeField]
            //private BuildingAsset[] facades;
            //[SerializeField]
            //private FrontWall[] frontWalls;
            //[SerializeField]
            //private BuildingAsset[] roofs;
            //[SerializeField]
            //private BuildingAsset[] signs;


        }





        [Serializable]
        public class BuildingAsset
        {
            public GameObject prefab;
            public int weight;
        }

        [Serializable]
        public class Balcony : BuildingAsset
        {
            public BalconyType type;
        }

        [Serializable]
        public class FrontWall : BuildingAsset
        {
            public FrontWallType type;
            public bool hasDoor;
        }
    }
}

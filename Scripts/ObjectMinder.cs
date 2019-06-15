////Made by Filmstorm - Author: Kieren Hovasapian 
////Object Minder - Keeps In-Game Object Changes in Sync with Editor

//using System.Collections;
//using System.IO;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//[ExecuteInEditMode]
//public class ObjectMinder : MonoBehaviour
//{

//    [Header("State of Play")]

//    public bool InEditor;
//    public bool InGame;

//    [Header("Controls")]

//    public bool setValuesToLastSave;
//    public string tagToSave;

//    private GameObject[] FoundObjectsInGame;
//    private List<GameObject> InGameObjects;


//    string savePath;



//    private void Awake()
//    {
//        InGameObjects = new List<GameObject>();

//        //Setup save path 
//        savePath = Path.Combine(Application.persistentDataPath, "saveFile");
//    }

//    //Save our data from the scenes objects
//    void Save()
//    {
//        using (var writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
//        {
//            for (int i = 0; i < InGameObjects.Count; i++)
//            {
//                Transform t = InGameObjects[i].transform;
//                writer.Write(t.localPosition.x);
//                writer.Write(t.localPosition.y);
//                writer.Write(t.localPosition.z);
//                writer.Write(t.localRotation.w);
//                writer.Write(t.localRotation.x);
//                writer.Write(t.localRotation.y);
//                writer.Write(t.localRotation.z);
//                writer.Write(t.localScale.x);
//                writer.Write(t.localScale.y);
//                writer.Write(t.localScale.z);

//            }
//        }

//        //Debug.Log("I tried to save!");
//    }

//    //Load specific data we saved previously
//    void Load()
//    {



//        using (var reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
//        {
//            for (int i = 0; i < InGameObjects.Count; i++)
//            {
//                Vector3 p;
//                Quaternion r;
//                Vector3 s;
//                p.x = reader.ReadSingle();
//                p.y = reader.ReadSingle();
//                p.z = reader.ReadSingle();
//                r.w = reader.ReadSingle();
//                r.x = reader.ReadSingle();
//                r.y = reader.ReadSingle();
//                r.z = reader.ReadSingle();
//                s.x = reader.ReadSingle();
//                s.y = reader.ReadSingle();
//                s.z = reader.ReadSingle();

//                //set position from load data
//                InGameObjects[i].transform.localPosition = p;
//                InGameObjects[i].transform.localRotation = r;
//                InGameObjects[i].transform.localScale = s;




//                if (i == InGameObjects.Count - 1)
//                {
//                    //Last operation now we break 
//                    setValuesToLastSave = false;
//                }

//            }
//        }
//    }
//    // Use this for initialization
//    void Start()
//    {

//        //Check if the game is in play or in editor mode
//        if (Application.isPlaying)
//        {
//            //Debug.Log("Running in play mode");
//            InEditor = false;
//            InGame = true;
//            setValuesToLastSave = true;
//        }
//        else
//        {
//            //Debug.Log("In editor");
//            InEditor = true;
//            InGame = false;
//            setValuesToLastSave = true;
//        }



//    }

//    // Update is called once per frame
//    void Update()
//    {

//        //If in game -- find all objects and store their transforms
//        if (tagToSave != null)
//        {
//            FoundObjectsInGame = GameObject.FindGameObjectsWithTag(tagToSave);
//            foreach (GameObject go in FoundObjectsInGame)
//            {
//                if (go.activeInHierarchy)
//                {
//                    if (!InGameObjects.Contains(go))
//                        //Save Gameobject to List?
//                        InGameObjects.Add(go);

//                }
//            }

//            //If in editor run the updated loader
//            if (InEditor)
//            {
//                if (setValuesToLastSave == true)
//                {
//                    Load();

//                }
//            }

//        }
//    }

//    //When we stop playing save object locations
//    private void OnApplicationQuit()
//    {
//        Save();
//    }




//}
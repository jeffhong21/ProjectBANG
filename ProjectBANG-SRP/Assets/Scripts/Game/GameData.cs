using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

using System;
using System.Collections.Generic;

using CharacterController;

//[CreateAssetMenu(fileName = "GameAsset", menuName = "Game/Level Dat")]
public class GameData : ScriptableObject
{
    const string PATH = "Assets/Resources/";
    const string ASSETNAME = "GameData.asset";
    const string PREFAB_PATH = "Prefabs/";


    private ObjectPool objectPool;
    [SerializeField]
    private CameraController cameraPrefab;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private InGameMenu inGameMenu;


    [SerializeField]
    private SceneData prototypeLevel;


    public CameraController CameraPrefab
    {
        get { return cameraPrefab; }
        set { cameraPrefab = value; }
    }

    public GameObject PlayerPrefab
    {
        get { return playerPrefab; }
        set { playerPrefab = value; }
    }

    public InGameMenu InGameMenu
    {
        get { return inGameMenu; }
        set { inGameMenu = value; }
    }

    public SceneData PrototypeLevel
    {
        get { return prototypeLevel; }
        set { prototypeLevel = value; }
    }






    public void InitializeGameData()
    {
        var _objectPool = Resources.Load<ObjectPool>(PREFAB_PATH + "ObjectPool");
        if (_objectPool != null) objectPool = Instantiate(_objectPool);


        if (FindObjectOfType<EventSystem>() == null){
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
    }







    [MenuItem("Assets/Create/Game/Game Data", false, -110)]
    private static void CreateGameData()
    {
        GameData levelData = CreateInstance<GameData>();
        string filePath = AssetDatabase.GenerateUniqueAssetPath(PATH + ASSETNAME);
        //Debug.LogFormat("{0} file path: <b> {1} </b>", gameData.name, filePath);
        AssetDatabase.CreateAsset(levelData, filePath);


        AssetDatabase.Refresh();
    }


}

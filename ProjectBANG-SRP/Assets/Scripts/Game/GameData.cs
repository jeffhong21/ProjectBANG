using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

//[CreateAssetMenu(fileName = "GameAsset", menuName = "Game/Level Dat")]
public class GameData : ScriptableObject
{
    const string PATH = "Assets/Resources/";
    const string ASSETNAME = "GameData.asset";
    const string PREFAB_PATH = "Prefabs/";


    private ObjectPool objectPool;
    [SerializeField]
    private GameObject playerPrefab;

    private InGameMenu inGameMenu;

    [Header("--  Level Data --")]
    [SerializeField]
    private SceneInfo bootScene;
    [SerializeField]
    private SceneData prototypeLevel;



    public GameObject PlayerPrefab
    {
        get { return playerPrefab; }
        set { playerPrefab = value; }
    }

    public InGameMenu IngameMenu
    {
        get { return inGameMenu; }
        set { inGameMenu = value; }
    }

    public SceneInfo BootScene
    {
        get { return bootScene; }
        set { bootScene = value; }
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

        var _ingameMenu = Resources.Load<InGameMenu>(PREFAB_PATH + "IngameMenu");
        if (_ingameMenu != null) inGameMenu = Instantiate(_ingameMenu);

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

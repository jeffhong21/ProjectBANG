using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

using CharacterController;
using CharacterController.UI;

public class LevelManager : SingletonMonoBehaviour<LevelManager>
{
    public static bool IsPaused;

    public static event Action<bool> OnPause = delegate { };
    public static event Action OnRestartScene = delegate { };


    [SerializeField]
    private GameData gameData;
    private string[] removeAssetsWithTag = { "Player" };

    private CameraController cameraController;
    private GameObject playerInstance;
    private InGameMenu inGameMenu;


    [Header("--  Debug --")]
    [SerializeField]
    private bool doNotRemoveAssetsWithTag;




    private int loadedLevelIndex;




    protected override void OnAwake()
    {
        if (gameData == null){
            //gameData = Resources.Load<GameData>("GameData");
            throw new NotImplementedException("<b><color=red> No LevelManager Data loaded. </color></b>");
        }

        //DontDestroyOnLoad(gameObject);


    }


    private void InitializeLevel()
    {
        if (!doNotRemoveAssetsWithTag){
            //  Remove all assets with Player tag.
            for (int i = 0; i < removeAssetsWithTag.Length; i++)
                RemoveAssetsWithTag(removeAssetsWithTag[i], true);
            ////  Removes all assets with StripGameObjectFromBuild from build.
            StripGameObjectsFromBuild();
        }


        //  Initialize any game data.
        gameData.InitializeGameData();

        //  Instantiate the UI.
        inGameMenu = Instantiate(gameData.InGameMenu);


        //  Spawn Player.
        InitializePlayer(true);
        //  Spawn Camera.
        InitializeCamera(playerInstance);
    }



    private void Start()
    {
        LoadScene(gameData.PrototypeLevel.ActiveScene);
        //  Initialize level.
        InitializeLevel();

        //Debug.LogFormat(" ** Loaded Scene: <b>{0}</b> | Root Count: {1}", SceneManager.GetActiveScene().name, SceneManager.GetActiveScene().rootCount);
    }




    /// <summary>
    /// Use this in the Start() method.  Using it in Awake() is too early because the scene hasn't been marked as loaded yet.
    /// </summary>
    private void LoadScene(SceneInfo scene)
    {
        if (Application.isEditor)
        {
            string scenePath = "Assets/" + scene.SceneName + ".unity";
            Scene loadedLevel = SceneManager.GetSceneByPath(scenePath);
            if (loadedLevel.isLoaded)
            {
                SceneManager.SetActiveScene(loadedLevel);
                loadedLevelIndex = loadedLevel.buildIndex;
                //Debug.LogFormat(" ** <b>{0}</b> is already loaded.", loadedLevel.name);
                return;
            }
        }

        //StartCoroutine(LoadLevel(scene.BuildIndex));
        StartCoroutine(LoadLevelAsync(scene.BuildIndex));
    }


    private IEnumerator LoadLevel(int sceneBuildIndex)
    {
        //  Prevent the Update() loop from executing while the scene loads.  enable component after all scenes has loaded.
        enabled = false;

        if (loadedLevelIndex > 0){
            yield return SceneManager.UnloadSceneAsync(loadedLevelIndex);
        }

        yield return SceneManager.LoadSceneAsync(
            sceneBuildIndex, LoadSceneMode.Additive
            );

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneBuildIndex));
        loadedLevelIndex = sceneBuildIndex;

        StripGameObjectsFromBuild();
        enabled = true;
    }


    private IEnumerator LoadLevelAsync(int sceneBuildIndex)
    {
        //  Prevent the Update() loop from executing while the scene loads.  enable component after all scenes has loaded.
        enabled = false;

        if (loadedLevelIndex > 0){
            yield return SceneManager.UnloadSceneAsync(loadedLevelIndex);
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);

        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log(progress);
            yield return null;
        }


        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneBuildIndex));
        loadedLevelIndex = sceneBuildIndex;

        StripGameObjectsFromBuild();
        enabled = true;
    }




    public void InitializePlayer(bool selectOnCreation = true)
    {
        if (doNotRemoveAssetsWithTag)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
                playerInstance = player;
        }

        if (playerInstance == null)
        {
            playerInstance = Instantiate(gameData.PlayerPrefab);

            playerInstance.transform.position = SpawnPointManager.GetSpawnPoint(0).Position;
            playerInstance.transform.rotation = SpawnPointManager.GetSpawnPoint(0).Rotation;
        }


		//  Select Player on creation.
		if (selectOnCreation) UnityEditor.Selection.activeGameObject = playerInstance;
    }



    private void InitializeCamera(GameObject target)
    {
        cameraController = CameraController.Instance;
        if (cameraController == null)
        {
            if (gameData.CameraPrefab != null)
                cameraController = Instantiate(gameData.CameraPrefab);
            else
                Debug.LogErrorFormat("<color=red><b>{0}</b>{1}</color>", "•[Error] ", "No Camera.");
            //cameraController = Instantiate(Resources.Load<CameraController>("Prefabs/PlayerCamera"));
        }

        cameraController.SetMainTarget(target);
        cameraController.transform.position = target.transform.position;
        cameraController.transform.rotation = target.transform.rotation;
    }




    private void StripGameObjectsFromBuild()
    {
        var objectsToDestroy = FindObjectsOfType<StripGameObjectFromBuild>();
        for (int i = 0; i < objectsToDestroy.Length; i++)
        {
            Destroy(objectsToDestroy[i].gameObject);
        }
    }


    //  Removes any Gameobject with designated tag.
    private void RemoveAssetsWithTag(string assetTag, bool debugLog = false)
    {
        string debug_msg = "Assets with tag (" + assetTag + ") that were removed:\n";
        GameObject[] assets = GameObject.FindGameObjectsWithTag(assetTag);

        if (assets.Length > 0)
        {
            for (int i = 0; i < assets.Length; i++)
            {
                debug_msg += assets[i].name + "\n";
                Destroy(assets[i]);
            }
            if (debugLog)
                Debug.Log(debug_msg);
        }
        //else{
        //    debug_msg = "No assets with tag (" + assetTag + ") removed.";
        //}
    }












    public static void PauseGame()
    {
        IsPaused = !IsPaused;
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;

        //Debug.LogFormat("Paused: {0} | TimeScale: {1}", IsPaused, Time.timeScale);
        OnPause(IsPaused);
    }


    public void RestartScene()
    {
        IsPaused = false;
        Time.timeScale = 1;

        string scenePath = gameData.PrototypeLevel.MainScene.ScenePath;
        int sceneIndex = SceneUtility.GetBuildIndexByScenePath(scenePath);

        StartCoroutine(LoadLevel(gameData.PrototypeLevel.ActiveScene.BuildIndex));

        //Scene scene = SceneManager.GetSceneByPath(scenePath);
        //SceneManager.LoadScene(scene.name);
        //SceneManager.SetActiveScene(scene);
        //LoadScene(gameData.PrototypeLevel.ActiveScene);

        OnRestartScene();
    }



}

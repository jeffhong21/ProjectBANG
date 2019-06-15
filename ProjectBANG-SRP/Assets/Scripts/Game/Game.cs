using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

using CharacterController;
using CharacterController.UI;

public class Game : SingletonMonoBehaviour<Game>
{
    public static bool IsPaused;

    public static event Action<bool> OnPause = delegate { };
    public static event Action OnRestartScene = delegate { };


    [SerializeField]
    private GameData m_GameData;
    private string[] m_RemoveAssetsWithTag = { "Player" };

    private CameraController m_CameraController;
    private GameObject m_PlayerInstance;



    private float timeScale = 1;
    private int m_LoadedLevelIndex;




    protected override void OnAwake()
    {
        if (m_GameData == null){
            //m_GameData = Resources.Load<GameData>("GameData");
            throw new NotImplementedException("<b><color=red> No Game Data loaded. </color></b>");
        }

        //DontDestroyOnLoad(gameObject);


    }


    private void InitializeLevel()
    {
        timeScale = Time.timeScale;
        //  Remove all assets with Player tag.
        for (int i = 0; i < m_RemoveAssetsWithTag.Length; i++)
            RemoveAssetsWithTag(m_RemoveAssetsWithTag[i], true);
        ////  Removes all assets with StripGameObjectFromBuild from build.
        StripGameObjectsFromBuild();

        //  Initialize any game data.
        m_GameData.InitializeGameData();
    }



    private void Start()
    {
        if (m_GameData.BootScene.BuildIndex != -1){
            Scene bootScene = SceneManager.GetSceneByPath(m_GameData.BootScene.FullScenePath);
            if(bootScene.GetHashCode() != SceneManager.GetActiveScene().GetHashCode())
                StartCoroutine(LoadLevel(m_GameData.BootScene.BuildIndex));
        }

        LoadScene(m_GameData.PrototypeLevel.ActiveScene);

        //  Initialize level.
        InitializeLevel();
        //  Spawn Player.
        InitializePlayer(true);


        //Debug.LogFormat(" ** Loaded Scene: <b>{0}</b> | Root Count: {1}", SceneManager.GetActiveScene().name, SceneManager.GetActiveScene().rootCount);
    }






    private void Update()
    {

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Q))
        {
            RestartScene();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.LogFormat(" ** PrototypeLevel Active Scene: <b>{0}</b>\nBuild Index: <b>{1}</b>\nScene Path: <b>{2}</b>\nFull Scene Path: <b>{3}</b>\n",
                m_GameData.PrototypeLevel.ActiveScene.Scene.name,
                m_GameData.PrototypeLevel.ActiveScene.BuildIndex,
                m_GameData.PrototypeLevel.ActiveScene.ScenePath,
                m_GameData.PrototypeLevel.ActiveScene.FullScenePath
                );

        }

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
                m_LoadedLevelIndex = loadedLevel.buildIndex;
                Debug.LogFormat(" ** <b>{0}</b> is already loaded.", loadedLevel.name);
                return;
            }
        }

        StartCoroutine(LoadLevel(scene.BuildIndex));
    }





    private IEnumerator LoadLevel(int sceneBuildIndex)
    {
        //  Prevent the Update() loop from executing while the scene loads.  enable component after all scenes has loaded.
        enabled = false;

        if (m_LoadedLevelIndex > 0){
            yield return SceneManager.UnloadSceneAsync(m_LoadedLevelIndex);
        }

        yield return SceneManager.LoadSceneAsync(
            sceneBuildIndex, LoadSceneMode.Additive
            );

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneBuildIndex));
        m_LoadedLevelIndex = sceneBuildIndex;

        StripGameObjectsFromBuild();

        enabled = true;
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




    public void InitializePlayer(bool selectOnCreation = true)
    {
        if (m_PlayerInstance == null)
            m_PlayerInstance = Instantiate(m_GameData.PlayerPrefab);
        SpawnPointManager.SpawnObject(m_PlayerInstance);

        //  Initialize camera.
        m_CameraController = Resources.Load<CameraController>("Prefabs/PlayerCamera");
        m_CameraController = Instantiate(m_CameraController);

        m_CameraController.SetMainTarget(m_PlayerInstance);
        m_CameraController.transform.position = m_PlayerInstance.transform.position;
        m_CameraController.transform.rotation = m_PlayerInstance.transform.rotation;

        //  Select Player on creation.
        if (selectOnCreation) UnityEditor.Selection.activeGameObject = m_PlayerInstance;
    }







    public static void PauseGame()
    {
        IsPaused = !IsPaused;
        Instance.timeScale = IsPaused ? 0f : 1f;
        Time.timeScale = Instance.timeScale;

        //Debug.LogFormat("Paused: {0} | TimeScale: {1}", IsPaused, Time.timeScale);
        OnPause(IsPaused);
    }


    public void RestartScene()
    {
        //IsPaused = false;
        //timeScale = 1;
        //Time.timeScale = 1;

        string scenePath = m_GameData.PrototypeLevel.MainScene.ScenePath;
        int sceneIndex = SceneUtility.GetBuildIndexByScenePath(scenePath);

        StartCoroutine(LoadLevel(m_GameData.PrototypeLevel.ActiveScene.BuildIndex));

        //Scene scene = SceneManager.GetSceneByPath(scenePath);
        //SceneManager.LoadScene(scene.name);
        //SceneManager.SetActiveScene(scene);
        //LoadScene(m_GameData.PrototypeLevel.ActiveScene);

        OnRestartScene();
    }



}

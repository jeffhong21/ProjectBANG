using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

[InitializeOnLoad]
public class OpenScenes
{
    private readonly static string[] SceneSearchFolders = { "Assets/Scenes" };

    private static Dictionary<string, SceneAsset> allSceneAssets = new Dictionary<string, SceneAsset>();
    private static Dictionary<string, Scene> allScenes = new Dictionary<string, Scene>();



    static OpenScenes()
    {
        RegisterAllScenes();


        EditorSceneManager.newSceneCreated -= RegisterAllScenes;
        EditorSceneManager.newSceneCreated += RegisterAllScenes;
    }



    private static void RegisterAllScenes(Scene scene, NewSceneSetup setup, NewSceneMode mode)
    {
        Debug.LogFormat("Scene: <b>{0}</b>\n SceneSetup: <b>{1}</b>  SceneSetup: <b>{2}</b>", scene.path, setup, mode);

        RegisterAllScenes();
    }

    private static void RegisterAllScenes()
    {
        if (allSceneAssets == null) allSceneAssets = new Dictionary<string, SceneAsset>();
        if (allScenes == null) allScenes = new Dictionary<string, Scene>();

        //var sceneAssets = Resources.FindObjectsOfTypeAll<SceneAsset>();
        string[] guids = AssetDatabase.FindAssets("t:SceneAsset", SceneSearchFolders);
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            Scene scene = SceneManager.GetSceneByPath(path);
            if (allSceneAssets.ContainsKey(path) == false)
            {
                allSceneAssets.Add(path, sceneAsset);
            }
            if (allScenes.ContainsKey(path) == false)
            {
                allScenes.Add(path, scene);
            }
        }


    }




    [MenuItem("Scenes/Prefab Edit Environment", false, 1)]
    public static void OpenPrefabEditEnvironmentScene()
    {
        string path = "Assets/Scenes/_Prefab_Edit_Environment/PrefabEditEnvironment.unity";
        OpenScene(path);
    }

    [MenuItem("Scenes/UI Setup", false, 2)]
    public static void OpenUISetupScene()
    {
        string path = "Assets/Scenes/UISetupScene.unity";
        OpenScene(path);
    }

    [MenuItem("Scenes/Prototype Scene", false, 100)]
    public static void OpenPrototypeScene()
    {
        string path = "Assets/Scenes/Prototype_Level/Prototype_Level.unity";
        OpenScene(path);
    }

    [MenuItem("Scenes/Prototype Environment Scene", false, 101)]
    public static void OpenPrototypeEnvironmentScene()
    {
        string path = "Assets/Scenes/Prototype_Environment/Prototype_Environment.unity";
        OpenScene(path);
    }

    [MenuItem("Scenes/Prototype Environment-IntenseLighting Scene", false, 101)]
    public static void OpenPrototypeEnvironmentIntenseScene()
    {
        string path = "Assets/Scenes/Prototype_Environment/Prototype_Environment-IntenseLighting.unity";
        OpenScene(path);
    }


    [MenuItem("Scenes/Debug All Scenes", false, 1000)]
    public static void DebugAllScenes()
    {
        RegisterAllScenes();

        foreach (var item in allSceneAssets)
        {
            Debug.Log(item.Key);
        }
    }


    private static void OpenScene(string path)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }
        EditorSceneManager.OpenScene(path);
    }
}

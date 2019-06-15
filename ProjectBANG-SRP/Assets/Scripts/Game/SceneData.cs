using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using Object = UnityEngine.Object;
using System;
using System.Collections;
using System.Collections.Generic;

//[CreateAssetMenu(fileName = "GameAsset", menuName = "Game/Level Dat")]
public class SceneData : ScriptableObject
{
    const string PATH = "Assets/Resources/";
    const string ASSETNAME = "SceneData.asset";


    [SerializeField]
    private SceneInfo mainScene;
    [SerializeField]
    private SceneInfo activeScene;



    public SceneInfo MainScene
    {
        get { return mainScene; }
        set { mainScene = value; }
    }

    public SceneInfo ActiveScene
    {
        get { return activeScene; }
        set { activeScene = value; }
    }





    [MenuItem("Assets/Create/Game/Level Data", false, -109)]
    private static void CreateGameData()
    {
        SceneData levelData = CreateInstance<SceneData>();
        string filePath = AssetDatabase.GenerateUniqueAssetPath(PATH + ASSETNAME);
        //Debug.LogFormat("{0} file path: <b> {1} </b>", gameData.name, filePath);
        AssetDatabase.CreateAsset(levelData, filePath);


        AssetDatabase.Refresh();

    }




    //public IEnumerator LoadLevel()
    //{

    //    SceneManager.LoadScene(mainScene.name);
    //    //SceneManager.LoadScene(environmentScene.name, LoadSceneMode.Additive);
    //    //yield return null;


    //    yield return SceneManager.LoadSceneAsync(
    //        environmentScene.name, LoadSceneMode.Additive
    //        );

    //    SceneManager.SetActiveScene(SceneManager.GetSceneByName(environmentScene.name));
    //}
}

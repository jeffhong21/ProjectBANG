using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CharacterController;
using CharacterController.UI;

public class Game : SingletonMonoBehaviour<Game> 
{
    private CameraController m_CameraController;
    [SerializeField]
    private GameObject m_PlayerPrefab;
    private GameObject m_PlayerInstance;
    //[SerializeField]
    private GameObject m_IngameHUD;



    private SpawnPoints[] m_SpawnPoints;


	protected override void Awake()
	{
        base.Awake();

        RemoveAssetsWithTag("Player", true);

        Initialize();
	}


	private void Initialize()
    {
        m_CameraController = Resources.Load<CameraController>("Prefabs/PlayerCamera");
        m_CameraController = Instantiate(m_CameraController);

        m_SpawnPoints = FindObjectsOfType<SpawnPoints>();


        var objectsToDestroy = FindObjectsOfType<StripGameObjectFromBuild>();
        for (int i = 0; i < objectsToDestroy.Length; i++){
            Destroy(objectsToDestroy[i].gameObject);
        }
    }





	private void Start()
	{
        SpawnPlayer();
	}





    public GameObject Spawn(GameObject obj, Vector3 position, Quaternion rotation)
    {
        var go = Instantiate(obj, position, rotation);
        return go;
    }


    public void SpawnPlayer(int spawnIndex = 0){
        if (m_SpawnPoints.Length > 0){
            if(m_SpawnPoints.Length < spawnIndex)
                m_PlayerInstance = Spawn(m_PlayerPrefab, m_SpawnPoints[spawnIndex].Position, m_SpawnPoints[spawnIndex].Rotation);
            else
                m_PlayerInstance = Spawn(m_PlayerPrefab, m_SpawnPoints[0].Position, m_SpawnPoints[0].Rotation);
        }
        else{
            m_PlayerInstance = Spawn(m_PlayerPrefab, Vector3.zero, Quaternion.identity);
        }
        UnityEditor.Selection.activeGameObject = m_PlayerInstance;

        m_CameraController.SetMainTarget(m_PlayerInstance);
        m_CameraController.transform.position = m_PlayerInstance.transform.position;
        m_CameraController.transform.rotation = m_PlayerInstance.transform.rotation;
    }






    //  Removes any Gameobject with designated tag.
    private void RemoveAssetsWithTag(string assetTag, bool debugLog = false)
    {
        string debug_msg = "Assets with tag (" + assetTag + ") that were removed:\n";
        GameObject[] assets = GameObject.FindGameObjectsWithTag(assetTag);

        if (assets.Length > 0){
            for (int i = 0; i < assets.Length; i++){
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

}

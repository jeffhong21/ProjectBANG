using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CharacterController;
using CharacterController.UI;

public class GameManager : SingletonMonoBehaviour<GameManager> 
{

    [SerializeField]
    private GameObject m_CameraController;
    [SerializeField]
    private CameraController m_CameraControllerInstance;
    [SerializeField]
    private float cameraLockDelay = 1;

    [Header("-- Player Settings--")]
    [SerializeField]
    private GameObject m_PlayerPrefab;
    private GameObject m_PlayerInstance;
    [SerializeField]
    private GameObject m_IngameHUD;
    private IngameHUD m_IngameHudInstance;
    //[SerializeField]
    //private GameObject m_CharacterHUD;
    //[SerializeField]
    //private GameObject m_ItemHUD;

    [Header("-- Agent Settings--")]
    [SerializeField]
    private GameObject m_AgentPrefab;
    [SerializeField, Range(0, 20)]
    private int m_AgentCount;
    private SpawnPoints[] m_SpawnPoints = new SpawnPoints[0];
    private float m_SpawnInterval = 2;




 //   public GameObject PlayerInstance{
 //       get { return m_PlayerInstance; }
 //   }




	//protected override void Awake()
	//{
 //       base.Awake();

 //       RemoveAssetsWithTag("Player", true);

 //       //m_CameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();;
 //       if (m_CameraControllerInstance == null){
 //           if (CameraController.Instance == null && m_CameraController != null)
 //               m_CameraControllerInstance = Instantiate(m_CameraController).GetComponent<CameraController>();

 //           else if (m_CameraController == null && CameraController.Instance != null)
 //               m_CameraControllerInstance = CameraController.Instance;
 //           //else
 //           //Debug.LogError("GameManager has no Camera");
 //       }
 //       m_CameraController.transform.position = Vector3.zero;



 //       GetAllSpawnPoints();
	//}


	//private void Start()
	//{
    //    //  Lock the camera rotation for a few seconds.
    //    StartCoroutine(LockCameraDelay());
    //    //  Initialize Player.
    //    InitializePlayer();

    //    //  Spawn Agents.
    //    StartCoroutine(SpawnAgents(m_SpawnInterval));

    //    if(m_PlayerInstance != null) UnityEditor.Selection.activeGameObject = m_PlayerInstance;
    //}



    //private void GetAllSpawnPoints()
    //{
    //    m_SpawnPoints = GetComponentsInChildren<SpawnPoints>();
    //}


    //private void InitializePlayer()
    //{
    //    if (m_SpawnPoints.Length <= 0){
    //        m_PlayerInstance = Spawn(m_PlayerPrefab, Vector3.zero, Quaternion.identity);
    //    }
    //    else{
    //        m_PlayerInstance = Spawn(m_PlayerPrefab, m_SpawnPoints[0].Position, m_SpawnPoints[0].Rotation);
    //    }

    //    //if (m_CharacterHUD != null)
    //    //    m_CharacterHUD = Instantiate(m_CharacterHUD);
    //    //if (m_ItemHUD != null)
    //    //m_ItemHUD = Instantiate(m_ItemHUD);
    //    m_IngameHudInstance = Instantiate(m_IngameHUD).GetComponent<IngameHUD>();
    //    m_IngameHudInstance.SetPanelActive(true);

    //    SetCameraTarget(m_PlayerInstance);
    //}


    //private void SetCameraTarget(GameObject target)
    //{
    //    m_CameraControllerInstance.SetMainTarget(target);
    //}



    //public GameObject Spawn(GameObject obj, Vector3 position, Quaternion rotation)
    //{
    //    var go = Instantiate(obj, position, rotation);
    //    return go;
    //}


    //private IEnumerator SpawnAgents(float intervalTime)
    //{
    //    var spawnInterval = new WaitForSeconds(intervalTime);

    //    if (m_SpawnPoints.Length > 0)
    //    {
    //        int spawnIndex = 1;
    //        for (int i = 0; i < m_AgentCount; i++)
    //        {
    //            if (spawnIndex == m_SpawnPoints.Length)
    //                spawnIndex = 1;
    //            var go = Spawn(m_AgentPrefab, m_SpawnPoints[spawnIndex].GetSpawnPosition(), m_SpawnPoints[spawnIndex].Rotation);
    //            go.name = string.Format("{0}({1})", "AI Agent", i);
    //            go.GetComponent<ActorSkins.ActorSkinComponent>().LoadActorSkin();

    //            spawnIndex++;
    //            yield return spawnInterval;
    //        }
    //    }

    //    yield return null;
    //}



    //private IEnumerator LockCameraDelay()
    //{
    //    CameraController.LockRotation = true;
    //    yield return new WaitForSeconds(cameraLockDelay);
    //    CameraController.LockRotation = false;
    //}



    ////  Removes any Gameobject with designated tag.
    //private void RemoveAssetsWithTag(string assetTag, bool debugLog = false)
    //{
    //    string debug_msg = "Assets with tag (" + assetTag + ") that were removed:\n";
    //    GameObject[] assets = GameObject.FindGameObjectsWithTag(assetTag);

    //    if (assets.Length > 0){
    //        for (int i = 0; i < assets.Length; i++){
    //            debug_msg += assets[i].name + "\n";
    //            Destroy(assets[i]);
    //        }
    //        if (debugLog)
    //            Debug.Log(debug_msg);
            
    //    }
    //    //else{
    //    //    debug_msg = "No assets with tag (" + assetTag + ") removed.";
    //    //}

    //}
}

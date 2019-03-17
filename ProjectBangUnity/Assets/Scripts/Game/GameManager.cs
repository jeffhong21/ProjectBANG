using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CharacterController;

public class GameManager : SingletonMonoBehaviour<GameManager> 
{
    [SerializeField]
    private CameraController m_CameraController;
    [Header("-- Player Settings--")]
    [SerializeField]
    private GameObject m_PlayerPrefab;
    private GameObject m_PlayerInstance;
    [SerializeField]
    private GameObject m_CrosshairsHUD;

    [Header("-- Agent Settings--")]
    [SerializeField]
    private GameObject m_AgentPrefab;
    [SerializeField, Range(0, 20)]
    private int m_AgentCount;
    private SpawnPoints[] m_SpawnPoints = new SpawnPoints[0];
    private float m_SpawnInterval = 2;




    public GameObject PlayerInstance{
        get { return m_PlayerInstance; }
    }




	protected override void Awake()
	{
        base.Awake();

        if (CameraController.Instance == null && m_CameraController)
            m_CameraController = Instantiate(m_CameraController) as CameraController;
        
        else if (m_CameraController == null && CameraController.Instance != null)
            m_CameraController = CameraController.Instance;
        
        else
            Debug.LogError("GameManager has no Camera");


        GetAllSpawnPoints();
	}


	private void Start()
	{
        //  Initialize Player.
        InitializePlayer();

        //  Spawn Agents.
        StartCoroutine(SpawnAgents(m_SpawnInterval));

        if(m_PlayerInstance != null) UnityEditor.Selection.activeGameObject = m_PlayerInstance;
    }



    private void GetAllSpawnPoints()
    {
        m_SpawnPoints = GetComponentsInChildren<SpawnPoints>();
    }


    private void InitializePlayer()
    {
        if (m_SpawnPoints.Length <= 0){
            m_PlayerInstance = Spawn(m_PlayerPrefab, Vector3.zero, Quaternion.identity);
        }
        else{
            m_PlayerInstance = Spawn(m_PlayerPrefab, m_SpawnPoints[0].Position, m_SpawnPoints[0].Rotation);
        }

        if(m_CrosshairsHUD != null) m_CrosshairsHUD = Instantiate(m_CrosshairsHUD);


        SetCameraTarget(m_PlayerInstance);
    }


    private void SetCameraTarget(GameObject target)
    {
        m_CameraController.SetMainTarget(target);
    }



    public GameObject Spawn(GameObject obj, Vector3 position, Quaternion rotation)
    {
        var go = Instantiate(obj, position, rotation);
        return go;
    }


    private IEnumerator SpawnAgents(float intervalTime)
    {
        var spawnInterval = new WaitForSeconds(intervalTime);

        if (m_SpawnPoints.Length > 0)
        {
            int spawnIndex = 1;
            for (int i = 0; i < m_AgentCount; i++)
            {
                if (spawnIndex == m_SpawnPoints.Length)
                    spawnIndex = 1;
                var go = Spawn(m_AgentPrefab, m_SpawnPoints[spawnIndex].GetSpawnPosition(), m_SpawnPoints[spawnIndex].Rotation);
                go.name = string.Format("{0}({1})", "AI Agent", i);
                go.GetComponent<ActorSkins.ActorSkinComponent>().LoadActorSkin();

                spawnIndex++;
                yield return spawnInterval;
            }
        }

        yield return null;
    }




}

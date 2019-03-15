using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CharacterController;

public class GameManager : SingletonMonoBehaviour<GameManager> 
{
    [SerializeField]
    private CameraController m_CameraController;
    public GameObject m_Player;

    public GameObject m_Agent;
    [Range(0, 20)]
    public int m_AgentCount;




    private SpawnPoints[] m_SpawnPoints = new SpawnPoints[0];
    private float m_SpawnInterval = 2;




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
        GameObject player = null;
        if (m_SpawnPoints.Length <= 0){
            player = Spawn(m_Player, Vector3.zero, Quaternion.identity);
        } else {
            player = Spawn(m_Player, m_SpawnPoints[0].Position, m_SpawnPoints[0].Rotation);
        }

        SetCameraTarget(player);


        StartCoroutine(SpawnAgents(m_SpawnInterval));

        if(player != null) UnityEditor.Selection.activeGameObject = player;
    }


    private void GetAllSpawnPoints()
    {
        m_SpawnPoints = GetComponentsInChildren<SpawnPoints>();
    }


    private void SetCameraTarget(GameObject target)
    {


        m_CameraController.SetMainTarget(target.transform);
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
                var go = Spawn(m_Agent, m_SpawnPoints[spawnIndex].GetSpawnPosition(), m_SpawnPoints[spawnIndex].Rotation);
                go.name = string.Format("{0}({1})", "AI Agent", i);
                go.GetComponent<ActorSkins.ActorSkinComponent>().LoadActorSkin();

                spawnIndex++;
                yield return spawnInterval;
            }
        }

        yield return null;
    }

}

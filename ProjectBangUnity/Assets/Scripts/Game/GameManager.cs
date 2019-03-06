using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager> 
{

    public GameObject m_Player;

    public GameObject m_Agent;
    [Range(0, 20)]
    public int m_AgentCount;




    private SpawnPoints[] m_SpawnPoints = new SpawnPoints[0];
    private float m_SpawnInterval = 1;




	protected override void Awake()
	{
        base.Awake();

        GetAllSpawnPoints();
	}


	private void Start()
	{
        if (m_SpawnPoints.Length <= 0){
            Spawn(m_Player, Vector3.zero, Quaternion.identity);
        } else {
            Spawn(m_Player, m_SpawnPoints[0].Position, m_SpawnPoints[0].Rotation);
        }



        StartCoroutine(SpawnAgents(m_SpawnInterval));
    }


    private void GetAllSpawnPoints()
    {
        m_SpawnPoints = GetComponentsInChildren<SpawnPoints>();
    }


    private void SetCameraTarget()
    {
        
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
            for (int i = 0; i < m_AgentCount; i++)
            {
                if (i + 1 == m_SpawnPoints.Length)
                    break;
                var go = Spawn(m_Agent, m_SpawnPoints[i + 1].Position, m_SpawnPoints[i + 1].Rotation);
                go.GetComponent<ActorSkins.ActorSkinComponent>().LoadActorSkin();

                yield return spawnInterval;
            }
        }

        yield return null;
    }

}

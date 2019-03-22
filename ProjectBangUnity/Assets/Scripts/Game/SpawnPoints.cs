using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour 
{
    public enum SpawnType {Default, SpawnOnly, RespawnOnly}

    [Serializable]
    public class SpawPointDebugSettings
    {
        [Range(0, 1)]
        public float gizmoSize = 0.25f;
        public Color defaultColor = new Color(0, 1, 1, 0.5f);
        public Color spawnColor = new Color(1, 0, 1, 0.5f);
        public Color respawnColor = new Color(0, 1, 0, 0.5f);
        public Color textColor = new Color(1, 1, 1, 1);
    }


    public SpawnType m_SpawnType = SpawnType.Default;
    public float m_Radius = 1;
    public float m_DelayBetweenSpawns = 3;


    [Space]
    public SpawPointDebugSettings m_DebugSettings = new SpawPointDebugSettings();


    private GameObject m_GameObject;
    private Transform m_Transform;




    //
    //  Properties
    // 
    public Vector3 Position{
        get { return transform.position; }
    }

    public Quaternion Rotation{
        get { return transform.rotation; }
    }


	//
    //  Methods
    //
    private void Awake()
	{
        m_GameObject = gameObject;
        m_Transform = transform;
	}


    public Vector3 GetSpawnPosition()
    {
        Vector3 spawnPoint = m_Transform.position;
        spawnPoint = m_Transform.position + (UnityEngine.Random.insideUnitSphere * m_Radius);
        spawnPoint.y = 0;
        return spawnPoint;
    }




    //private void OnDrawGizmos()
    //{
    //    DrawGizmos();
    //}

    #if UNITY_EDITOR

	private void OnDrawGizmosSelected()
	{
        DrawGizmos();
	}


	public void DrawGizmos()
	{
        if (m_DebugSettings == null) return;

        switch (m_SpawnType)
        {
            case SpawnType.SpawnOnly:
                Gizmos.color = m_DebugSettings.spawnColor;
                break;
            case SpawnType.RespawnOnly:
                Gizmos.color = m_DebugSettings.respawnColor;
                break;
            default:
                Gizmos.color = m_DebugSettings.defaultColor;
                break;
        }

        Gizmos.DrawSphere(transform.position, m_Radius);
        Gizmos.DrawRay(transform.position + transform.up * 1.5f, transform.forward);

        GizmosUtils.DrawString(gameObject.name, transform.position + Vector3.up, m_DebugSettings.textColor);

	}

    #endif
}




    //public class SpawnPoint : MonoBehaviour
    //{
    //    public int grouping;
    //    public SpawnShape shape;

    //    public float radius = 1f;
    //    public float xLength = 1f;
    //    public float zLength = 1f;
    //    public Color gizmoColor = new Color(1f, 0f, 0f, 0.2f);

    //    private Vector3 boxSize = Vector3.one;


    //    private void Awake()
    //    {
    //        gameObject.tag = "RespawnPoint";
    //    }


    //    private void OnEnable()
    //    {
    //        boxSize = new Vector3(xLength, 1, zLength);
    //    }


    //    public Vector3 GetSpawnPoint()
    //    {
    //        Vector3 spawnPoint = transform.position;
    //        switch (shape)
    //        {
    //            case SpawnShape.Point:
    //                spawnPoint = transform.position;
    //                break;
    //            case SpawnShape.Sphere:
    //                spawnPoint = transform.position + (Random.insideUnitSphere * radius);
    //                spawnPoint.y = 0;
    //                break;
    //            case SpawnShape.Box:
    //                float randomX = Random.Range(transform.position.x - xLength, transform.position.x + xLength);
    //                float randomZ = Random.Range(transform.position.z - zLength, transform.position.z + zLength);
    //                Vector3 randomRange = new Vector3(randomX, 0, randomZ);

    //                spawnPoint = transform.position + randomRange;
    //                break;
    //        }

    //        return spawnPoint;
    //    }


    //    private void OnDrawGizmosSelected()
    //    {
    //        Gizmos.color = gizmoColor;
    //        switch (shape)
    //        {
    //            case SpawnShape.Point:
    //                Gizmos.DrawSphere(transform.position, 0.25f);
    //                break;
    //            case SpawnShape.Sphere:
    //                Gizmos.DrawSphere(transform.position, radius);
    //                break;
    //            case SpawnShape.Box:
    //                boxSize.Set(xLength, 1, zLength);
    //                Gizmos.DrawCube(transform.position + Vector3.up * (boxSize.y * 0.5f), boxSize);
    //                break;
    //        }    
    //    }

    //}

    //public enum SpawnShape
    //{
    //    Point,
    //    Sphere,
    //    Box
    //}
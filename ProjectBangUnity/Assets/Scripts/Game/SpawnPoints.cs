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
        public Color defaultColor = Color.cyan;
        public Color spawnColor = Color.green;
        public Color respawnColor = Color.magenta;

    }



    public SpawnType m_SpawnType = SpawnType.Default;

    public SpawPointDebugSettings m_DebugSettings = new SpawPointDebugSettings();


    private GameObject m_GameObject;
    private Transform m_Transform;



    public Vector3 Position{
        get { return transform.position; }
    }

    public Quaternion Rotation{
        get { return transform.rotation; }
    }


	private void Awake()
	{
        m_GameObject = gameObject;
        m_Transform = transform;
	}






    private void OnDrawGizmos()
    {
        DrawGizmos();
    }


	//private void OnDrawGizmosSelected()
	//{
 //       DrawGizmos();
	//}


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


        Gizmos.DrawSphere(transform.position, m_DebugSettings.gizmoSize);
	}
}

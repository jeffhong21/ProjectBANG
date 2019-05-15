using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour 
{
    public enum SpawnType {Default, SpawnOnly, RespawnOnly}

    public static bool useDrawGizmos;

    [Serializable]
    public class SpawPointDebugSettings
    {
        [Range(0, 1)]
        public float gizmoSize = 0.25f;
        public Color defaultColor = new Color(0, 0.64f, 1, 0.5f);
        public Color spawnColor = new Color(1, 0, 1, 0.5f);
        public Color respawnColor = new Color(0, 1, 0, 0.5f);
        public Color textColor = new Color(1, 1, 1, 1);
    }


    public SpawnType spawnType = SpawnType.Default;
    public float radius = 1;
    public float delayBetweenSpawns = 3;


    [Space]
    public SpawPointDebugSettings debugSettings = new SpawPointDebugSettings();




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


    public Vector3 GetSpawnPosition()
    {
        Vector3 spawnPoint = transform.position;
        spawnPoint = transform.position + (UnityEngine.Random.insideUnitSphere * radius);
        spawnPoint.y = 0;
        return spawnPoint;
    }





    //private void OnDrawGizmos()
    //{
    //    DrawGizmos();
    //}

#if UNITY_EDITOR

    private void OnDrawGizmos() => DrawGizmos(false);

    private void OnDrawGizmosSelected() => DrawGizmos(true);


	public void DrawGizmos(bool selected)
	{
        if (debugSettings == null) return;

        Color color = debugSettings.defaultColor;
        switch (spawnType)
        {
            case SpawnType.SpawnOnly:
                color = debugSettings.spawnColor;
                break;
            case SpawnType.RespawnOnly:
                color = debugSettings.respawnColor;
                break;
            default:
                color = debugSettings.defaultColor;
                break;
        }

        if (!selected)
            //color.a = color.a * 0.25f;
            color = Color.gray;


        if(Event.current.type == EventType.Repaint)
        {
            Gizmos.color = color;
            if (selected)
                Gizmos.DrawSphere(transform.position, debugSettings.gizmoSize);

            Gizmos.DrawWireSphere(transform.position, radius);



            if (selected){
                Color diskColor = color;
                diskColor.a = color.a * 0.2f;
                UnityEditor.Handles.color = diskColor;
                UnityEditor.Handles.DrawSolidDisc(transform.position, transform.up, radius);
            }


            UnityEditor.Handles.color = color;

            UnityEditor.Handles.ArrowHandleCap(
                0,
                transform.position + transform.up * radius,
                Quaternion.LookRotation(transform.forward),
                radius,
                EventType.Repaint );


                

            var displayText = gameObject.name + "\n" + transform.position.ToString();
            GizmosUtils.DrawString(displayText, transform.position + Vector3.up, debugSettings.textColor);
        }



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
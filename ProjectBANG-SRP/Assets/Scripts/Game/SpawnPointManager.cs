using System;
using System.Collections.Generic;
using UnityEngine;



public class SpawnPointManager : SingletonMonoBehaviour<SpawnPointManager>
{

    private static List<SpawnPoint> m_SpawnPoints = new List<SpawnPoint>();

    [SerializeField, DisplayOnly]
    private SpawnPoint[] spawnPoints;




    public static int Count {
        get { return m_SpawnPoints.Count; }
    }



    public static event Action OnSpawn = delegate { };



    protected override void OnAwake()
    {
        Initialize();

    }






    private static void Initialize()
    {
        SpawnPoint[] sp = FindObjectsOfType<SpawnPoint>();

        m_SpawnPoints.Clear();

        Instance.spawnPoints = new SpawnPoint[sp.Length];

        for (int i = 0; i < sp.Length; i++){
            m_SpawnPoints.Add(sp[i]);
            Instance.spawnPoints[i] = sp[i];
        }
    }


    public static SpawnPoint GetSpawnPoint(int index)
    {
        //DebugSpawnPoints();

        if (m_SpawnPoints.Count - 1 > index){
            return null;
        }
        return m_SpawnPoints[index];
    }



    public static void SpawnObject(GameObject instance, int spawnIndex = 0)
    {
        if (instance == null) {
            Debug.Log("Object to spawn is null");
            return;
        }
        if (Count > 0){
            if (Count <= spawnIndex){
                instance.transform.position = GetSpawnPoint(spawnIndex).Position;
                instance.transform.rotation = GetSpawnPoint(spawnIndex).Rotation;
            }
            else{
                instance.transform.position = GetSpawnPoint(0).Position;
                instance.transform.rotation = GetSpawnPoint(0).Rotation;
            }

        }
        else{
            instance.transform.position = Vector3.zero;
            instance.transform.rotation = Quaternion.identity;
            //Debug.Log("<b>** Couldn't find any SpawnPoints()</b>");
        }

        OnSpawn();
    }


    private static void DebugSpawnPoints()
    {
        string msg = "<b>SpawnPoints<b/>\n";
        foreach (var sp in m_SpawnPoints)
        {
            msg += sp + "\n";
        }
        Debug.Log(msg);
    }
}

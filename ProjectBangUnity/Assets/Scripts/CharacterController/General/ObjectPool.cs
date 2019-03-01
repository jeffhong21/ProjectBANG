using UnityEngine;
using System;



[Serializable]
public class ObjectPool
{
    [SerializeField]
    private string m_ID;
    [SerializeField]
    private GameObject m_Prefab;
    [SerializeField, Range(0, 100)]
    private int m_Count;
    [SerializeField, HideInInspector]
    private int m_MaxCount;




    public string ID
    {
        get { return m_ID; }
    }

    public GameObject Prefab
    {
        get { return m_Prefab; }
    }

    public int Count
    {
        get { return m_Count; }
    }

    public int MaxCount
    {
        get { return m_MaxCount; }
    }




}
using UnityEngine;
using System.Collections;

public class ObjectManager : SingletonMonoBehaviour<ObjectManager> 
{

    public GameObject[] characters = new GameObject[0];


	protected override void Awake()
	{
        base.Awake();


        DontDestroyOnLoad(this);
	}


}

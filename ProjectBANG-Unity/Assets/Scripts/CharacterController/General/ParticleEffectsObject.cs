using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectsObject : MonoBehaviour
{

    private ParticleSystem[] particleSystems = new ParticleSystem[0];

	private void Awake()
	{
        particleSystems = GetComponentsInChildren<ParticleSystem>();
	}

	private void Update()
	{
        //for (int i = 0; i < particleSystems.Length; i++)
        //{
        //    if (particleSystems[i].isPlaying) return;


        //}
	}




}

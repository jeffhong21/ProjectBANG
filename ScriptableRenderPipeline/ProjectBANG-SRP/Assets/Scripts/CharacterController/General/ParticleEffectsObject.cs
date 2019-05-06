using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectsObject : MonoBehaviour
{

    private ParticleSystem[] particleSystems = new ParticleSystem[0];

    private float duration;
    private float currentDuration;

	private void Awake()
	{
        particleSystems = GetComponentsInChildren<ParticleSystem>();
	}


	private void OnEnable()
	{
        for (int i = 0; i < particleSystems.Length; i++){
            if (particleSystems[i].main.duration > duration)
                duration = particleSystems[i].main.duration;
            particleSystems[i].Play(true);
        }
	}

	private void OnDisable()
	{
        for (int i = 0; i < particleSystems.Length; i++){
            particleSystems[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        duration = 0;
	}


    private IEnumerator Start()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }



	//private void Update()
	//{
 //       if(duration > 0){
 //           currentDuration += Time.deltaTime;
 //           if(currentDuration > duration){
 //               ObjectPool.Return(gameObject);
 //           }
 //       }
	//}




}

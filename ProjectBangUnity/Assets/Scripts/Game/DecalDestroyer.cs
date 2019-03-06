using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalDestroyer : MonoBehaviour
{

    public float lifeTime = 5.0f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(lifeTime);
        //ObjectPoolManager.Instance.Destroy(gameObject);
        Destroy(gameObject);
    }
}

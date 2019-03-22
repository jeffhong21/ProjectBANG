using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalDestroyer : MonoBehaviour
{
    public float lifeTime = 5.0f;

    private float currentDuration;
    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private Material material;




	private IEnumerator Start()
    {
        //currentDuration = 0;

        //if (meshRenderer == null)
        //{
        //    meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
        //    material = meshRenderer.sharedMaterial;
        //}

        //if(material != null){
        //    currentDuration = Time.deltaTime * lifeTime;
        //    material.color = Color.Lerp(material.color, Color.clear, currentDuration);
        //}

        yield return new WaitForSeconds(lifeTime);
        //ObjectPoolManager.Instance.Destroy(gameObject);
        Destroy(gameObject);
    }
}

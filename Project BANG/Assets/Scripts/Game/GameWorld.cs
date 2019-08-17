using UnityEngine;
using System;

public class GameWorld : MonoBehaviour
{
    [Serializable]
    private class GameWorldAssetContainer
    {
        public string layerName;
        public GameObject container;
    }

    [SerializeField]
    private GameWorldAssetContainer[] m_AssetContainers = {
        new GameWorldAssetContainer{ layerName = "Solid" },
        new GameWorldAssetContainer{ layerName = "Cover" },
        new GameWorldAssetContainer{ layerName = "Vault" }
    };


	private void Awake()
	{

        for (int index = 0; index < m_AssetContainers.Length; index++)
        {
            var container = m_AssetContainers[index].container;

            var layerIndex = container.layer;
            if(container.layer == 0){
                var layerName = m_AssetContainers[index].layerName;
                layerIndex = LayerMask.NameToLayer(layerName);
            }


            for (int i = 0; i < container.transform.childCount; i++)
            {
                if (container.transform.GetChild(i).gameObject.layer == 0)
                    container.transform.GetChild(i).gameObject.layer = layerIndex;
            }
        }

	}









}

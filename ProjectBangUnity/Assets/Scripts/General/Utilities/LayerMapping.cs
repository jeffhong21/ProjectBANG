namespace Bang
{
    using UnityEngine;

    /// <summary>
    /// Maps Unity layers to the internal <see cref="Layers"/>
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public sealed class LayerMapping : MonoBehaviour
    {
        public LayerMask worldLayer;
        public LayerMask hitableObjectsLayer;
        public LayerMask entitiesLayer;

        private void Awake()
        {
            Layers.worldObjects = worldLayer;
            Layers.hitableObjects = hitableObjectsLayer;
            Layers.entites = entitiesLayer;
        }
    }



    public static class Layers
    {
        public static LayerMask worldObjects;
        public static LayerMask hitableObjects;
        public static LayerMask entites;
    }


}



namespace Bang
{
    using UnityEngine;

    /// <summary>
    /// Maps Unity layers to the internal <see cref="Layers"/>
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public sealed class LayerMapping : MonoBehaviour
    {
        public LayerMask coverLayer;
        public LayerMask hitableObjectsLayer;
        public LayerMask entitiesLayer;

        private void Awake()
        {
            Layers.cover = coverLayer;
            Layers.hitableObjects = hitableObjectsLayer;
            Layers.entites = entitiesLayer;
        }
    }



    public static class Layers
    {
        public static LayerMask cover;
        public static LayerMask hitableObjects;   //  Used for projectiles
        public static LayerMask entites;
    }


}



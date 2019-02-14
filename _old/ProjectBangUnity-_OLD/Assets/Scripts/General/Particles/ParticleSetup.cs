namespace Bang
{
    using UnityEngine;
    using System;

    [Serializable]
    public class ParticleSetup
    {
        //  The tag is the identifier for the type of pool.
        public ParticlesType type;
        public GameObject prefab;
        [Range(0, 50)]
        public int initialInstanceCount = 18;
    }
}



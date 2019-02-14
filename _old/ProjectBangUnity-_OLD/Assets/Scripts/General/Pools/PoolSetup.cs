namespace Bang
{
    using UnityEngine;
    using System;

    [Serializable]
    public class PoolSetup
    {
        //  The tag is the identifier for the type of pool.
        //public string tag;
        public PoolTypes type;
        public string id;
        public GameObject prefab;
        [Range(0, 50)]
        public int initialInstanceCount = 18;
    }
}



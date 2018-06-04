namespace Bang
{
    using UnityEngine;
    using System;

    [Serializable]
    public class ActorPoolSetup
    {
        //  The tag is the identifier for the type of pool.
        //public string tag;
        public ActorTypes type;
        public GameObject prefab;
        [Range(0, 50)]
        public int initialInstanceCount = 18;
    }
}



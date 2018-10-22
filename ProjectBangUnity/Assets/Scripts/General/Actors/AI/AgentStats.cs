using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Bang
{
    [System.Serializable]
    public class AgentStats : ActorStats
    {

        [Range(0, 360)]
        public float fieldOfView = 170f;

        public float sightRange = 12;

        public float scanRadius = 10;
        [Tooltip("How often the Agent handles scanning, perception etc.")]
        public float checkRate = 1;

        public float shootSpeed = 1f;
        [Range(0, 3)]
        public float aimAccuracy = 3f;
    }
}
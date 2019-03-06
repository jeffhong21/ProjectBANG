using System;
using System.Collections.Generic;
using UnityEngine;
using uUtilityAI;

namespace CharacterController.AI
{
    [Serializable]
    public class AgentContext : IAIContext
    {
        [HideInInspector]
        public AgentController agent;
        [HideInInspector]
        public NavMeshAgentBridge navMeshAgent;

        public Transform target;

        public Vector3 destination;

        public bool hasDestination;

        public List<Vector3> sampledPositions = new List<Vector3>();
       
        public List<GameObject> hostiles = new List<GameObject>();
        [HideInInspector]
        public LayerMask hostilesLayer;



        public AgentContext()
        {
            
        }

        public AgentContext(AgentController a, NavMeshAgentBridge bridge)
        {
            agent = a;
            navMeshAgent = bridge;
            hostilesLayer = agent.GetComponent<LayerManager>().EnemyLayer;
        }







    }
}


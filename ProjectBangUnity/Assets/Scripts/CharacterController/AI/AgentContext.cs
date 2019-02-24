using System;
using System.Collections.Generic;
using UnityEngine;


namespace CharacterController.AI
{
    [Serializable]
    public class AgentContext 
    {

        public AgentController agent;

        public NavMeshAgentBridge navMeshAgent;

        public Transform target;

        public Vector3 destination;

        public bool hasDestination;

        public List<Vector3> sampledPositions = new List<Vector3>();
       
        public List<GameObject> entitites = new List<GameObject>();





        public AgentContext(){
            
        }

        public AgentContext(AgentController a, NavMeshAgentBridge bridge)
        {
            agent = a;
            navMeshAgent = bridge;

        }

        public void Initialize(AgentController a, NavMeshAgentBridge bridge)
        {
            agent = a;
            navMeshAgent = bridge;
        }






    }
}


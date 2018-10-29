﻿namespace Bang
{
    using UnityEngine;
    using UnityEngine.AI;
    using AtlasAI;


    public sealed class SearchForTargets : ActionBase
    {
        [SerializeField]
        public float sampleRange = 10;

        [SerializeField]
        public int samplePoints = 30;

        [SerializeField]
        public float maxDistance = 1;  //  Sample within this distance from sourcePosition.

        public float distanceForNewTarget = 5f;


        private Vector3 position;


        public override void Execute(IAIContext context)
        {
            var c = context as AgentContext;

            if (c.agent.IsSearching){
                float distance = Vector3.Distance(c.agent.position, position);
                if (distance <= 2f){
                    c.agent.IsSearching = false;
                }else{
                    return;
                }
            }


            for (int i = 0; i < samplePoints; i++)
            {
                position = new Vector3(Random.Range(-sampleRange, sampleRange), 0, Random.Range(-sampleRange, sampleRange));

                NavMeshHit hit;
                if (NavMesh.SamplePosition(position, out hit, this.maxDistance, NavMesh.AllAreas))
                {
                    position = hit.position;
                    c.agent.IsSearching = true;
                    c.agent.MoveTo(position);

                    Debug.LogFormat("{0} is searching for target at {1}", c.agent.gameObject.name, position);
                    return;
                }
            }





        }
    }
}
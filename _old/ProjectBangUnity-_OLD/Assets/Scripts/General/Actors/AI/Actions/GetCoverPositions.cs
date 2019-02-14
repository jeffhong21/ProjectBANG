namespace Bang
{
    using System.Collections.Generic;
    using UnityEngine;
    using AtlasAI;


    public class GetCoverPositions : ActionBase
    {


        public override void OnExecute(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;
            float scanRadius = agent.stats.scanRadius;

            List<Vector3> coverPositions = new List<Vector3>();


            Collider[] colliders = Physics.OverlapSphere(agent.position, scanRadius, Layers.cover);

            for (int i = 0; i < colliders.Length; i++)
            {
                var collider = colliders[i];

                if (collider == null || collider.gameObject == agent.gameObject){
                    continue;
                }


                if (collider.GetComponent<CoverObject>())
                {
                    CoverObject coverObject = colliders[i].GetComponent<CoverObject>();

                    for (int index = 0; index < coverObject.CoverSpots.Count; index++)
                    {
                        Vector3 position = coverObject.CoverSpots[index];
                        coverPositions.Add(position);
                    }
                }
            }

            c.CoverPositions = coverPositions;
        }
    }
}



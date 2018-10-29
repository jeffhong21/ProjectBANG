namespace Bang
{
    using System.Collections.Generic;

    using UnityEngine;
    using AtlasAI;

    public class FindCover : ActionBase
    {


        public override void Execute(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            float scanRadius = agent.stats.scanRadius;
            float mDist = float.MaxValue;
            CoverObject closestCover = null;
            List<Vector3> coverPositions = new List<Vector3>();


            Collider[] colliders = Physics.OverlapSphere(agent.position, scanRadius, Layers.cover);

            for (int i = 0; i < colliders.Length; i++)
            {
                var collider = colliders[i];

                if (collider == null || collider.gameObject == agent.gameObject)
                {
                    continue;
                }


                if (collider.GetComponent<CoverObject>())
                {
                    CoverObject coverObject = colliders[i].GetComponent<CoverObject>();

                    //  Get all cover positions from cover
                    for (int index = 0; index < coverObject.CoverSpots.Count; index++)
                    {
                        Vector3 position = coverObject.CoverSpots[index];
                        coverPositions.Add(position);
                    }

                    float tDist = Vector3.Distance(colliders[i].transform.position, agent.position);
                    if (tDist < mDist)
                    {
                        mDist = tDist;
                        closestCover = coverObject;
                    }
                }
            }

            c.CoverPositions = coverPositions;
            c.coverTarget = closestCover;
        }
    }
}



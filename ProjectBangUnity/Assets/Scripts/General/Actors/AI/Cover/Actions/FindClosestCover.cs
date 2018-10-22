﻿namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    public class FindClosestCover : ActionBase
    {


        public override void Execute(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;
            float scanRadius = agent.stats.scanRadius;


            Collider[] colliders = Physics.OverlapSphere(agent.position, scanRadius, Layers.cover);


            float mDist = float.MaxValue;
            CoverObject closestCover = null;

            for (int i = 0; i < colliders.Length;i ++)
            {
                var collider = colliders[i];

                if (collider == null){
                    continue;
                }

                if (collider.gameObject == agent.gameObject){
                    // ignore hits with self
                    continue;
                }



                if (collider.GetComponent<CoverObject>())
                {
                    CoverObject coverObject = colliders[i].GetComponent<CoverObject>();
                    float tDist = Vector3.Distance(colliders[i].transform.position, agent.position);

                    if (tDist < mDist)
                    {
                        mDist = tDist;
                        closestCover = coverObject;
                    }
                }


            }

            c.coverTarget = closestCover;
            //c.coverPosition = c.coverTarget.transform.position;
        }
    }
}


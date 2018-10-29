namespace Bang
{
    using System.Collections.Generic;
    using UnityEngine;
    using AtlasAI;


    public sealed class MoveToCover : ActionWithOptions<Vector3>
    {



        public override void Execute(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            //float scanRadius = agent.stats.scanRadius;
            //float mDist = float.MaxValue;
            //CoverObject closestCover = null;
            //List<Vector3> coverPositions = new List<Vector3>();


            //Collider[] colliders = Physics.OverlapSphere(agent.position, scanRadius, Layers.cover);

            //for (int i = 0; i < colliders.Length; i++)
            //{
            //    var collider = colliders[i];

            //    if (collider == null || collider.gameObject == agent.gameObject)
            //    {
            //        continue;
            //    }


            //    if (collider.GetComponent<CoverObject>())
            //    {
            //        CoverObject coverObject = colliders[i].GetComponent<CoverObject>();

            //        //  Get all cover positions from cover
            //        for (int index = 0; index < coverObject.CoverSpots.Length; index++)
            //        {
            //            Vector3 position = coverObject.CoverSpots[index];
            //            coverPositions.Add(position);
            //        }

            //        float tDist = Vector3.Distance(colliders[i].transform.position, agent.position);
            //        if (tDist < mDist)
            //        {
            //            mDist = tDist;
            //            closestCover = coverObject;
            //        }
            //    }
            //}

            //c.CoverPositions = coverPositions;
            //c.coverTarget = closestCover;





            Vector3 coverPosition = GetBest(c, c.CoverPositions);

            //  Move to the best position...
            if (coverPosition.sqrMagnitude == 0f){
                return;
            }

            c.coverPosition = coverPosition;

            //float distance = Vector3.Distance(c.coverPosition, agent.position);
            //if(distance <= 2)
            //{
            //    agent.EnterCover();
            //    return;
            //}
            //agent.MoveTo(coverPosition);



            if(c.coverTarget != null){
                //  if agent is within range of a cover spot.
                if(c.coverTarget.IsInRange(agent.transform)){
                    //  if agent cant take a spot.
                    if(c.coverTarget.TakeCoverSpot(agent.gameObject)){
                        agent.EnterCover(c.coverTarget);
                        return;
                    }
                }
                if(agent.States.InCover && c.coverTarget.IsInRange(agent.transform) == false){
                    c.coverTarget.LeaveCoverSpot(agent.gameObject);
                    agent.ExitCover();
                }
            }


            agent.MoveTo(coverPosition);
            //c.agent.GetComponent<PositionScoreVisualizer>().EntityUpdate(this, context);
        }





    }
}
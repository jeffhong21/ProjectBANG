namespace Bang
{
    using UnityEngine;
    using AtlasAI;


    public sealed class MoveToCoverPositions : ActionWithOptions<Vector3>
    {


        public override void Execute(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;


            Vector3 coverPosition = GetBest(c, c.CoverPositions);

            //  Move to the best position...
            if (coverPosition.sqrMagnitude == 0f){
                return;
            }

            c.coverPosition = coverPosition;


            if(c.coverTarget != null){
                //  if agent is within range of a cover spot.
                if(c.coverTarget.IsInRange(agent.transform)){
                    //  if agent cant take a spot.
                    if(c.coverTarget.TakeCoverSpot(agent.gameObject)){
                        agent.EnterCover(c.coverTarget);
                        return;
                    }
                }
                if(agent.IsInCover && c.coverTarget.IsInRange(agent.transform) == false){
                    c.coverTarget.LeaveCoverSpot(agent.gameObject);
                    agent.ExitCover();
                }
            }


            agent.MoveTo(coverPosition);
            //c.agent.GetComponent<PositionScoreVisualizer>().EntityUpdate(this, context);
        }





    }
}
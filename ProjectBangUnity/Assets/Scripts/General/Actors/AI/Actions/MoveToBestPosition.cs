namespace Bang
{
    using UnityEngine;
    using AtlasAI;


    public sealed class MoveToBestPosition : ActionWithOptions<Vector3>
    {


        public override void Execute(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;
            Vector3 best = GetBest(c, c.sampledPositions);

            //  Move to the best position...
            if (best.sqrMagnitude == 0f){
                return;
            }

            c._positionScores = scorers;


            c.agent.MoveTo(best);
            //c.agent.GetComponent<PositionScoreVisualizer>().EntityUpdate(this, context);
        }


  


    }
}
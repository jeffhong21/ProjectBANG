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
            Vector3 bestDestination = GetBest(c, c.sampledPositions);

            //  Move to the best position...
            if (bestDestination.sqrMagnitude == 0f)
            {
                Debug.Log("Did not get a best destination");
                return;
            }

            c.agent.MoveTo(bestDestination);

            //c.agent.GetComponent<PositionScoreVisualizer>().EntityUpdate(this, context);




        }


  


    }
}
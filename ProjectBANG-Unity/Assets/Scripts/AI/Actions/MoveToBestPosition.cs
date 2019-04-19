namespace CharacterController.AI
{
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;
    using uUtilityAI;


    public sealed class MoveToBestPosition : ActionWithOptions<Vector3>
    {


        public override void OnExecute(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;
            Vector3 best = GetBest(c, c.sampledPositions);

            //  Move to the best position...
            if (best.sqrMagnitude == 0f) return;
            

            //c.agent.MoveTo(best);


        }




    }
}
namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    public class StopMovement : ActionBase
    {


        public override void Execute(IAIContext context)
        {
            var c = context as AgentContext;
            c.agent.StopMoving();
        }

    }
}



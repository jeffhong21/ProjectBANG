namespace Bang
{
    using UnityEngine;
    using UtilityAI;

    public class StopMovement : ActionBase
    {


        public override void Execute(IAIContext context)
        {
            var c = context as AgentContext;
            c.agent.StopMoving();
        }

    }
}



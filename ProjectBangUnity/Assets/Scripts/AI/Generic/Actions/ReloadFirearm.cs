namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    public class ReloadFirearm : ActionBase
    {


        public override void Execute(IAIContext context)
        {
            var c = context as AgentContext;

            c.agent.Reload();
        }

    }
}



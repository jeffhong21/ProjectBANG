namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    public class ReloadGun : ActionBase
    {


        public override void Execute(IAIContext context)
        {
            var c = context as AgentContext;

            c.agent.Reload();
        }

    }
}



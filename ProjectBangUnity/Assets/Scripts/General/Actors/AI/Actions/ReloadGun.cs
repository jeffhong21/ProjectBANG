namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    public class ReloadGun : ActionBase
    {


        public override void OnExecute(IAIContext context)
        {
            var c = context as AgentContext;

            c.agent.Reload();
        }

    }
}



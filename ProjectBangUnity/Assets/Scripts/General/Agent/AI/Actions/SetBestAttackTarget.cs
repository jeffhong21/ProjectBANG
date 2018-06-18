namespace Bang
{
    using UnityEngine;
    using UtilityAI;

    public class SetBestAttackTarget : ActionWithOptions<IHasHealth>
    {

        public override void Execute(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            var enemies = c.hostiles;

            var best = this.GetBest(context, enemies);
            if (best != null)
            {
                // Set the attack target
                c.attackTarget = best;
                agent.OnAttackTargetChanged(c.attackTarget);
                //Debug.Log("FocusTarget is " + c.attackTarget);
            }
        }

    }
}



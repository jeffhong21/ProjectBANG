using UnityEngine;
using UnityEngine.AI;
using uUtilityAI;

namespace CharacterController.AI
{
    public sealed class SetBestAttackTarget : ActionBase
    {

        public override void OnExecute(IAIContext context)
        {
            //var c = context as AgentContext;
            //var agent = c.agent;

            //var hostiles = c.hostiles;

            //var best = this.GetBest(context, hostiles);
            //if (best != null || best != agent.transform)
            //{
            //    // Set the attack target
            //    c.attackTarget = best;
            //    agent.OnAttackTargetChanged(c.attackTarget);
            //    //Debug.Log("FocusTarget is " + c.attackTarget);
            //}
        }

    }
}
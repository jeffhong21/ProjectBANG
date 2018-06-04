namespace Bang
{
    using UnityEngine;
    using UtilityAI;

    public class FireAtAttackTarget : ActionBase
    {

        public override void Execute(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            var attackTarget = agent.attackTarget;

            if (attackTarget == null)
            {
                // no valid attack target to attack -> so return
                return;
            }

            //  TODO:  Get a Random Range of the target.
            //Debug.LogFormat("Firing at {0}", attackTarget.position);
            agent.FireWeapon(attackTarget.position);
        }
    }
}



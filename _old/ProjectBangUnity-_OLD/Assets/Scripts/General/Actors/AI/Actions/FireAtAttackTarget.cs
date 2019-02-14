namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    public class FireAtAttackTarget : ActionBase
    {

        public override void OnExecute(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            var attackTarget = c.attackTarget;

            if (attackTarget == null)
            {
                // no valid attack target to attack -> so return
                return;
            }

            //  TODO:  Get a Random Range of the target.
            //Debug.LogFormat("Firing at {0}", attackTarget.position);
            agent.ShootWeapon(attackTarget.transform.position);
        }
    }
}



namespace Bang
{
    using UnityEngine;
    using UtilityAI;


    public sealed class MoveToCover : ActionBase
    {


        public override void Execute(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            Collider col = agent.coverTarget;

            if (agent.coverTarget == null || agent.attackTarget == null)
                return;

            var currentCover = col;
            Vector3 dirToTarget = agent.attackTarget.position - col.transform.position;
            dirToTarget.Normalize();

            Vector3 targetPosition = col.transform.position + (dirToTarget * -1);

            //Debug.LogFormat("Position:  {0} | Cover Position: {1}", agent.position, col.transform.position);

            agent.agentInput.MoveTo(targetPosition);
        }


    }
}
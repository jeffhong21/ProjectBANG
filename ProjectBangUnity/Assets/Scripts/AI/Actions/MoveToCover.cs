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


            if (c.coverTarget == null || c.attackTarget == null)
                return;

            Collider col = c.coverTarget;

            Collider currentCover = col;
            Vector3 dirToTarget = c.attackTarget.position - col.transform.position;
            dirToTarget.Normalize();

            Vector3 targetPosition = col.transform.position + (dirToTarget * -1);

            //Debug.LogFormat("Position:  {0} | Cover Position: {1}", agent.position, col.transform.position);
            c.coverPosition = targetPosition;
            agent.MoveTo(targetPosition);
        }


    }
}
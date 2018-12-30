namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    public class MoveToPeekPosition : ActionBase
    {

        public override void OnExecute(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            if(c.coverTarget != null)
            {
                float distance = 0f;
                float range = c.coverTarget.EntitySize;
                float closest = float.MaxValue;
                Vector3? closestSpot = null;

                for (int i = 0; i < c.coverTarget.Corners.Length; i++)
                {
                    distance = Vector3.Distance(agent.position, c.coverTarget.Corners[i]);
                    if (distance < closest)
                    {
                        closest = distance;
                        closestSpot = c.coverTarget.Corners[i];
                    }
                }

                if (closestSpot != null)
                {
                    agent.MoveTo((Vector3)closestSpot);
                }
            }

        }
    }
}



namespace Bang
{
    using UnityEngine;
    using UtilityAI;

    /// <summary>
    /// Returns a score if there are any hostiles
    /// </summary>
    public class HasCoverPosition : ScorerBase
    {


        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;
            float scanRadius = agent.scanRadius;

            Collider[] colliders = Physics.OverlapSphere(agent.position, scanRadius, Layers.cover);


            if (colliders == null)
            {
                return 0f;
            }


            return this.score;
        }
    }
}

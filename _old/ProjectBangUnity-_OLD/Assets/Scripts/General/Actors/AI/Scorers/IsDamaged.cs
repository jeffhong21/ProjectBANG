namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    /// <summary>
    /// If agent has reached destination, than return score.
    /// </summary>
    public class IsDamaged : ContextualScorerBase
    {
        
        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            if (agent.IsUnderFire)
            {
                return this.score;
            }

            return 0f;
        }


    }
}
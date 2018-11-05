namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    /// <summary>
    /// Returns the score if agent has cover positions.
    /// </summary>
    public class HasCoverPosition : ContextualScorerBase
    {


        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;



            if (c.CoverPositions.Count == 0)
            {
                return 0f;
            }

            return this.score;
        }
    }
}

namespace Bang
{
    using UnityEngine;
    using UtilityAI;

    /// <summary>
    /// Will prioritize an entity that is alive.
    /// </summary>
    public sealed class IsTargetAlive : ScorerOptionBase<IHasHealth>
    {
        
        public float score = 100f;

        public override float Score(IAIContext context, IHasHealth hostile)
        {
            var c = context as AgentContext;

            if (hostile.currentHealth > 0)
            {
                return this.score;
            }

            return 0f;
        }
    }
}

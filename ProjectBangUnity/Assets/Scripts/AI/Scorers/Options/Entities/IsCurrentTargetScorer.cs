namespace Bang
{
    using UnityEngine;
    using UtilityAI;

    /// <summary>
    /// This is a tie breaker type scorer. 
    /// Returns a score for each hostile based on the distance minus the score.  (score is like a minimum range)
    /// </summary>
    public sealed class IsCurrentTargetScorer : ScorerOptionBase<IHasHealth>
    {
        public float multiplier = 1f;
        public float score = 1.5f;

        public override float Score(IAIContext context, IHasHealth hostile)
        {
            var c = context as AgentContext;

            var distance = (hostile.position - c.agent.position).magnitude;
            return Mathf.Max(0f, (distance - this.score) * this.multiplier);
        }
    }
}

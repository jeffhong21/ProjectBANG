namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    /// <summary>
    /// Returns a score for each hostile based on the distance minus the score.  (score is like a minimum range).
    /// The further away the Agent is from the player, the lower the score.
    /// </summary>
    public sealed class EnemyProximityToSelf : ScorerOptionBase<ActorHealth>
    {
        public float multiplier = 1f;
        public float score = 50f;

        public override float Score(IAIContext context, ActorHealth hostile)
        {
            var c = context as AgentContext;

            var distance = (hostile.position - c.agent.position).magnitude;
            return Mathf.Max(0f, (distance - this.score) * this.multiplier);
        }
    }
}

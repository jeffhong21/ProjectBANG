namespace Bang
{
    using UnityEngine;
    using UtilityAI;

    /// <summary>
    /// Score returned is the score minus the distance of the point * factor.
    /// </summary>
    public class PositionProximityToSelf : ScorerOptionBase<Vector3>
    {
        public float score = 10f;
        public float factor = 0.01f;

        public override float Score(IAIContext context, Vector3 position)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            var range = (position - agent.position).magnitude;
            return Mathf.Max(0f, (this.score - range) * factor);
        }
    }
}

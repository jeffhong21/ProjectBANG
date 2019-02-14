namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    /// <summary>
    /// Will prioritize an entity that is alive.
    /// </summary>
    public sealed class IsTargetAlive : OptionScorerBase<ActorHealth>
    {
        
        public float score = 100f;

        public override float Score(IAIContext context, ActorHealth hostile)
        {
            var c = context as AgentContext;

            if (hostile.CurrentHealth > 0){
                return this.score;
            }

            return 0f;
        }
    }
}

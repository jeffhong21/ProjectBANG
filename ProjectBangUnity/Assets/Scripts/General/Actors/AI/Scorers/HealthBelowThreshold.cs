namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    /// <summary>
    /// Returns a score if agent health is below a threshold.
    /// </summary>
    public class HealthBelowThreshold : ScorerBase
    {
        [SerializeField]
        public bool not = false;

        public float threshold = 2f;


        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            ActorHealth health = agent.GetComponent<ActorHealth>();
            var currentHealth = health.currentHealth;

            if (currentHealth < threshold)
            {
                return this.not ? this.score : 0f;  //  If "not" is false, it will return second option.  If true it will return first option.
            }

            return this.not ? 0f : this.score;  //  If "not" is false, it will return second option.  If true it will return first option.
        }


    }
}
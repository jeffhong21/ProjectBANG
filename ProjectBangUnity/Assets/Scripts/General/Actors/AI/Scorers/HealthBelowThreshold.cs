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

        public float threshold = 0.5f;


        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            ActorHealth health = agent.GetComponent<ActorHealth>();


            float percent = health.currentHealth / health.maxHealth;
            if (percent <= threshold)
            {
                return this.score;
            }

            return 0;
        }


    }
}
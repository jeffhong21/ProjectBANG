namespace Bang
{
    using UnityEngine;
    using UtilityAI;

    public class IsDamaged : ScorerBase
    {

        private float currentHealth;
        private float prevHealth;

        public bool not;

        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            currentHealth = agent.currentHealth;
            prevHealth = agent.currentHealth;

            if(currentHealth < prevHealth)
            {
                return this.score;
            }

            return 0f;
        }


    }
}
namespace Bang
{
    using UnityEngine;
    using UtilityAI;

    public class IsAiming : ScorerBase
    {
        [SerializeField]
        public bool not = false;

        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;


            if (c.agent.isAiming)
            {
                return this.not ? 0f : this.score;
            }

            return this.not ? this.score : 0f;
        }


    }
}
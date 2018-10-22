namespace Bang
{
    using UnityEngine;
    using AtlasAI;


    public class HasAttackTarget : ScorerBase
    {

        [SerializeField]
        public float score = 25;

        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            if (c.attackTarget == null)
            {
                return 0f;
            }

            return this.score;
        }
    }
}

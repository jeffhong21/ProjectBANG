namespace Bang
{
    using UnityEngine;
    using UtilityAI;

    public class IsAttackTargetAlive : ScorerBase
    {
        [SerializeField, Tooltip("Set to true to inverse the logic of this scorer, e.g. instead of scoring when true, it scores when false.")]
        public bool not;

        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;

            var attackTarget = c.attackTarget;
            if (attackTarget == null)
            {
                return 0f;
            }

            if (attackTarget.isDead == false)
            {
                return this.not ? 0f : this.score;
            }

            return this.not ? this.score : 0f;
        }


    }
}
namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    public class IsTargetInSight : ContextualScorerBase
    {
        [SerializeField]
        public bool not = false;  //  If false, it will return second option.  If true it will return first option.


        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            //RaycastHit hit;
            //if (Physics.Raycast(agent.AimOrigin, c.attackTarget.position, out hit, agent.stats.scanRadius, agent.targetLayerMask))
            //{
            //      return this.not ? 0f : this.score;  //  If "not" is false, it will return second option.  If true it will return first option.
            //}



            return this.not ? this.score : 0f;  //  If "not" is false, it will return second option.  If true it will return first option.
        }
    }
}
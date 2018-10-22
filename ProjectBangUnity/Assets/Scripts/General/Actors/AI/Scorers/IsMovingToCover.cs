namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    /// <summary>
    /// Returns the score if agent is moving to cover.
    /// </summary>
    public class IsMovingToCover : ScorerBase
    {
        [SerializeField]
        public bool not = false;  


        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            if (c.agent.IsMovingToCover)
            {
                return this.not ? this.score : 0f;  //  If "not" is false, it will return second option.  If true it will return first option.
            }
            return this.not ? 0f : this.score;  //  If "not" is false, it will return second option.  If true it will return first option.
        }


    }
}
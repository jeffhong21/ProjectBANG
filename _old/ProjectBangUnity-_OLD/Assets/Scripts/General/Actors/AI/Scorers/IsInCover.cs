namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    /// <summary>
    /// Returns score if agent is in cover.
    /// </summary>
    public class IsInCover : ContextualScorerBase
    {
        [SerializeField]
        public bool not = false;

        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            //if (agent.States.InCover)
            //{
            //    return this.not ? 0f : this.score;  //  If "not" is false, it will return second option.  If true it will return first option.
            //}

            Debug.Log("***** Removed InCover State. *****");

            return this.not ? this.score : 0f;  //  If "not" is false, it will return second option.  If true it will return first option.
        }


    }
}
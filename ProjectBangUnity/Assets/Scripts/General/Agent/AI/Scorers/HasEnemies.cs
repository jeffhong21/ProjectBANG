namespace Bang
{
    using UnityEngine;
    using UtilityAI;

    /// <summary>
    /// Returns a score if there are any hostiles
    /// </summary>
    public class HasEnemies : ScorerBase
    {
        [SerializeField]
        public bool not = false;  //  If false, it will return second option.  If true it will return first option.
        
        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            if (c.hostiles.Count == 0)
            {
                //  returns 0 if "not" is false or returns the score if "not" is true.
                return this.not ? this.score : 0f;
            }

            return this.not ? 0f : this.score;

        }
    }
}

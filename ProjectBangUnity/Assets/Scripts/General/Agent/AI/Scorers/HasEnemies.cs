namespace Bang
{
    using UnityEngine;
    using UtilityAI;

    /// <summary>
    /// Returns a score if there are any hostiles
    /// </summary>
    public class HasEnemies : ScorerBase
    {
        
        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            if (c.hostiles.Count == 0)
            {
                //Debug.Log("No hostiles");
                return 0f;
            }

            return this.score;
        }
    }
}

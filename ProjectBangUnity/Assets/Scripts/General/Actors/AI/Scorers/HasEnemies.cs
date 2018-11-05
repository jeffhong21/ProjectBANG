﻿namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    /// <summary>
    /// Returns a score if there are any hostiles
    /// </summary>
    public class HasEnemies : ContextualScorerBase
    {
        [SerializeField]
        public bool not = false;  //  If false, it will return second option.  If true it will return first option.
        
        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            if (c.hostiles.Count == 0)
            {
                return this.not ? this.score : 0f;  //  If "not" is false, it will return second option.  If true it will return first option.
            }

            return this.not ? 0f : this.score;  //  If "not" is false, it will return second option.  If true it will return first option.

        }
    }
}

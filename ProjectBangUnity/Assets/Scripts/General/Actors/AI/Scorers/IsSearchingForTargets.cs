﻿namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    /// <summary>
    /// Returns score if agent is searching for targets.
    /// </summary>
    public class IsSearchingForTargets : ScorerBase
    {
        [SerializeField]
        public bool not = false;

        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;


            if (c.agent.IsSearching)
            {
                return this.not ? 0f : this.score;  //  If "not" is false, it will return second option.  If true it will return first option.
            }

            return this.not ? this.score : 0f;  //  If "not" is false, it will return second option.  If true it will return first option.
        }


    }
}
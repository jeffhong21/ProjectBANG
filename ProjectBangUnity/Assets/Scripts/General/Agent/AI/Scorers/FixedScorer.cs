﻿namespace Bang
{
    using UnityEngine;
    using UtilityAI;

    public class FixedScorer : ScorerBase
    {
        
        public override float Score(IAIContext context)
        {
            return this.score;
        }


    }
}
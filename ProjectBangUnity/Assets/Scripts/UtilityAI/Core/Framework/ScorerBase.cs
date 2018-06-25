namespace UtilityAI
{
    using UnityEngine;
    using System;

    [Serializable]
    public abstract class ScorerBase : IScorer
    {
        public int score;


        protected ScorerBase(){

        }

        public abstract float Score(IAIContext context);

    }
}
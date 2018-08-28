namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    public class FixedScorer : ScorerBase
    {
        
        public override float Score(IAIContext context)
        {
            return this.score;
        }


    }
}
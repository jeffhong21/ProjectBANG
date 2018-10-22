namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    public class RandomScore : ScorerBase
    {

        public override float Score(IAIContext context)
        {
            return this.score;
        }


    }
}
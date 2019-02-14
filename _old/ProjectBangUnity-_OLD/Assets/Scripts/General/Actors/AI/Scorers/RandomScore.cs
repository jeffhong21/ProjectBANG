namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    public class RandomScore : ContextualScorerBase
    {

        public override float Score(IAIContext context)
        {
            return this.score;
        }


    }
}
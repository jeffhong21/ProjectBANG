namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    public class ShouldFlee : ScorerBase
    {

        public override float Score(IAIContext context)
        {
            return this.score;
        }


    }
}
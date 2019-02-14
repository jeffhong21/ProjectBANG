namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    public class ShouldFlee : ContextualScorerBase
    {

        public override float Score(IAIContext context)
        {
            return this.score;
        }


    }
}
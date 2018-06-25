namespace Bang
{
    using UnityEngine;
    using UtilityAI;

    public class ShouldFlee : ScorerBase
    {

        public override float Score(IAIContext context)
        {
            return this.score;
        }


    }
}
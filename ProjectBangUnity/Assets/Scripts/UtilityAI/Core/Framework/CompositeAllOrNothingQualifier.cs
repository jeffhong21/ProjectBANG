namespace UtilityAI
{

    using System;
    using System.Collections.Generic;


    [AICategory("Composite Qualifiers"), FriendlyName("All or Nothing", "Only scores if all child scorers score above the threshold.")]
    public class CompositeAllOrNothingQualifier : CompositeQualifier
    {

        public float threshold;

        //
        // Constructors
        //
        public CompositeAllOrNothingQualifier()
            : base()
        {
        }

        //
        // Methods
        //
        public override float Score(IAIContext context, List<IScorer> scorers)
        {
            var score = 0f;
            if (scorers.Count == 0)
                return score;


            foreach (IScorer scorer in scorers)
            {
                score += scorer.Score(context);
            }

            _score = score;  //  For CompareTo()

            if(score >= threshold)
            {
                return score;
            }

            return 0f;
        }
    }
}

namespace UtilityAI
{

    using System.Collections.Generic;



    [AICategory ("Composite Qualifiers"), FriendlyName ("Sum of Children", "Scores by summing the score of all child scorers.")]
    public class CompositeScoreQualifier : CompositeQualifier
    {



        //
        // Constructors
        //
        public CompositeScoreQualifier()
            : base()
        {
        }


        public override float Score(IAIContext context, List<IScorer> scorers)
        {
            var score = 0f;
            if (scorers.Count == 0)
                return score;
            
            //Debug.Log(this.GetType().ToString() + " is scoring");

            foreach (IScorer scorer in scorers)
            {
                score += scorer.Score(context);
            }
            _score = score;  //  For CompareTo()


            return score;
        }
    }











}


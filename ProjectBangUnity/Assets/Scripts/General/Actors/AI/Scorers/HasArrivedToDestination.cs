namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    /// <summary>
    /// If agent has reached destination, than return score.
    /// </summary>
    public class HasArrivedToDestination : ScorerBase
    {
        [SerializeField]
        public float arrivalDistance = 1f;

        [SerializeField]
        public bool not = false; 

        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;

            if(c.navMeshAgent.path.corners != null)
            {
                float distance = 0.0f;
                Vector3[] corners;
                corners = c.navMeshAgent.path.corners;


                for (int i = 0; i < corners.Length - 1; i++)
                {
                    distance += Mathf.Abs((corners[i] - corners[i + 1]).magnitude);
                }


                if (distance <= arrivalDistance)
                {
                    //  Agent is at destination.
                    return this.not ? 0f : this.score;  //  If "not" is false, it will return second option.  If true it will return first option.
                }
            }

            //  Agent is not at destination
            return this.not ? this.score : 0f;  //  If "not" is false, it will return second option.  If true it will return first option.
        }


    }
}
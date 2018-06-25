namespace Bang
{
    using UnityEngine;
    using UtilityAI;

    /// <summary>
    /// If agent has reached destination, than return score.
    /// </summary>
    public class HasArrivedToDestination : ScorerBase
    {
        [SerializeField]
        public float arrivalDistance = 1f;

        [SerializeField]
        public bool not = false;  //  If false, it will return second option.  If true it will return first option.

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
                    return this.not ? 0f : this.score;
                }
            }


            return this.not ? this.score : 0f;
        }


    }
}
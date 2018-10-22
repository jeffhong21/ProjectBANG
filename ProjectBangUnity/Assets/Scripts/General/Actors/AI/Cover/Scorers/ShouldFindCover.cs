namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    /// <summary>
    /// Returns a score if the agent doesn't have a cover target or if the agent is close to one.
    /// </summary>
    public class ShouldFindCover : ScorerBase
    {

        [SerializeField]
        public float inCoverRange = 2;



        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;

            //  If no coverTarget, need to find cover, so return score.
            if (c.coverTarget == null)
            {
                return this.score;
            }

            //  Agent has a coverTarget, so checking if agent is within range of the coverTarget.
            float distanceToCover = Mathf.Abs((c.agent.position - c.coverTarget.GetComponent<Collider>().ClosestPoint(c.agent.position)).sqrMagnitude);


            if(distanceToCover > inCoverRange * inCoverRange )
            {
                return this.score;
            }


            //  Agent is within range of a coverTArget, so do not need to find cover.
            return 0f;
        }


    }
}
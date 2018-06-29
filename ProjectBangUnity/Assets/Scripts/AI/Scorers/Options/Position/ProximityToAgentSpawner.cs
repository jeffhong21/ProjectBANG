namespace Bang
{
    using UnityEngine;
    using UtilityAI;

    /// <summary>
    /// Score returned is the score minus the distance of the point * factor.
    /// The Agent will prefer to stay close to where he spawned if nothing else has happened.
    /// This will ensure he will move to his original position so he has the most options to move.
    /// </summary>
    public class ProximityToAgentSpawner : ScorerOptionBase<Vector3>
    {
        public float score = 100f;
        public float multiplier = 1f;

        public override float Score(IAIContext context, Vector3 position)
        {
            //var c = context as AgentContext;


            //var range = (position - c.agent.spawnPoint).magnitude;
            //return Mathf.Max(0f, (this.score - range) * multiplier);
            Debug.Log("ProximityToAgentSpawner returns 0");
            return 0;
        }
    }
}

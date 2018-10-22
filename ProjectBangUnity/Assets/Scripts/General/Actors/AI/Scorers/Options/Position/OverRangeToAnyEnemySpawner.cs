namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    /// <summary>
    /// Score returned is the score minus the distance of the point * factor.
    /// Ensures the Agent will not move towards undesirable locations.
    /// </summary>
    public class OverRangeToAnyEnemySpawner : ScorerOptionBase<Vector3>
    {
        
        public float multiplier = 1f;

        public float score = 100f;

        public override float Score(IAIContext context, Vector3 position)
        {
            //var c = context as AgentContext;


            //var range = (position - c.agent.spawnPoint).magnitude;
            //return Mathf.Max(0f, (this.score - range) * multiplier);

            return 0;
        }
    }
}

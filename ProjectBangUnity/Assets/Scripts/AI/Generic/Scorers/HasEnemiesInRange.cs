namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    /// <summary>
    /// Returns the score if a hostile is within range.
    /// </summary>
    public class HasEnemiesInRange : ScorerBase
    {

        public float range = 10f;  //  entites within this range


        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;



            var hostiles = c.hostiles;
            var count = hostiles.Count;

            for (int i = 0; i < count; i++)
            {
                var enemy = hostiles[i];
                var sqrDist = (enemy.position - c.agent.position).sqrMagnitude;

                //  If enemy is within range.
                if (sqrDist <= range * range)
                    return score;
            }

            return 0f;

        }
    }
}
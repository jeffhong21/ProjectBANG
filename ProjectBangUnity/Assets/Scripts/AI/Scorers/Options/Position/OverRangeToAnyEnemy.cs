namespace Bang
{
    using UnityEngine;
    using UtilityAI;


    /// <summary>
    /// Returns a score if any hostile entity is over desired range.
    /// </summary>
    public sealed class OverRangeToAnyEnemy : ScorerOptionBase<Vector3>
    {
        
        public float desiredRange = 5f;

        public float score = 50f;


        public override float Score(IAIContext context, Vector3 position)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            var enemies = c.hostiles;
            var count = enemies.Count;
            if (count == 0)
            {
                return 0f;
            }

            var sqrDesiredRange = desiredRange * desiredRange;
            for (int i = 0; i < count; i++)
            {
                var enemy = enemies[i];

                var dirPlayerToEnemy = (enemy.position - agent.transform.position);
                var dirPositionToEnemy = (enemy.position - position);

                //all positions behind the enemy or closer than the desired range are not of interest
                if (Vector3.Dot(dirPlayerToEnemy, dirPositionToEnemy) < 0f || dirPositionToEnemy.sqrMagnitude < sqrDesiredRange)
                {
                    return 0f;
                }
            }

            return this.score;
        }


    }
}
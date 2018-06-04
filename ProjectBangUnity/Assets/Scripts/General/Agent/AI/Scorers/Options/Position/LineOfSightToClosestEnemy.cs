namespace Bang
{
    using UnityEngine;
    using UtilityAI;

    /// <summary>
    /// Returns a score if position is within line of sight of the closest enemy.
    /// </summary>
    public class LineOfSightToClosestEnemy : ScorerOptionBase<Vector3>
    {
        public float score = 50f;
        public float YHeightOffset = 0.5f;

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

            var nearest = Vector3.zero;
            var shortest = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                var enemy = enemies[i];

                var distance = (agent.position - enemy.position).sqrMagnitude;
                if (distance < shortest)
                {
                    shortest = distance;
                    nearest = enemy.position;
                }
            }

            var dir = (nearest - position);
            var range = dir.magnitude;
            var ray = new Ray(position + Vector3.up * YHeightOffset, dir);

            if (!Physics.Raycast(ray, range, Layers.worldObjects))
            {
                return this.score;
            }

            return 0f;
        }
    }
}

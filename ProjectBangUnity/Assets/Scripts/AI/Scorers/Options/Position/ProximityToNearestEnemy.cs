namespace Bang
{
    using UnityEngine;
    using UtilityAI;

    /// <summary>
    /// Score returned is the score minus the distance to the nearest enemy minus range.
    /// </summary>
    public class ProximityToNearestEnemy : ScorerOptionBase<Vector3>
    {
        
        public float desiredRange = 14f;

        public float score = 50f;


        public override float Score(IAIContext context, Vector3 position)
        {
            var c = context as AgentContext;


            var enemies = c.hostiles;
            var count = c.hostiles.Count;
            if (count == 0)
            {
                return 0f;
            }

            var nearest = Vector3.zero;
            var shortest = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                var enemy = enemies[i];

                var distance = (position - enemy.position).sqrMagnitude;
                if (distance < shortest)
                {
                    shortest = distance;
                    nearest = enemy.position;
                }
            }

            if (nearest.sqrMagnitude == 0f)
            {
                return 0f;
            }

            var range = (position - nearest).magnitude;
            return Mathf.Max(0f, (this.score - Mathf.Abs(this.desiredRange - range)));
        }
    }
}

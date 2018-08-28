namespace Bang
{
    using UnityEngine;
    using AtlasAI;


    /// <summary>
    /// Returns a score if the closest hostile entity is over desired range.
    /// </summary>
    public sealed class OverRangeToClosestEnemy : ScorerOptionBase<Vector3>
    {
        
        public float desiredRange = 5f;

        public float score = 100f;


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

                var distance = (position - enemy.gameObject.transform.position).sqrMagnitude;
                if (distance < shortest)
                {
                    shortest = distance;
                    nearest = enemy.gameObject.transform.position;
                }
            }


            var range = (position - nearest).magnitude;

            if (range > desiredRange){
                return this.score;
            }
            else{
                return 0;
            }
        }


    }
}
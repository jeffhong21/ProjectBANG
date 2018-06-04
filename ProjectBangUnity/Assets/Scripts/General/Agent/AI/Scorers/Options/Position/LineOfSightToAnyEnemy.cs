﻿namespace Bang
{
    using UnityEngine;
    using UtilityAI;

    /// <summary>
    /// Returns a score if position is within line of sight any enemy.
    /// </summary>
    public class LineOfSightToAnyEnemy : ScorerOptionBase<Vector3>
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

            for (int i = 0; i < count; i++)
            {
                var enemy = enemies[i];
                var dir = enemy.position - position;
                var range = dir.magnitude;
                var ray = new Ray(position + Vector3.up * YHeightOffset, dir);
                //Debug.DrawRay(position + Vector3.up * YHeightOffset, dir, Color.red, 1f);
                if (Physics.Raycast(ray, range, Layers.worldObjects) == false)
                {
                    return this.score;
                }
            }

            return 0f;
        }
    }
}

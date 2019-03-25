﻿using UnityEngine;
using UnityEngine.AI;
using uUtilityAI;

namespace CharacterController.AI
{

    /// <summary>
    /// Returns a score if position is within line of sight of the closest enemy.
    /// </summary>
    public class LineOfSightToClosestEnemy : OptionScorerBase<Vector3>
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

                var distance = (agent.Position - enemy.transform.position).sqrMagnitude;
                if (distance < shortest)
                {
                    shortest = distance;
                    nearest = enemy.transform.position;
                }
            }

            var dir = (nearest - position);
            var range = dir.magnitude;
            var ray = new Ray(position + Vector3.up * YHeightOffset, dir);

            if (Physics.Raycast(ray, range, c.hostilesLayer))
            {
                return this.score;
            }

            return 0f;
        }
    }
}
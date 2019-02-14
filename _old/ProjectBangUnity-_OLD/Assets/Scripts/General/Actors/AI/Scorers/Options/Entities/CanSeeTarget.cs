﻿namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    /// <summary>
    /// Returns a score for each hostile based on the distance minus the score.  (score is like a minimum range).
    /// The further away the Agent is from the player, the lower the score.
    /// </summary>
    public sealed class CanSeeTarget : OptionScorerBase<ActorHealth>
    {
        
        public float score = 50f;


        public override float Score(IAIContext context, ActorHealth hostile)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            bool canSeeTarget = false;

            if(c.attackTarget != null){
                canSeeTarget = agent.CanSeeTarget(agent.lookTransform.position, c.attackTarget.position);
            }

            if (canSeeTarget){
                return this.score;
            }
            return 0f;
        }



    }
}
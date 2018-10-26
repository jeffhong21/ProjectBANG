namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    /// <summary>
    /// Returns the score if agent can see any enemy within range.  If "not" is true, than will return this score if 
    /// </summary>
    public class CanSeeEnemiesInRange : ScorerBase
    {
        public bool not = false;

        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            var hostiles = c.hostiles;
            var count = hostiles.Count;

            bool canSeeAnyTarget = false;
            float range = agent.stats.sightRange;  //  entites within this range


            for (int i = 0; i < count; i++)
            {
                var enemy = hostiles[i];
                var sqrDist = (enemy.position - c.agent.position).sqrMagnitude;

                //  If enemy is within range.
                if (sqrDist <= range * range){
                    if(agent.CanSeeTarget(enemy.transform)){
                        canSeeAnyTarget = true;
                        //  If we are checking to see if we can see any targets, break out of the loop.
                        if (not == false)
                            break;
                    }
                }
            }


            if(canSeeAnyTarget){
                return this.not ? 0f : this.score;  //  If "not" is false, it will return second option.  If true it will return first option.
            }
            //  No targets can be seen.
            return this.not ? this.score : 0f;  //  If "not" is false, it will return second option.  If true it will return first option.
        }
    }
}
namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    /// <summary>
    /// Returns score if gun has no ammo.  If not is set to true, it will return 0 if gun has no ammo.
    /// </summary>
    public class IsGunLoaded : ScorerBase
    {


        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            if (agent.weapon != null){
                //  If weapon no ammo, return score (or 0 if not is true).

                if (agent.weapon.CurrentAmmo <= 0){
                    
                    return this.score;
                }
            }

            return 0f;
        }
    }
}
namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    /// <summary>
    /// Returns a score if agent health is below a threshold.
    /// </summary>
    public class AmmoBelowThreshold : ScorerBase
    {

        public float threshold = 0.5f;


        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            var weapon = agent.weapon;

            if(weapon != null)
            {
                float percent = weapon.CurrentAmmo / weapon.MaxAmmo;
                if(percent <= threshold)
                {
                    return this.score;
                }
            }

            return 0;
        }


    }
}
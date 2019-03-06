using UnityEngine;
using UnityEngine.AI;
using uUtilityAI;

namespace CharacterController.AI
{
    /// <summary>
    /// Returns the score if agent does not have a destination.
    /// </summary>
    public class CanMoveToNewPosition : ContextualScorerBase
    {

        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;

            if(c.hasDestination)
            {
                //  Agent has a destination, but has reached the destination.
                if(c.navMeshAgent.HasReachedDestination()){
                    c.hasDestination = false;
                    return score;
                }
                return 0;
            }

            //  Agent has no destination.  Return score.
            return score;
        }
    }
}

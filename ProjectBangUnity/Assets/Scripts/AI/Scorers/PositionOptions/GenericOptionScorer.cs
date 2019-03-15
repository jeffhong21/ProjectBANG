using UnityEngine;
using UnityEngine.AI;
using uUtilityAI;

namespace CharacterController.AI
{
    public class GenericOptionScorer : OptionScorerBase<Vector3>
    {


        public override float Score(IAIContext context, Vector3 position)
        {
            //var c = context as AgentContext;

            return 0;
        }
    }
}

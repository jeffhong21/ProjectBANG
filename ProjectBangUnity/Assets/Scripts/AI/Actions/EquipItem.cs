using UnityEngine;
using UnityEngine.AI;
using uUtilityAI;

namespace CharacterController.AI
{
    public sealed class EquipItem : ActionBase
    {

        public override void OnExecute(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;


        }

    }
}
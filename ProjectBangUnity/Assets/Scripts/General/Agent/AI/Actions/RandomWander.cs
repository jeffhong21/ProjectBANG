namespace Bang
{
    using UnityEngine;
    using UtilityAI;

    public class RandomWander : ActionBase
    {
        [SerializeField]
        float randomWanderRadius = 5f;


        public override void Execute(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            var pos = agent.transform.position + (UnityEngine.Random.onUnitSphere.normalized * randomWanderRadius);
            pos.y = agent.position.y;

            Debug.Log("RandomWander position:  " + pos);
            agent.agentInput.MoveTo(pos);
        }
    }
}



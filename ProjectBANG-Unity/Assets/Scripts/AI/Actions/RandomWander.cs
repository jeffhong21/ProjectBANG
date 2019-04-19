using UnityEngine;
using UnityEngine.AI;
using uUtilityAI;

namespace CharacterController.AI
{
    public sealed class RandomWander : ActionBase
    {
        
        public float range = 20;


        public override void OnExecute(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            NavMeshHit navHit;
            Vector3 randomPoint = agent.Position + (Random.onUnitSphere.normalized * range);
            randomPoint.y = 0;


            if (NavMesh.SamplePosition(randomPoint, out navHit, 5f, NavMesh.AllAreas)){
                c.destination = navHit.position;
                c.hasDestination = true;
                c.navMeshAgent.SetDestination(c.destination);

                //agent._SetAgentMoveType();
            } 
            else{
                c.destination = agent.Position;
                c.hasDestination = false;
            }


            //DebugAction(agent.gameObject);
        }


        public void DebugAction(GameObject agent)
        {
            Debug.LogFormat("** {0} | {1} is executing <color=blue>{2}</color>  ",Time.timeSinceLevelLoad, agent.name, GetType().Name);
        }
    }
}
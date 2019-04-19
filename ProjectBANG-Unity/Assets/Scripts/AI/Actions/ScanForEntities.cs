using UnityEngine;
using UnityEngine.AI;
using uUtilityAI;

namespace CharacterController.AI
{
    public class ScanForEntities : ActionBase
    {
        
        private float sightRange = 20;
        private float fieldOfView = 135;


        public override void OnExecute(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            sightRange = agent.SightRange;
            fieldOfView = agent.FieldOfView;

            c.hostiles.Clear();

            // Use OverlapSphere for getting all relevant colliders within scan range, filtered by the scanning layer
            var colliders = Physics.OverlapSphere(agent.Position, sightRange, c.hostilesLayer);
            //Physics.OverlapSphereNonAlloc(agent.Position, sightRange, agent.colliders, c.hostilesLayer);


            for (int i = 0; i < colliders.Length; i++)
            {
                Collider hit = colliders[i];
                // ignore hits with self
                if (hit == null || hit.gameObject == agent.gameObject){
                    continue;
                }

                if (hit.GetComponent<CharacterHealth>())
                {
                    bool canSeeTarget = agent.CanSeeTarget(agent.LookTransform.position, hit.transform.position + Vector3.up);
                    if (canSeeTarget){
                        c.hostiles.Add(hit.gameObject);
                    }

                }
            }
        }



    }
}
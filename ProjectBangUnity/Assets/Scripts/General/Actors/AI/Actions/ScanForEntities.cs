namespace Bang
{
    using UnityEngine;
    using AtlasAI;


    /// <summary>
    /// Scans for any Actors.
    /// </summary>

    public class ScanForEntities : ActionBase
    {
        [SerializeField]
        public float samplingRange = 20;
        [SerializeField]
        public float samplingDensity = 3f;



        public override void Execute(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;
            float sightRange = agent.stats.sightRange;
            float fieldOfView = agent.stats.fieldOfView;

            c.hostiles.Clear();

            // Use OverlapSphere for getting all relevant colliders within scan range, filtered by the scanning layer
            var colliders = Physics.OverlapSphere(agent.transform.position, sightRange, Layers.entites);

            c.hostiles.Clear();
            for (int i = 0; i < colliders.Length; i++)
            {
                Collider hit = colliders[i];

                if (hit == null){
                    continue;
                }

                // ignore hits with self
                if (hit.gameObject == agent.gameObject){
                    continue;
                }


                if (hit.GetComponent<ActorController>().teamId != agent.teamId)
                {
                    bool canSeeTarget = agent.CanSeeTarget(agent.lookTransform.position, hit.transform.position, true);

                    if(canSeeTarget)
                    {
                        c.hostiles.Add(hit.GetComponent<ActorHealth>());
                    }

                    //c.hostiles.Add(hit.GetComponent<ActorHealth>());
                    agent.IsSearching = false;
                }
            }
        }


    }
}
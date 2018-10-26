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
        [SerializeField]
        public float scanRange = 20f;
        [SerializeField]
        public string entityTag = Tags.Actor;
        [SerializeField]
        public bool useContext;

        public override void Execute(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;

            c.hostiles.Clear();

            // Use OverlapSphere for getting all relevant colliders within scan range, filtered by the scanning layer
            var colliders = Physics.OverlapSphere(agent.transform.position, 8, Layers.entites);

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

                if (hit.CompareTag(entityTag))
                {
                    c.hostiles.Add(hit.GetComponent<ActorHealth>());
                }
            }




        }


    }
}
namespace Bang
{
    using UnityEngine;
    using AtlasAI;

    /// <summary>
    /// Returns a score if there are any hostiles
    /// </summary>
    public class HasCoverPosition : ScorerBase
    {

        public float range = 10f;

        //Collider[] colliderBuffer = new Collider[10];

        public override float Score(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;
            float scanRadius = agent.scanRadius;

            //Physics.OverlapSphereNonAlloc(agent.position, scanRadius, colliderBuffer, Layers.cover);

            Collider[] colliders = Physics.OverlapSphere(agent.position, scanRadius, Layers.cover);

            //Debug.Log(colliderBuffer);

            if (colliders == null)
            {
                return 0f;
            }


            return this.score;
        }
    }
}

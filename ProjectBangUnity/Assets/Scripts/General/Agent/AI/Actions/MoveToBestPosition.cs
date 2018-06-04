namespace Bang
{
    using UnityEngine;
    using UtilityAI;


    public sealed class MoveToBestPosition : ActionWithOptions<Vector3>
    {


        public override void Execute(IAIContext context)
        {
            var c = context as AgentContext;
            var agent = c.agent;
            Vector3 bestDestination = GetBest(c, c.sampledPositions);

            //  Move to the best position...
            if (bestDestination.sqrMagnitude == 0f)
            {
                Debug.Log("Did not get a best destination");
                return;
            }

            c.agent.MoveTo(bestDestination);
            //if (Mathf.Abs(bestDestination.sqrMagnitude) > Mathf.Abs(c.agent.transform.position.sqrMagnitude))
            //{
            //    //Debug.Log(string.Format("Entity position:  <{0}>  |  Entity sqrMagnitude:  {1}\nBestDestination position:  <{2}>  |  BestDestination sqrMagnitude:  {3}", 
            //    //c.agent.transform.position, Mathf.Abs(c.agent.transform.position.sqrMagnitude),bestDestination, Mathf.Abs(bestDestination.sqrMagnitude)));

            //    c.agent.MoveTo(bestDestination);
            //}
            //else
            //{
            //    //Debug.Log(string.Format("Entity position:  {0}\nBest Destination: {1}", c.agent.transform.position, bestDestination));
            //    return;
            //}

        }


  


    }
}
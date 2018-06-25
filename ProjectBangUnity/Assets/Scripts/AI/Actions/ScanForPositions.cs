namespace Bang
{
    using UnityEngine;
    using UnityEngine.AI;
    using UtilityAI;

    /// <summary>
    /// Scans for all availible positions
    /// </summary>
    public class ScanForPositions : ActionBase
    {
        //  "Sampling Range", "How large a range points are sampled in, in a square with the entity in the center"
        public float samplingRange = 20f;
        //  "Sampling Density", "How much distance there is between individual samples"
        public float samplingDensity = 2.5f;



        public override void Execute(IAIContext context)
        {

            var c = context as AgentContext;


            c.sampledPositions.Clear();

            var halfSamplingRange = this.samplingRange * 0.5f;
            var pos = c.agent.transform.position;

            // nested loop in x and z directions, starting at negative half sampling range and ending at positive half sampling range, thus sampling in a square around the entity
            for (var x = -halfSamplingRange; x < halfSamplingRange; x += this.samplingDensity)
            {
                for (var z = -halfSamplingRange; z < halfSamplingRange; z += this.samplingDensity)
                {
                    var p = new Vector3(pos.x + x, 0f, pos.z + z);

                    // Sample the position in the navigation mesh to ensure that the desired position is actually walkable
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(p, out hit, this.samplingDensity * 0.5f, NavMesh.AllAreas))
                    {
                        c.sampledPositions.Add(hit.position);
                    }
                }
            }


            //Debug.Log(c.sampledPositions.Count);
        }


        /*
         ( (samplingRange / 2) / samplingDensity ) * 2 = the amount of position points on one side.
         samplingRange = 12f;
         samplingDensity = 1.5f;
         ( (12 / 2) / 1.5 ) * 2 = 4 for left side.
         4 points on left plus 4 on points on right = 8 for one side.
         8 * 8 = total points in square.
         
         To find Sampling density:
         samplingDensity * samplingDensity = range * range / totalPointCount.
        */

    }
}
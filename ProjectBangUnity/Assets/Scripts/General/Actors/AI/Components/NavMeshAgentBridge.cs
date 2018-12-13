namespace Bang
{
    using UnityEngine;
    using UnityEngine.AI;

    public class NavMeshAgentBridge : MonoBehaviour
    {

        public float arrivalDistance = 1;

        public bool hideNavMeshAgentComponent;

        private AgentController agent;

        private NavMeshAgent navAgent;

        private Transform target;

        private float distance;

        private Vector3[] corners;



        private void Awake()
        {
            agent = GetComponent<AgentController>();
            navAgent = GetComponent<NavMeshAgent>();
        }


        private void OnEnable()
        {
            navAgent.stoppingDistance = arrivalDistance;
            navAgent.autoBraking = true;
            // we give the unit a random avoidance priority so as to ensure that units will actually avoid each other (since same priority units will not try to avoid each other)
            navAgent.avoidancePriority = UnityEngine.Random.Range(0, 99);

            navAgent.enabled = true;
        }

        private void OnDisable()
        {
            navAgent.enabled = false;
        }


        private void FixedUpdate()
        {
            RotateTowardsTarget();
        }



        public void SetDestination(Vector3 destination, float maxDist = 2f, int areaMask = NavMesh.AllAreas) // maxDist is Sample within this distance from sourcePosition.
        {
            navAgent.speed = agent.stats.walkSpeed;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(destination, out hit, maxDist, areaMask))
            {
                destination = hit.position;
                navAgent.SetDestination(destination);
            }
        }


        public void StopMoving()
        {
            navAgent.velocity = Vector3.zero;
            navAgent.ResetPath();
        }


        /// <summary>
        /// Set the position to look at. Set to null is the Ai should stop looking
        /// </summary>
        /// <param name="lookAtTarget"></param>
        public virtual void LookAtTarget(Transform lookAtTarget)
        {
            target = lookAtTarget;
            if (lookAtTarget == null){
                navAgent.updateRotation = true;
            }
            else{
                navAgent.updateRotation = false;
            }
        }



        private void RotateTowardsTarget()
        {
            if (target != null)
            {
                Vector3 lookRotation = (target.position - transform.position);
                //  Errors out when roation method is called towards a vector zero.
                if (lookRotation != Vector3.zero)
                {
                    // Create a quaternion (rotation) based on looking down the vector from the player to the target.
                    Quaternion newRotatation = Quaternion.LookRotation(lookRotation);
                    transform.rotation = Quaternion.Slerp(transform.rotation, newRotatation, Time.fixedDeltaTime * agent.stats.turnSpeed);
                }
            }
        }


        private float GetDistanceRemaining()
        {
            corners = navAgent.path.corners;

            for (int c = 0; c < corners.Length - 1; c++)
            {
                distance += Mathf.Abs((corners[c] - corners[c + 1]).magnitude);
            }
            return distance;
        }


        private bool HasReachedDestination()
        {
            //return GetDistanceRemaining() <= arrivalDistance && agent.pathPending == false;
            return GetDistanceRemaining() <= arrivalDistance;
        }







        bool hasPath;
        private void MonitorPath()
        {
            if(hasPath){
                if(navAgent.remainingDistance < navAgent.stoppingDistance){
                    navAgent.isStopped = true;
                    hasPath = false;
                    //  OnReachPosition
                }
            }
            else{
                if(navAgent.remainingDistance > 2){
                    hasPath = true;
                }
            }
        }
    }
}



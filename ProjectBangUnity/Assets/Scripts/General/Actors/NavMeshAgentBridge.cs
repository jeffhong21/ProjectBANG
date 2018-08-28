namespace Bang
{
    using UnityEngine;
    using UnityEngine.AI;

    public class NavMeshAgentBridge : MonoBehaviour
    {

        public float arrivalDistance = 1;


        protected NavMeshAgent navMeshAgent;
        [SerializeField]
        private Transform target;
        private float distance;
        private Vector3[] corners;



        protected virtual void Awake()
        {
            
            navMeshAgent = GetComponent<NavMeshAgent>();
        }


        protected virtual void OnEnable()
        {
            navMeshAgent.stoppingDistance = arrivalDistance;
            navMeshAgent.autoBraking = true;
            // we give the unit a random avoidance priority so as to ensure that units will actually avoid each other (since same priority units will not try to avoid each other)
            navMeshAgent.avoidancePriority = UnityEngine.Random.Range(0, 99);

            navMeshAgent.enabled = true;
        }

        protected virtual void OnDisable()
        {
            navMeshAgent.enabled = false;
        }


        protected virtual void FixedUpdate()
        {
            RotateTowardsTarget();
        }



        public void SetDestination(Vector3 destination, float maxDist = 1f, int areaMask = NavMesh.AllAreas) // maxDist is Sample within this distance from sourcePosition.
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(destination, out hit, maxDist, areaMask))
            {
                destination = hit.position;
                navMeshAgent.SetDestination(destination);
            }
        }


        public void StopMoving()
        {
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.ResetPath();
        }


        /// <summary>
        /// Set the position to look at. Set to null is the Ai should stop looking
        /// </summary>
        /// <param name="lookAtTarget"></param>
        public virtual void LookAt(Transform lookAtTarget)
        {
            target = lookAtTarget;

            if (lookAtTarget == null)
            {
                navMeshAgent.updateRotation = true;
            }
            else
            {
                navMeshAgent.updateRotation = false;
            }
        }


        private void RotateTowardsTarget()
        {
            if (target != null)
            {
                Vector3 lookRotation = (target.position - transform.position);
                // Create a quaternion (rotation) based on looking down the vector from the player to the target.
                Quaternion newRotatation = Quaternion.LookRotation(lookRotation);
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotatation, Time.fixedDeltaTime * navMeshAgent.angularSpeed);
            }
        }


        private float GetDistanceRemaining()
        {
            corners = navMeshAgent.path.corners;

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
    }
}



namespace Bang
{
    using UnityEngine;
    using UnityEngine.AI;
    using System;
    using System.Collections.Generic;
    using UtilityAI;



    public class AgentInput : ActorInput, IAgentInput
    {
        public enum NavigationType { NavMeshAgent, CustomPath}

        [SerializeField]
        protected NavigationType navType = NavigationType.NavMeshAgent;
        [Header("----- Movement -----")]
        [SerializeField]
        protected float _moveSpeed = 6f;
        [SerializeField]
        protected float _acceleration = 8f;
        [SerializeField]
        protected float _angularSpeed = 270f;
        [Header("----- Misc -----")]
        [SerializeField]
        protected float _arrivalDistance = 1f;
        [SerializeField, HideInInspector]
        protected NavMeshAgent _agent;
        [SerializeField]
        protected NavMeshPath _path;


        Transform _target;
        bool hasPath;
        Vector3 currentDestination;



        public float moveSpeed{
            get { return _moveSpeed; }
            set { _moveSpeed = value; }
        }

        public float acceleration{
            get { return _acceleration; }
            set { _acceleration = value; }
        }

        public float angularSpeed{
            get { return _angularSpeed; }
            set { _angularSpeed = value; }
        }

        public float arrivalDistance{
            get { return _arrivalDistance; }
            set { _arrivalDistance = value; }
        }

        public NavMeshAgent agent{
            get { return _agent; }
        }

        public NavMeshPath path
        {
            get { return _path; }
            set { _path = value; }
        }

        public Vector3 position{
            get { return transform.position; }
        }


        protected override void Awake()
        {
            base.Awake();

            if (_agent == null) _agent = GetComponent<NavMeshAgent>();

        }


        protected override void OnEnable()
        {
            base.OnEnable();

            agent.speed = moveSpeed;
            agent.acceleration = acceleration;
            agent.angularSpeed = angularSpeed;
            agent.stoppingDistance = arrivalDistance;
            agent.autoBraking = false;
            // we give the unit a random avoidance priority so as to ensure that units will actually avoid each other (since same priority units will not try to avoid each other)
            agent.avoidancePriority = UnityEngine.Random.Range(0, 99);

            agent.enabled = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            agent.enabled = false;
        }


        protected virtual void Update()
        {
            if(HasReachedDestination()){
                StopWalking();
            }

            Turn();
            ////  Play walk animation.
            PlayAnimations();
        }


        public virtual void MoveTo(Vector3 destination)
        {
            isMoving = true;
            agent.isStopped = false;
            agent.SetDestination(destination);
            //Debug.LogFormat("Agent setting destination to:  {0} | {1}", destination, Time.time);
        }


        public virtual void StopWalking()
        {
            isMoving = false;
            //_anim.SetBool("IsWalking", false);
            agent.isStopped = true;

        }
 

        /// <summary>
        /// Set the position to look at. Set to null is the Ai should stop looking
        /// </summary>
        /// <param name="lookAtTarget"></param>
        public virtual void LookAt(Transform lookAtTarget)
        {
            _target = lookAtTarget;

            if (lookAtTarget == null)
            {
                agent.updateRotation = true;
            }
            else
            {
                agent.updateRotation = false;
            }
        }


        private void Turn()
        {
            if (_target != null)
            {
                Vector3 lookRotation = (_target.position - transform.position);
                // Create a quaternion (rotation) based on looking down the vector from the player to the target.
                Quaternion newRotatation = Quaternion.LookRotation(lookRotation);
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotatation, Time.fixedDeltaTime * angularSpeed);
            }
        }




        float GetDistanceRemaining()
        {
            float distance = 0.0f;

            Vector3[] corners;
            switch (navType)
            {
                case NavigationType.NavMeshAgent:
                    corners = agent.path.corners;
                    break;
                case NavigationType.CustomPath:
                    corners = path.corners;
                    break;
                default:
                    corners = new Vector3[0];
                    break;
            }

            for (int c = 0; c < corners.Length - 1; c++)
            {
                distance += Mathf.Abs((corners[c] - corners[c + 1]).magnitude);
            }
            return distance;
        }


        bool HasReachedDestination()
        {
            //return GetDistanceRemaining() <= arrivalDistance && agent.pathPending == false;
            return GetDistanceRemaining() <= arrivalDistance;
        }



    }
}
namespace Bang
{
    using UnityEngine;
    using UnityEngine.AI;
    using System;
    using System.Collections.Generic;


    using AtlasAI;


    /// <summary>
    /// Represents knowledge that the AI uses to do what it needs to do.
    /// </summary>
    [Serializable]
    public class AgentContext : IAIContext
    {
        
        public AgentContext(AgentController agent)
        {
            this.agent = agent;
            sampledPositions = new List<Vector3>();
            hostiles = new List<ActorHealth>();

            navMeshAgent = agent.GetComponent<NavMeshAgent>();
            if(navMeshAgent == null){
                Debug.Log("Context can't find navMeshAgent");
            }
        }

        [HideInInspector]
        public NavMeshAgent navMeshAgent;
        //[SerializeField]
        private List<Vector3> _sampledPositions;
        [SerializeField]
        private Vector3 _destination;
        [SerializeField]
        private Vector3 _lastTargetPosition;
        [SerializeField]
        private Vector3 _locationOfInterest;
        [SerializeField]
        private ActorHealth _attackTarget;
        [SerializeField]
        private Collider _coverTarget;
        [SerializeField]
        private Vector3 _coverPosition;
        [SerializeField]
        private List<ActorHealth> _hostiles;
        [SerializeField]
        private bool _isSearching;


        public AgentController agent
        {
            get;
            private set;
        }

        public List<Vector3> sampledPositions
        {
            get { return _sampledPositions; }
            set { _sampledPositions = value; }
        }


        public Vector3 destination
        {
            get{
                return _destination;
            }
            set{
                _destination = value;
            }
        }

        public Vector3 lastTargetPosition
        {
            get{
                return _lastTargetPosition;
            }
            set{
                _lastTargetPosition = value;
            }
        }

        public Vector3? locationOfInterest
        {
            get{
                return _locationOfInterest;
            }
            set{
                if (value == null)
                    _locationOfInterest = default(Vector3);
                else
                    _locationOfInterest = (Vector3)value;
            }
        }


        public ActorHealth attackTarget
        {
            get{
                return _attackTarget;
            }
            set
            {
                _attackTarget = value;
                _lastTargetPosition = _attackTarget.transform.position;

                _isSearching = false;
                agent.OnAttackTargetChanged(_attackTarget);
            }
        }


        public Collider coverTarget
        {
            get { return _coverTarget; }
            set { _coverTarget = value; }
        }


        public Vector3 coverPosition
        {
            get
            {
                if(coverTarget != null)
                {
                    Vector3 closestCoverPoint = coverTarget.ClosestPoint(agent.position);

                    return closestCoverPoint;
                }

                return Vector3.zero;
            }
            set{ _coverPosition = value; }
        }


        public List<ActorHealth> hostiles
        {
            get { 
                return _hostiles; 
            }
            set {
                _hostiles = value;
            }
        }


        public bool isSearching
        {
            get { return _isSearching; }
            set { _isSearching = value; }
        }


    }
}









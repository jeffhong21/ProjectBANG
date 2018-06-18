namespace Bang
{
    using UnityEngine;
    using UnityEngine.AI;
    using System;
    using System.Collections.Generic;


    using UtilityAI;


    /// <summary>
    /// Represents knowledge that the AI uses to do what it needs to do.
    /// </summary>
    [Serializable]
    public class AgentContext : IAIContext
    {
        
        public AgentContext(AgentCtrl agent)
        {
            this.agent = agent;
            sampledPositions = new List<Vector3>();
            waypoints = new Queue<Vector3>();
            hostiles = new List<IHasHealth>();

            navMeshAgent = agent.agentInput.agent;
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
        [SerializeField, ReadOnly]    
        private string __attackTargetName;       //  TEMP
        [SerializeField]
        private IHasHealth _attackTarget;
        [SerializeField]
        private Collider _coverTarget;
        [SerializeField]
        private Vector3 _coverPosition;
        //[SerializeField]
        private List<IHasHealth> _hostiles;
        [SerializeField, ReadOnly]
        private GameObject[] __hostiles;       //  TEMP
        [SerializeField]
        private bool _isSearching;


        public AgentCtrl agent
        {
            get;
            private set;
        }

        public List<Vector3> sampledPositions
        {
            get { return _sampledPositions; }
            set { _sampledPositions = value; }
        }

        public Queue<Vector3> waypoints
        {
            get;
            set;
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

        public IHasHealth attackTarget
        {
            get{
                return _attackTarget;
            }
            set
            {
                _attackTarget = value;
                _lastTargetPosition = _attackTarget.position;
                _isSearching = false;
                agent.OnAttackTargetChanged(_attackTarget);

                //  Temp
                __attackTargetName = _attackTarget.GetType().Name;  
            }
        }

        public Collider coverTarget
        {
            get { return _coverTarget; }
            set { _coverTarget = value; }
        }


        public Vector3 coverPosition
        {
            get{ return _destination;}
            set{ _destination = value; }
        }


        public List<IHasHealth> hostiles
        {
            get { 
                return _hostiles; 
            }
            set {
                _hostiles = value;

                //  Temp
                __hostiles = new GameObject[_hostiles.Count];
                for (int i = 0; i < _hostiles.Count; i ++)
                    __hostiles[i] = _hostiles[i].gameObject;
            }
        }


        public bool isSearching
        {
            get { return _isSearching; }
            set { _isSearching = value; }
        }


    }
}









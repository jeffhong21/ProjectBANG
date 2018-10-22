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
            _sampledPositions = new List<Vector3>();
            _hostiles = new List<ActorHealth>();
            _coverPositions = new List<Vector3>();

            navMeshAgent = agent.GetComponent<NavMeshAgent>();
            if(navMeshAgent == null) Debug.Log("Context can't find navMeshAgent");
            
        }


        [HideInInspector]
        public NavMeshAgent navMeshAgent;

        private List<Vector3> _sampledPositions;

        [SerializeField]
        private Vector3 _destination;

        [SerializeField]
        private ActorHealth _attackTarget;

        [SerializeField]
        private List<ActorHealth> _hostiles;

        [SerializeField]
        private bool _isSearching;

        [SerializeField]
        private CoverObject _coverTarget;

        [SerializeField]
        private List<Vector3> _coverPositions;

        [SerializeField]
        private Vector3 _coverPosition;

        public List<IOptionScorer<Vector3>> _positionScores;


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
            get{ return _destination;}
            set{_destination = value;}
        }


        public ActorHealth attackTarget
        {
            get{return _attackTarget;}
            set{
                _attackTarget = value;
                //_lastTargetPosition = _attackTarget.transform.position;
                if(value != null)
                    _isSearching = false;
                
                agent.OnAttackTargetChanged(_attackTarget);
            }
        }


        public List<ActorHealth> hostiles
        {
            get { return _hostiles; }
            set { _hostiles = value; }
        }



        public CoverObject coverTarget
        {
            get { return _coverTarget; }
            set { _coverTarget = value; }
        }


        public List<Vector3> CoverPositions{
            get { return _coverPositions; }
            set { _coverPositions = value; }
        }


        public Vector3 coverPosition
        {
            get{
                //if(coverTarget != null){
                //    Vector3 closestCoverPoint = coverTarget.ClosestPoint(agent.position);
                //    closestCoverPoint.y = 0;
                //    _coverPosition = closestCoverPoint;
                //    return _coverPosition;
                //}
                //return Vector3.zero;
                return _coverPosition;
            }
            set{ _coverPosition = value; }
        }


        public bool isSearching
        {
            get { return _isSearching; }
            set { _isSearching = value; }
        }




    }
}









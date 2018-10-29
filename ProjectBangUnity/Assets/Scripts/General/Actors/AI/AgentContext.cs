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
            _teammates = new List<ActorHealth>();

            navMeshAgent = agent.GetComponent<NavMeshAgent>();
            if(navMeshAgent == null) Debug.Log("Context can't find navMeshAgent");
            
        }


        [HideInInspector]
        public NavMeshAgent navMeshAgent;

        [SerializeField]
        private Vector3 _destination;

        [SerializeField]
        private ActorHealth _attackTarget;

        [SerializeField]
        private Vector3 _lastTargetPosition;

        [SerializeField]
        private List<ActorHealth> _hostiles;

        [SerializeField]
        private CoverObject _coverTarget;

        [SerializeField]
        private Vector3 _coverPosition;

        [SerializeField]
        private List<ActorHealth> _teammates;



        private List<Vector3> _sampledPositions;

        private List<Vector3> _coverPositions;

        [SerializeField]
        private List<IOptionScorer<Vector3>> _positionScores;






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
                //_lastTargetPosition = _attackTarget.transform.position;
                _attackTarget = value;
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
                return _coverPosition;
            }
            set{ _coverPosition = value; }
        }


        public List<IOptionScorer<Vector3>> PositionScores
        {
            get { return _positionScores; }
            set { _positionScores = value; }
        }


    }
}









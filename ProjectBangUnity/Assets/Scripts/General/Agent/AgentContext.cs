namespace Bang
{
    using UnityEngine;
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
            this._sampledPositions = new List<Vector3>();
            this.waypoints = new Queue<Vector3>();
            this.hostiles = new List<IHasHealth>();
        }



        [SerializeField]
        private List<Vector3> _sampledPositions;


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


        public Vector3? moveTarget
        {
            get;
            set;
        }

        public Vector3? locationOfInterest
        {
            get;
            set;
        }


        public IHasHealth focusTarget
        {
            get;
            set;
        }

        public List<IHasHealth> hostiles
        {
            get;
            set;
        }

    }
}









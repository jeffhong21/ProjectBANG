namespace Bang
{
    using System;
    using UnityEngine;

    using UtilityAI;

    public class AIContextProvider : MonoBehaviour, IContextProvider
    {


        [SerializeField]
        private AgentContext _context;
        public AgentContext context { get { return _context; } set { _context = value; } }


        void OnEnable()
        {
            _context = new AgentContext(GetComponent<AgentCtrl>());
        }


        public IAIContext GetContext()
        {
            return context as IAIContext;
        }

        public IAIContext GetContext(Guid aiId)
        {
            return context as IAIContext;
        }

    }
}

namespace Bang
{
    using System;
    using UnityEngine;

    using AtlasAI;

    public class AIContextProvider : MonoBehaviour, IContextProvider
    {


        [SerializeField]
        private AgentContext _context;
        public AgentContext context 
        { 
            get 
            { 
                return _context;
            }
            set 
            {
                _context = value;
            }
        }


        private void Awake()
        {
            _context = new AgentContext(GetComponent<AgentController>());
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

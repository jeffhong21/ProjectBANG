namespace uUtilityAI
{
    using System;
    using UnityEngine;


    public class AIContextProvider : MonoBehaviour, IContextProvider
    {


        [SerializeField]
        private IAIContext _context;
        public IAIContext context { get {return _context;} set {_context = value;}}


        private void OnEnable()
        {
            //_context = new IAIContext(GetComponent<EntityAIController>());
        }


        public IAIContext GetContext(){
            return context as IAIContext;
        }

        public IAIContext GetContext(Guid aiId){
            return GetContext();
        }

    }
}

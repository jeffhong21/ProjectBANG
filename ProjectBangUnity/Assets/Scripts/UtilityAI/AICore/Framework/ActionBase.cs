namespace AtlasAI
{
    
    [System.Serializable]
    public abstract class ActionBase : IAction
    {
        
        public string name { get; set; }



        public abstract void Execute(IAIContext context);


    
        public void CloneFrom(ActionBase other){
            //utilityAIComponent = other.utilityAIComponent;
        }


        public ActionBase(){
            //Debug.Log(actionStatus);
        }

        protected ActionBase(ActionBase other){
            //utilityAIComponent = other.utilityAIComponent;
        }


    }


}
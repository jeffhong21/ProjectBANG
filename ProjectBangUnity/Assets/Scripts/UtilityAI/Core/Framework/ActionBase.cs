namespace UtilityAI
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





    //public abstract class ActionBase<TContext> : IAction
    //{
    //    public string name { get; set; }

    //    public TaskNetworkComponent utilityAIComponent {get; set;} //  So it can start coroutine
    //    public ActionStatus actionStatus { get; protected set; }

    //    /// <summary>
    //    /// Execute methode for custom Context type.
    //    /// </summary>
    //    /// <param name="context"></param>
    //    public abstract void Execute(TContext context);

    //    /// <summary>
    //    /// Default execute for generic context.
    //    /// </summary>
    //    /// <param name="context"></param>
    //    public virtual void Execute(IAIContext context){
            
    //    }

    //    //public void EndAction(){
    //    //    //Debug.Log("Ending Action");
    //    //    if (actionStatus != ActionStatus.Running)
    //    //        return;

    //    //    actionStatus = ActionStatus.Success;
    //    //}


    //    //public void ExecuteAction(IAIContext context)
    //    //{
    //    //    //  TODO: Maybe add some check to see if action can be executed.
    //    //    //actionStatus = ActionStatus.Running;
    //    //    Execute(context);
    //    //}
    //}

}
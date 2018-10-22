namespace AtlasAI
{
    
    public interface IAction
    {
        //UtilityAIComponent utilityAIComponent {get; set;}
        //ActionStatus actionStatus { get;  }
        //void EndAction();
        string name { get; set; }
        void Execute(IAIContext context);
    }
}
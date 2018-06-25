namespace UtilityAI
{
    
    public interface IAction
    {
        //TaskNetworkComponent utilityAIComponent {get; set;}
        //ActionStatus actionStatus { get;  }
        //void EndAction();
        string name { get; set; }
        void Execute(IAIContext context);
    }
}
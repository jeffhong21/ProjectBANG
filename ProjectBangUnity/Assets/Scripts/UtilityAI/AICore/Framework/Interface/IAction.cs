namespace AtlasAI
{
    
    public interface IAction
    {
        string name { get; set; }

        void Execute(IAIContext context);
    }
}
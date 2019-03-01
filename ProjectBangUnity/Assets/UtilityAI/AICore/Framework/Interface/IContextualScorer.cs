namespace uUtilityAI
{

    public interface IContextualScorer
    {
        float Score(IAIContext context);
    }
}
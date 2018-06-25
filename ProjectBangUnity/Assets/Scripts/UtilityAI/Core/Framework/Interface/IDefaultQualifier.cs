namespace UtilityAI
{

    public interface IDefaultQualifier : IQualifier, ICanBeDisabled
    {

        float score
        {
            get;
            set;
        }


    }
}
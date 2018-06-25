namespace UtilityAI
{
    using System;

    /// <summary>
    /// Interface for Qualifier.
    /// Can be used as a base class in which the Qualifier generates a score via the Score method.
    /// </summary>
    public interface IQualifier : IComparable<IQualifier>
    {
        IAction action 
        { 
            get; 
            set;
        }

        float _score {
            get;
        }

        float Score(IAIContext context);
    }
}
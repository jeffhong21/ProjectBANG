namespace uUtilityAI
{
    using System;

    public interface ISelect
    {

        //
        // Properties
        //
        Guid id
        {
            get;
        }

        //
        // Methods
        //
        IAction Select(IAIContext context);


    }
}
using System;
using System.Collections.Generic;

namespace uUtilityAI
{
    public interface ICompositeScorer
    {
        //
        // Properties
        //
        List<IContextualScorer> scorers
        {
            get;
        }

        //
        // Methods
        //
        float Score(IAIContext context, List<IContextualScorer> scorers);
    }
}

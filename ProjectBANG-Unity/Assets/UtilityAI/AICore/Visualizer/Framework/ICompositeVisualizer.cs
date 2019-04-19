using System;
using System.Collections;

namespace uUtilityAI.Visualization
{
    public interface ICompositeVisualizer
    {
        //
        // Properties
        //
        IList children
        {
            get;
        }

        //
        // Methods
        //
        void Add(object item);
    }
}

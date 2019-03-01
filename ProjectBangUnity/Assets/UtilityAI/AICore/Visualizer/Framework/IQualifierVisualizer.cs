using System;

namespace uUtilityAI.Visualization
{
    public interface IQualifierVisualizer : IQualifier
    {
        //
        // Properties
        //
        bool isHighScorer{
            get;
        }

        float? lastScore{
            get;
        }

        SelectorVisualizer parent {
            get;
        }

        IQualifier qualifier {
            get;
        }


        //
        // Methods
        //
        void Init();

        void Reset();

    }
}

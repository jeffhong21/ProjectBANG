namespace UtilityAI
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public struct OptionScorerComparer<T> : IComparer<OptionScorer<T>>
    {
        public bool _descending;

        //public ScoredOptionComparer(bool descending)
        //{
            
        //}



        int IComparer<OptionScorer<T>>.Compare(OptionScorer<T> x, OptionScorer<T> y)
        {
            throw new NotImplementedException();
        }
    }

}
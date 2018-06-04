namespace UtilityAI
{
    using System;
    using System.Collections.Generic;

    [System.Serializable]
    public struct ScoredOptionComparer<T> : IComparer<ScoredOption<T>>
    {
        public bool _descending;

        //public ScoredOptionComparer(bool descending)
        //{
            
        //}



        int IComparer<ScoredOption<T>>.Compare(ScoredOption<T> x, ScoredOption<T> y)
        {
            throw new NotImplementedException();
        }
    }

}
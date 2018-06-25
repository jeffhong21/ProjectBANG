namespace UtilityAI
{

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A base class for Qualifiers that base their score on a number of child.
    /// </summary>
    public abstract class CompositeQualifier : IQualifier, ICanBeDisabled
    {
        // //  Used for CompareTo().  
        // public float _score { get; protected set; }  // (IQualifier)

        protected List<IScorer> _scorers;

        // (IQualifier)
        public IAction action { 
            get; 
            set; 
        }

        // (IQualifier)
        public bool isDisabled { 
            get; 
            set; 
        }

        public List<IScorer> scorers 
        {
            get { return _scorers; }
            set { _scorers = value; }  //  Do not need?
        }

        public float _score { 
            get; 
            protected set; 
        }

        protected CompositeQualifier()
        {
            _scorers = new List<IScorer>();
        }

        public virtual void CloneFrom (object other)
        {

        }

        // (IQualifier)
        public float Score(IAIContext context)
        {
            var score = 0f;
            if (scorers.Count == 0)
                return score;
            
            foreach (IScorer scorer in scorers)
            {
                score += scorer.Score(context);
            }

            _score = score;
            return score;
            //return Score(context, scorers);
        }

        public abstract float Score(IAIContext context, List<IScorer> scorers);


        public int CompareTo(IQualifier other)
        {
            //  Current instance is greater than object being compared too.
            if (other == null) return 1;
            return this._score.CompareTo(other._score);
        }

    }







}


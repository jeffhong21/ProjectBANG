namespace AtlasAI
{

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A base class for Qualifiers that base their score on a number of child.
    /// </summary>
    public abstract class CompositeQualifier : IQualifier, ICompositeScorer, ICanBeDisabled
    {
        // //  Used for CompareTo().  
        // public float _score { get; protected set; }  // (IQualifier)

        protected List<IContextualScorer> _scorers;

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

        public List<IContextualScorer> scorers 
        {
            get { return _scorers; }
            set { _scorers = value; }  //  Do not need?
        }

        public float _score { 
            get; 
            protected set; 
        }


        //
        // Constructors
        //
        protected CompositeQualifier()
        {
            _scorers = new List<IContextualScorer>();
        }



        public virtual void CloneFrom (object other)
        {

        }

        // (IQualifier)
        public float Score(IAIContext context)
        {
            return Score(context, scorers);

            //var score = 0f;
            //if (scorers.Count == 0)
            //    return score;
            
            //foreach (IContextualScorer scorer in scorers)
            //{
            //    score += scorer.Score(context);
            //}

            //_score = score;
            //return score;
            ////return Score(context, scorers);
        }

        public abstract float Score(IAIContext context, List<IContextualScorer> scorers);




        public int CompareTo(IQualifier other)
        {
            //  Current instance is greater than object being compared too.
            if (other == null) return 1;
            return this._score.CompareTo(other._score);
        }

    }







}


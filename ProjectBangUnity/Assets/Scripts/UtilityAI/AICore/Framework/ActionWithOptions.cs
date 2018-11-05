using System;
using System.Collections.Generic;

namespace AtlasAI
{
    public abstract class ActionWithOptions<TOption> : IAction
    {
        public string name { get; set; }
        //
        // Fields
        //

        protected List<IOptionScorer<TOption>> _scorers;

        //
        // Properties
        //
        public List<IOptionScorer<TOption>> scorers
        {
            get { return _scorers; }
            set { _scorers = value; }
        }

        //
        // Constructors
        //
        protected ActionWithOptions()
        {
            
        }

        //
        // Methods

        public abstract void Execute(IAIContext context);




        public TOption GetBest(IAIContext context, List<TOption> options)
        {
            List<ScoredOption<TOption>> scoredOptions = GetAllScores(context, options);

            scoredOptions.Sort(new ScoredOptionComparer<TOption>());

            return scoredOptions[0].option;
        }




        public List<ScoredOption<TOption>> GetAllScores(IAIContext context, List<TOption> options)
        {
            List<ScoredOption<TOption>> scoredOptions = new List<ScoredOption<TOption>>();

            for (int i = 0; i < options.Count; i++)
            {
                //  For each option, loop through all the option scorers.
                var option = options[i];
                var score = 0f;

                for (int index = 0; index < scorers.Count; index++)
                {
                    score += scorers[index].Score(context, option);
                }

                ScoredOption<TOption> scoredOption = new ScoredOption<TOption>(option, score);
                scoredOptions.Add(scoredOption);
            }

            return scoredOptions;
        }



        ///<summary>
        /// Gets all options with the score they received from the<see cref= "P:Apex.AI.ActionWithOptions`1.scorers" />.
        /// optionBuffer parameter is a list from a ListBufferPool
        ///</ summary >
        ///< param name= "context" > The context.</param>
        ///<param name = "options" > The options.</param>
        ///<param name = "optionsBuffer" > The buffer which is populated with the scored options.</param>
        public void GetAllScores(IAIContext context, List<TOption> options, List<ScoredOption<TOption>> optionsBuffer)
        {
            for (int i = 0; i < options.Count; i++)
            {
                //  For each option, loop through all the option scorers.
                var option = options[i];
                var score = 0f;

                for (int index = 0; index < scorers.Count; index++)
                {
                    score += scorers[index].Score(context, option);
                }

                ScoredOption<TOption> scoredOption = new ScoredOption<TOption>(option, score);
                optionsBuffer.Add(scoredOption);
            }
        }
    }
}

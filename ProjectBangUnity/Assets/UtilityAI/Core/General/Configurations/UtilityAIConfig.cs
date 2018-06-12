namespace UtilityAI
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;



    public abstract class UtilityAIAssetConfig : IUtilityAIConfig
    {
        public AIStorage asset;

        public string name{
            get;
            set;
        }

        protected IAction a;
        protected IScorer scorer;
        protected List<IScorer> scorers;
        protected IQualifier q;
        protected Selector s;

        protected List<IQualifier> qualifiers;
        protected List<IScorer[]> allScorers;
        protected List<IAction> actions;


        public abstract void ConfigureAI(IUtilityAI asset);

    }

}


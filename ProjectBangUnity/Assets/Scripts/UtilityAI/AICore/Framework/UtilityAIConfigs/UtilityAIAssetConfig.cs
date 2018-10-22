namespace AtlasAI
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;



    public abstract class UtilityAIAssetConfig : IUtilityAIAssetConfig
    {
        public AIStorage asset;

        public string name{
            get;
            set;
        }

        protected Selector rs;
        protected IAction a;
        protected IScorer scorer;
        protected List<IScorer> scorers;
        protected IQualifier q;
        protected Selector s;

        protected List<IQualifier> qualifiers;
        protected List<IScorer[]> allScorers;
        protected List<IAction> actions;



        protected void InitializeAI(IUtilityAI asset)
        {
            if (qualifiers == null) 
                qualifiers = new List<IQualifier>();
            
            if (allScorers == null) 
                allScorers = new List<IScorer[]>();
            
            if (actions == null) 
                actions = new List<IAction>();

            rs = asset.rootSelector;
        }


        /// <summary>
        /// Setup each qualifiers action and scorers.
        /// </summary>
        /// <param name="debugFinish">If set to <c>true</c> debug finish.</param>
        protected void ConfigureAI(bool debugFinish = false)
        {
            //if (debugFinish) Debug.Log("ConfigureAI qualifier count: " + qualifiers.Count);
            for (int index = 0; index < qualifiers.Count; index++)
            {
                //  Add qualifier to rootSelector.
                rs.qualifiers.Add(qualifiers[index]);
                var qualifier = rs.qualifiers[index];
                //  Set qualifier's action.
                qualifier.action = actions[index];
                //  Add scorers to qualifier.
                foreach (IScorer scorer in allScorers[index])
                {
                    if (qualifier is CompositeQualifier)
                    {
                        var q = qualifier as CompositeQualifier;
                        q.scorers.Add(scorer);
                    }
                }
            }

            if (debugFinish) Debug.Log("Finish Initializing " + this.GetType().Name);


        }



        public abstract void SetupAI(IUtilityAI asset);


    }

}


namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    using AtlasAI;

    [Serializable]
    public class AgentScanAI : UtilityAIAssetConfig
    {
        
        /*
        private IAction a;
        private IScorer scorer;
        private List<IScorer> scorers;
        private IQualifier q;
        private Selector s;

        private List<IQualifier> qualifiers;
        private List<IScorer[]> allScorers;
        private List<IAction> actions;
        */


        public AgentScanAI()
        {
            name = "AgentScanAI";
        }

        public override void SetupAI(IUtilityAI asset)
        {
            //  Initializes the setup of AI.
            InitializeAI(asset);



            //// ---- New Action ----
            //a = new CompositeAction()
            //{
            //    name = "Scan",
            //    actions = new List<IAction>()
            //    {
            //        new ScanForEntities(){name = "ScanForEntitites"},
            //        new ScanForPositions(){ name = "ScanForPositions", samplingRange = 20, samplingDensity = 2.5f }
            //    }
            //};
            //actions.Add(a);  // --  Add to Actions Group
            //
            //// ---- New Scorers Group ----
            //scorers = new List<IScorer>();
            ////
            //// ---- New Scorer ----
            //scorer = new FixedScorer() { score = 5 };
            //scorers.Add(scorer);
            ////
            //// ---- Add All Scorers to Scorers Group ----
            //allScorers.Add(scorers.ToArray());
            //
            //// ---- New Qualifier ----
            //q = new CompositeScoreQualifier();
            //qualifiers.Add(q);



            //
            //  Default Qualifier
            //
            rs.defaultQualifier.action = new CompositeAction()
            {
                name = "Scan",
                actions = new List<IAction>()
                {
                    new ScanForEntities(){name = "ScanForEntitites"},
                    new ScanForPositions(){ name = "ScanForPositions", samplingRange = 20, samplingDensity = 2.5f }
                }
            };




            //  
            //  Configure AI.  Setup each qualifiers action and scorers.
            //
            base.ConfigureAI();
        }






    }

}


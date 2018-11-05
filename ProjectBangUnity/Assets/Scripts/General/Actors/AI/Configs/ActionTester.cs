//namespace Bang
//{
//    using UnityEngine;
//    using System;
//    using System.Collections.Generic;

//    using AtlasAI;

//    [Serializable]
//    public class ActionTester : UtilityAIAssetConfig
//    {

//        /*
//        private IAction a;
//        private IContextualScorer scorer;
//        private List<IContextualScorer> scorers;
//        private IQualifier q;
//        private Selector s;

//        private List<IQualifier> qualifiers;
//        private List<IContextualScorer[]> allScorers;
//        private List<IAction> actions;
//        */

//        public ActionTester()
//        {
//            name = "ActionTester";
//        }

//        public override void SetupAI(IUtilityAI asset)
//        {
//            //  Initializes the setup of AI.
//            InitializeAI(asset);


//            // ---- New Action ----
//            a = new CompositeAction()
//            {
//                name = "SearchForTarget",
//                actions = new List<IAction>()
//                {
//                    new SearchForTargets(){name = "SearchForTarget"},
//                }
//            };
//            actions.Add(a);  // --  Add to Actions Group

//            // ---- New Scorers Group ----
//            scorers = new List<IContextualScorer>();
//            // ---- Add Scorers ----
//            scorer = new FixedScorer() { score = 10 };
//            scorers.Add(scorer);  // --  Add to Scorers Group
//            allScorers.Add(scorers.ToArray());

//            // ---- New Qualifier ----
//            q = new CompositeScoreQualifier();
//            qualifiers.Add(q);



//            rs.defaultQualifier.action = new EmptyAction()
//            {
//                name = "Null Action"
//            };



//            //  
//            //  Configure AI.  Setup each qualifiers action and scorers.
//            //
//            base.ConfigureAI();

//        }






//    }

//}


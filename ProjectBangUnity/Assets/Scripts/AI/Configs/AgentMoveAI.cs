namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    using AtlasAI;

    [Serializable]
    public class AgentMoveAI : UtilityAIAssetConfig
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


        public AgentMoveAI()
        {
            name = "AgentMoveAI";
        }


        public override void SetupAI(IUtilityAI asset)
        {
            //  Initializes the setup of AI.
            InitializeAI(asset);

            //rs.debugScores = true;
            //rs.debugIfDefault = true;






            #region MoveToBestAttackPosition

            // ---- New Action ----
            a = new MoveToBestPosition()
            {
                name = "MoveToBestAttackPosition",
                scorers = new List<IOptionScorer<Vector3>>()
                {
                    new PositionProximityToSelf(),      //  How close each point is to agent.
                    new ProximityToNearestEnemy(),      //  How close each point is to each enemy.
                    new OverRangeToClosestEnemy(),      //  If point is over a certain range to each enemy.
                    new LineOfSightToAnyEnemy(),        //  Does each point have line of sight to each enemy.
                    new LineOfSightToClosestEnemy(),    //  Does each point have line of sight to closest enemy.
                    new OverRangeToAnyEnemy()           //  If point is over range to any enemy.
                }
            };
            actions.Add(a);  // --  Add to Actions Group

            // ---- New Scorers Group ----
            scorers = new List<IScorer>();
            //
            // ---- Add Scorers ----
            scorer = new HasEnemies() { score = 25, not = false };
            scorers.Add(scorer);
            // ---- New Scorer ----
            scorer = new HasEnemiesInRange() { score = 20, range = 8 };
            scorers.Add(scorer);
            //
            // ---- Add All Scorers to Scorers Group ----
            allScorers.Add(scorers.ToArray());

            // ---- New Qualifier ----
            q = new CompositeAllOrNothingQualifier() { threshold = 20 };
            //q = new CompositeScoreQualifier();
            qualifiers.Add(q);

            #endregion



            #region MoveToCover

            //// ---- New Action ----
            //a = new CompositeAction()
            //{
            //    name = "MoveToCover",
            //    actions = new List<IAction>()
            //    {
            //        new FindClosestCover(){name = "FindClosestCover"},
            //        new MoveToCover(){name = "MoveToCover"}
            //    }
            //};
            //actions.Add(a);  // --  Add to Actions Group

            //// ---- New Scorers Group ----
            //scorers = new List<IScorer>();
            ////
            //// ---- New Scorer ----
            //scorer = new ShouldFindCover() { score = 15 };
            //scorers.Add(scorer);
            //// ---- New Scorer ----
            //scorer = new HasEnemiesInRange() { score = 10, range = 4 };
            //scorers.Add(scorer);
            //// ---- New Scorer ----
            //scorer = new HasCoverPosition() { score = 5};
            //scorers.Add(scorer);
            ////
            //// ---- Add All Scorers to Scorers Group ----
            //allScorers.Add(scorers.ToArray());

            //// ---- New Qualifier ----
            //q = new CompositeAllOrNothingQualifier() { threshold = 20 };
            ////q = new CompositeScoreQualifier();
            //qualifiers.Add(q);

            #endregion



            #region StopMovement

            //// ---- New Action ----
            //a = new StopMovement()
            //{
            //    name = "StopMovement",
            //};
            //actions.Add(a);  // --  Add to Actions Group

            //// ---- New Scorers Group ----
            //scorers = new List<IScorer>();
            ////
            //// ---- Add Scorers ----
            //scorer = new IsAiming() { score = 50 };
            //scorers.Add(scorer);  // --  Add to Scorers Group
            ////
            //// ---- Add All Scorers to Scorers Group ----
            //allScorers.Add(scorers.ToArray());

            //// ---- New Qualifier ----
            //q = new CompositeAllOrNothingQualifier() { threshold = 50 };
            ////q = new CompositeScoreQualifier();
            //qualifiers.Add(q);

            #endregion




            //
            //  Default Qualifier
            //
            rs.defaultQualifier.score = 1;
            rs.defaultQualifier.action = new SearchForTargets() 
            {
                name = "SearchForTarget"
            };




            //  
            //  Configure AI.  Setup each qualifiers action and scorers.
            //
            base.ConfigureAI();
        }





    }

}


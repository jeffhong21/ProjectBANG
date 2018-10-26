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
            //scorer = new HasEnemiesInRange() { score = 50, range = 10 };
            //scorers.Add(scorer);
            //// ---- New Scorer ----
            //scorer = new HasCoverPosition() { score = 45 };
            //scorers.Add(scorer);
            //// ---- New Scorer ----
            //scorer = new HealthBelowThreshold() { score = 20, threshold = 2 };
            //scorers.Add(scorer);

            ////
            //// ---- Add All Scorers to Scorers Group ----
            //allScorers.Add(scorers.ToArray());

            //// ---- New Qualifier ----
            ////q = new CompositeAllOrNothingQualifier() { threshold = 35 };
            //q = new CompositeScoreQualifier();
            //qualifiers.Add(q);

            #endregion



            #region Move to Cover Positiion

            // ---- New Action ----
            a = new CompositeAction()
            {
                name = "Move to Cover Positiion",
                actions = new List<IAction>()
                {
                    new GetCoverPositions(){name = "Get Cover Positions"},
                    new FindClosestCover(){name = "Find Closest Cover"},
                    new MoveToCoverPositions()
                    {
                        name = "Move to Cove Position",
                        scorers = new List<IOptionScorer<Vector3>>()
                        {
                            new PositionProximityToSelf() { score = 10, factor = 0.01f },      //  How close each point is to agent.
                            new OverRangeToClosestEnemy() { desiredRange = 5f, score = 100f },      //  If point is over a certain range to each enemy.
                        }
                    }
                }
            };
            actions.Add(a);  // --  Add to Actions Group

            // ---- New Scorers Group ----
            scorers = new List<IScorer>();
            //
            // ---- New Scorer ----
            scorer = new HasEnemiesInRange() { score = 50, range = 10 };
            scorers.Add(scorer);
            // ---- New Scorer ----
            scorer = new HasCoverPosition() { score = 50 };
            scorers.Add(scorer);
            // ---- New Scorer ----
            scorer = new HealthBelowThreshold() { score = 15, threshold = 2 };
            scorers.Add(scorer);
            // ---- New Scorer ----
            scorer = new ShouldFindCover() { score = 15 };
            scorers.Add(scorer);
            //
            // ---- Add All Scorers to Scorers Group ----
            allScorers.Add(scorers.ToArray());

            // ---- New Qualifier ----
            q = new CompositeAllOrNothingQualifier() { threshold = 100 };
            //q = new CompositeScoreQualifier();
            qualifiers.Add(q);

            #endregion



            #region MoveToBestAttackPosition

            // ---- New Action ----
            a = new MoveToBestPosition()
            {
                name = "MoveToBestAttackPosition",
                scorers = new List<IOptionScorer<Vector3>>()
                {
                    new PositionProximityToSelf() { score = 10, factor = 0.01f },      //  How close each point is to agent.
                    new ProximityToNearestEnemy() { desiredRange = 14f, score = 10f },      //  How close each point is to each enemy.
                    new OverRangeToClosestEnemy() { desiredRange = 5f, score = 100f },      //  If point is over a certain range to each enemy.
                    new LineOfSightToAnyEnemy() { score = 50f },        //  Does each point have line of sight to each enemy.
                    new LineOfSightToClosestEnemy() { score = 50f },    //  Does each point have line of sight to closest enemy.
                    new OverRangeToAnyEnemy() { desiredRange = 5f, score = 50f }          //  If point is over range to any enemy.
                }
            };
            actions.Add(a);  // --  Add to Actions Group

            // ---- New Scorers Group ----
            scorers = new List<IScorer>();
            //
            // ---- New Scorer ----
            scorer = new HasEnemiesInRange() { score = 150, range = 3 };
            scorers.Add(scorer);
            //
            // ---- Add All Scorers to Scorers Group ----
            allScorers.Add(scorers.ToArray());

            // ---- New Qualifier ----
            q = new CompositeAllOrNothingQualifier() { threshold = 100 };
            //q = new CompositeScoreQualifier();
            qualifiers.Add(q);

            #endregion



            #region Move In Cover

            // ---- New Action ----
            a = new MoveToPeekPosition()
            {
                name = "Move To Peek Position",
            };
            actions.Add(a);  // --  Add to Actions Group

            // ---- New Scorers Group ----
            scorers = new List<IScorer>();
            //
            // ---- New Scorer ----
            scorer = new IsInCover() { score = 125 };
            scorers.Add(scorer);
            //
            // ---- Add All Scorers to Scorers Group ----
            allScorers.Add(scorers.ToArray());

            // ---- New Qualifier ----
            q = new CompositeAllOrNothingQualifier() { threshold = 100 };
            qualifiers.Add(q);


            #endregion



            #region SearchForTarget

            // ---- New Action ----
            a = new SearchForTargets()
            {
                name = "Search For Target",
            };
            actions.Add(a);  // --  Add to Actions Group

            // ---- New Scorers Group ----
            scorers = new List<IScorer>();
            //
            // ---- New Scorer ----
            scorer = new IsSearchingForTargets() { score = 15 };
            scorers.Add(scorer);
            // ---- New Scorer ----
            scorer = new HasEnemies() { score = 45, not = true};  //  If agent has no hostiles ,than it scores.
            scorers.Add(scorer);
            // ---- New Scorers ----
            scorer = new HasArrivedToDestination() { score = -10, not = true };
            scorers.Add(scorer);
            //
            // ---- Add All Scorers to Scorers Group ----
            allScorers.Add(scorers.ToArray());

            // ---- New Qualifier ----
            q = new CompositeScoreQualifier();
            qualifiers.Add(q);

            #endregion







            //
            //  Default Qualifier
            //
            rs.defaultQualifier.score = 75;
            rs.defaultQualifier.action = new EmptyAction() 
            {
                name = "Default Null Action"
            };




            //  
            //  Configure AI.  Setup each qualifiers action and scorers.
            //
            base.ConfigureAI();
        }





    }

}

